using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using SAPbobsCOM;
using Service_SAP_MAX.Entities;
using Service_SAP_MAX.Entities.Response;
using Service_SAP_MAX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Process
{

    public class AccountingAccountsProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(GLComponentProcess));
        internal static void Process(ref SAPbobsCOM.Company oCompany, List<Entities.ConfigClass> listConfig)
        {
            try
            {
                var listComp = GetAccounts(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_CHART_ACC).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listComp)
                {

                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendAccount(item, oCompany, listConfig, url, maxAuth, authorization);
                    else
                        UpdateSendAccount(item, oCompany, listConfig, url, maxAuth, authorization);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateSendAccount(ChartAccountsClass item, Company oCompany, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {
                url = url.Replace("?lean=1", "");
                url = url + item.idMaximo + "?lean=1";

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                        IgnoreSerializableAttribute = true
                    }
                };
                settings.ContractResolver = new CustomResolver(new[] { "glaccount", "activedate", "glcomp01", "glcomp02", "glcomp03", "glcomp04" });

                var jsonBody = JsonConvert.SerializeObject(item, settings);
                var properties = "active, glaccount, activedate, accountname, glcomp01, glcomp02, glcomp03, glcomp04, orgid, sendersysid, sourcesysid, chartofaccountsid";
                var response = RestHelper.SendRest(url, Method.Post, maxAuth, authorization, jsonBody, properties, "PATCH", "MERGE");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<ChartOfAccountsResponseClass>(response.Content);
                    UpdateState("S", "Enviado", oCompany, item);
                }
                else
                {
                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    logger.Error(response.Content);
                    string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                    UpdateState("E", msg, oCompany, item);
                }
            }
            catch (Exception ex)
            {

                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void SendAccount(ChartAccountsClass item, Company oCompany, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {
                var jsonBody = JsonConvert.SerializeObject(item);
                var properties = "active, glaccount, activedate, accountname, glcomp01, glcomp02, glcomp03, glcomp04, orgid, sendersysid, sourcesysid, chartofaccountsid";
                var response = RestHelper.SendRest(url, Method.Post, maxAuth, authorization, jsonBody, properties);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<ChartOfAccountsResponseClass>(response.Content);
                    UpdateState("S", "Enviado", oCompany, item, resp.chartofaccountsid.ToString());
                }
                else
                {
                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    logger.Error(response.Content);
                    string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                    UpdateState("E", msg, oCompany, item);
                }
            }
            catch (Exception ex)
            {

                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateState(string state, string message, Company oCompany, ChartAccountsClass item, string id = "")
        {
            Recordset recordset = null;
            try
            {
                string query = "";
                string setId = "";
                if (!string.IsNullOrEmpty(id))
                    setId = $@" ,""{Constants.U_EXX_MAX_ID}""='{id}' ";

                query = $"UPDATE \"@{Constants.TABLE_ACCT}\" SET \"{Constants.U_EXX_MAX_STD}\" = '{state}',\"{Constants.U_EXX_MAX_MSJ}\" = '{message}' {setId} WHERE \"Code\"='{item.codeSAP}'";


                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset.DoQuery(query);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
            finally
            {
                // 🔹 Liberar memoria del objeto COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
                recordset = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static List<ChartAccountsClass> GetAccounts(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ChartAccountsClass> list = new List<ChartAccountsClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT 
                                ""Code"",
                                 C1.""PrcName"" ||' + ' || 
                                 C2.""PrcName"" ||' + ' || 
                                 C3.""PrcName"" ||' + ' || 
                                 C4.""PrcName"" ||' + ' || 
                                 C5.""PrcName"" 
                                 as  ""accountname"",
                                ""U_EXX_GL1"" as ""glcomp01"",
                                ""U_EXX_GL2"" as ""glcomp02"",
                                ""U_EXX_GL3"" as ""glcomp03"",
                                ""U_EXX_GL4"" as ""glcomp04"",
                                ""U_EXX_GL5"" as ""glcomp05"",
                                IFNULL(ACT.""U_EXX_MAX_ID"",'') as ""idMaximo"",
                                IFNULL(ACT.""U_EXX_MAX_ACT"",'') as ""active""
                                FROM
                                ""@{Constants.TABLE_ACCT}""   ACT  
                                INNER JOIN ""OPRC"" C1 ON C1.""PrcCode""= ACT.""U_EXX_GL1"" 
                                INNER JOIN ""OPRC"" C2 ON C2.""PrcCode""= ACT.""U_EXX_GL2"" 
                                INNER JOIN ""OPRC"" C3 ON C3.""PrcCode""= ACT.""U_EXX_GL3"" 
                                INNER JOIN ""OPRC"" C4 ON C4.""PrcCode""= ACT.""U_EXX_GL4"" 
                                INNER JOIN ""OPRC"" C5 ON C5.""PrcCode""= ACT.""U_EXX_GL5"" 
                                --INNER JOIN ""OACT"" CT ON SUBSTRING(CT.""FormatCode"",0,8)= ACT.""U_EXX_GL1""    
                                WHERE
                                ACT.""U_EXX_MAX_STD"" = 'P'

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    ChartAccountsClass doc = new ChartAccountsClass();
                    doc.codeSAP = (string)recordset.Fields.Item("Code").Value;
                    doc.idMaximo = (string)recordset.Fields.Item("idMaximo").Value;
                    doc.active = recordset.Fields.Item("active").Value.ToString() == "Y" ? true : false;
                    doc.accountname = (string)recordset.Fields.Item("accountname").Value;
                    doc.glaccount = (string)recordset.Fields.Item("Code").Value;
                    doc.glcomp01 = (string)recordset.Fields.Item("glcomp01").Value;
                    doc.glcomp02 = (string)recordset.Fields.Item("glcomp02").Value;
                    doc.glcomp03 = recordset.Fields.Item("glcomp03").Value.ToString();
                    doc.glcomp04 = recordset.Fields.Item("glcomp04").Value.ToString();
                    doc.glcomp05 = recordset.Fields.Item("glcomp05").Value.ToString();
                    doc.orgid = "TPE";//(string)recordset.Fields.Item("TableName").Value;
                    doc.sourcesysid = "SAPBO";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.sendersysid = "SAPBO";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.activedate = DateTime.Now;

                    list.Add(doc);
                    recordset.MoveNext();
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
            finally
            {
                // 🔹 Liberar memoria del objeto COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
                recordset = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return list;
        }
    }

}
