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
    public class GLComponentProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(GLComponentProcess));

        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listComp = GetComponents(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_GL_COMP).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listComp)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendGLComponent(item, oCompany, listConfig, url ,maxAuth, authorization);
                    else
                        UpdateGLComponent(item, oCompany, listConfig, url, maxAuth, authorization);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateGLComponent(GLComponentClass item, Company oCompany, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {
                url = url.Replace("?lean=1", "");
                url = url +  item.idMaximo + "?lean=1";

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                        IgnoreSerializableAttribute = true
                    }
                };
                settings.ContractResolver = new CustomResolver(new[] { "compvalue", "glorder" });

                var jsonBody = JsonConvert.SerializeObject(item,settings);
                var properties = "active, compvalue, comptext, glorder, orgid, userid, sourcesysid, glcomponentsid";
                var response = RestHelper.SendRest(url, Method.Post, maxAuth, authorization, jsonBody, properties,"PATCH","MERGE");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<GLComponentResponseClass>(response.Content);
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

        private static void SendGLComponent(GLComponentClass item, Company oCompany, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {
                var jsonBody = JsonConvert.SerializeObject(item);
                var properties = "active, compvalue, comptext, glorder, orgid, userid, sourcesysid, glcomponentsid";
                var response = RestHelper.SendRest(url, Method.Post, maxAuth, authorization, jsonBody,properties);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<GLComponentResponseClass>(response.Content);
                    UpdateState("S", "Enviado", oCompany, item, resp.glcomponentsid.ToString());
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

        private static void UpdateState(string state, string message, Company oCompany, GLComponentClass item, string id="")
        {
            Recordset recordset = null;
            try
            {
                string query = "";
                string setId = "";
                if (!string.IsNullOrEmpty(id))
                    setId = $@" ,""{Constants.U_EXX_MAX_ID}""='{id}' ";

                if (item.glorder == 1)
                    query = $"UPDATE \"OACT\" SET \"{Constants.U_EXX_MAX_STD}\" = '{state}',\"{Constants.U_EXX_MAX_MSJ}\" = '{message}' {setId} WHERE \"AcctCode\"='{item.codeSAP}'";
                if (item.glorder == 2)
                    query = $"UPDATE \"OPRC\" SET \"{Constants.U_EXX_MAX_STD}\" = '{state}',\"{Constants.U_EXX_MAX_MSJ}\" = '{message}' {setId} WHERE \"PrcCode\"='{item.codeSAP}'";
                 if (item.glorder == 3)
                    query = $"UPDATE \"OPRC\" SET \"{Constants.U_EXX_MAX_STD}\" = '{state}',\"{Constants.U_EXX_MAX_MSJ}\" = '{message}' {setId} WHERE \"PrcCode\"='{item.codeSAP}'";


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

        private static List<GLComponentClass> GetComponents(Company oCompany, List<ConfigClass> listConfig)
        {
            List<GLComponentClass> list = new List<GLComponentClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT TOP 1
                                ""AcctCode"" as ""codeSAP"",
                                ""AcctName"" as  ""comptext"",
                                SUBSTRING(""FormatCode"",0,8)  as ""compvalue"",
                                1 as ""glorder"",
                                IFNULL(""U_EXX_MAX_ID"",'') as ""idMaximo"",
                                    '' as ""Active""
                                FROM
                                ""OACT""
                                WHERE
                                ""U_EXX_MAX_STD"" = 'P' AND
                                ""Levels"" = 15 AND  IFNULL(""U_EXX_MAX_ID"",'') <>''

                                UNION ALL

                                SELECT Top 1
                                ""PrcCode"" as ""codeSAP"",
                                ""PrcName"" as  ""comptext"",
                                ""PrcCode"" as ""compvalue"",
                                2 as ""glorder"",
                                IFNULL(""U_EXX_MAX_ID"",'') as ""idMaximo"",
                                ""Active""
                                FROM
                                ""OPRC""
                                WHERE
                                ""U_EXX_MAX_STD"" = 'ZZZ' and ""DimCode""=1 AND  IFNULL(""U_EXX_MAX_ID"",'') <>''

                                UNION ALL

                                SELECT Top 1
                                ""PrcCode"" as ""codeSAP"",
                                ""PrcName"" as  ""comptext"",
                                ""PrcCode"" as ""compvalue"",
                                3 as ""glorder"",
                                IFNULL(""U_EXX_MAX_ID"",'') as ""idMaximo"",
                                ""Active""
                                FROM
                                ""OPRC""
                                WHERE
                                ""U_EXX_MAX_STD"" = 'P' and ""DimCode""=2  AND  IFNULL(""U_EXX_MAX_ID"",'') <>''

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    GLComponentClass doc = new GLComponentClass();
                    doc.active = recordset.Fields.Item("active").Value.ToString() == "Y" ? true:false ;
                    doc.comptext = (string)recordset.Fields.Item("comptext").Value;
                    doc.codeSAP = (string)recordset.Fields.Item("codeSAP").Value;
                    doc.compvalue = (string)recordset.Fields.Item("compvalue").Value;
                    doc.glorder = int.Parse(recordset.Fields.Item("glorder").Value.ToString());
                    doc.orgid = "TPE";//(string)recordset.Fields.Item("TableName").Value;
                    doc.sourcesysid = "SAPBO";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.userid = "SAPBO"; //(string)recordset.Fields.Item("U_EXX_STA_SIP").Value;
                    doc.idMaximo = (string)recordset.Fields.Item("idMaximo").Value; ;

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
