using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SAPbobsCOM;
using Service_SAP_MAX.Entities;
using Service_SAP_MAX.Entities.Response;
using Service_SAP_MAX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service_SAP_MAX.Process
{
    public class BusinessPartnerProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(BusinessPartnerProcess));
        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listSN = GetVendors(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_COMPANIES).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listSN)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendCompany(oCompany, item, listConfig, url, maxAuth, authorization);
                    else
                        UpdateCompany(oCompany, item, listConfig,url,maxAuth,authorization);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void SendCompany(Company oCompany, CompaniesClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {
               

                string body = JsonConvert.SerializeObject(item);
                var properties = "address1, company, currencycode, disabled, externalrefid, inclusive1, name, orgid, paymentterms, tax1code, type, sendersysid, companiesid";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post,maxAuth,authorization , body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendCompany: Respuesta " + response.Content);
                    UpdateBP(item, oCompany, "S", "Enviado",resp.companiesid.ToString());
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateCompany(oCompany, item, listConfig);
                        logger.Error("SendCompany " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateBP(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendCompany " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateBP(item, oCompany, "E", msg);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateCompany(Company oCompany, CompaniesClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {
                url = url.Replace("?lean=1", "");
                url = url + item.idMaximo + "?lean=1";

                //var settings = new JsonSerializerSettings
                //{
                //    ContractResolver = new DefaultContractResolver
                //    {
                //        NamingStrategy = new CamelCaseNamingStrategy(),
                //        IgnoreSerializableAttribute = true
                //    }
                //};
                //settings.ContractResolver = new CustomResolver(new[] { "glaccount", "activedate", "glcomp01", "glcomp02", "glcomp03", "glcomp04" });

                var jsonBody = JsonConvert.SerializeObject(item);

                var properties = "address1, company, currencycode, disabled, externalrefid, inclusive1, name, orgid, paymentterms, tax1code, type, sendersysid, companiesid";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth,authorization, jsonBody,properties,"PATCH","MERGE");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendCompany: Respuesta " + response.Content);
                    UpdateBP(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateCompany(oCompany, item, listConfig);
                        logger.Error("SendCompany " + response.Content);
                        UpdateBP(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("SendCompany " + response.Content);
                        UpdateBP(item, oCompany, "E", resp.Error.message);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateBP(CompaniesClass item, Company oCompany, string state, string message,string id="")
        {
            BusinessPartners obusinessPartners = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            try
            {

                if (!obusinessPartners.GetByKey(item.CardCode))
                {
                    logger.Error("obusinessPartners no encontrado.");

                }

                obusinessPartners.UserFields.Fields.Item(Constants.U_EXX_MAX_STD).Value = state;
                obusinessPartners.UserFields.Fields.Item(Constants.U_EXX_MAX_MSJ).Value = message;

                if(!string.IsNullOrEmpty(id))
                    obusinessPartners.UserFields.Fields.Item(Constants.U_EXX_MAX_ID).Value = id;

                int result = obusinessPartners.Update();
                if (result != 0)
                {
                    logger.Error(" Error al actualizar BP: " + item.CardCode + " : " + oCompany.GetLastErrorDescription());
                }
                else
                {
                    logger.Info("BP actualizado " + item.CardCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                Marshal.ReleaseComObject(obusinessPartners);
                if (obusinessPartners != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static List<CompaniesClass> GetVendors(Company oCompany, List<ConfigClass> listConfig)
        {
            List<CompaniesClass> list = new List<CompaniesClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT TOP 100 ""CardCode"", ""LicTradNum"", ""CardName"", 'N' as ""Type"",""Currency"",
                                    CT.""PymntGroup"",""Address"" ,""{Constants.U_EXX_MAX_ID}""
                                    FROM OCRD CR
                                    JOIN OCTG CT ON CT.""GroupNum""=  CR.""GroupNum""
                                    WHERE ""{Constants.U_EXX_MAX_STD}""='P' AND ""CardType""='S'  AND ""CardCode""= 'P20600346149'

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    CompaniesClass doc = new CompaniesClass();
                    doc.CardCode = (string)recordset.Fields.Item("CardCode").Value;
                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value;
                    doc.company = (string)recordset.Fields.Item("LicTradNum").Value;
                    doc.name = (string)recordset.Fields.Item("CardName").Value;
                    doc.type = (string)recordset.Fields.Item("Type").Value;
                    doc.orgid = "TPE";
                    doc.currencycode = (string)recordset.Fields.Item("Currency").Value;
                    doc.tax1code = "IGV18";
                    doc.paymentterms = (string)recordset.Fields.Item("PymntGroup").Value;
                    doc.disabled = "0";
                    doc.inclusive1 = "1";
                    doc.sendersysid = "SAPBO";
                    doc.address1 = (string)recordset.Fields.Item("Address").Value;

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
