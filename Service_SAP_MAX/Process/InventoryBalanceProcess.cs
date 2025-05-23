using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SAPbobsCOM;
using Service_SAP_MAX.Entities.Response;
using Service_SAP_MAX.Entities;
using Service_SAP_MAX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Process
{
    public class InventoryBalanceProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PurchaseOrderProcess));
        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listSN = GetBalances(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_BALANCE).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listSN)
                {
                    //if (string.IsNullOrEmpty(item.idMaximo))
                    //    SendOrder(oCompany, item, listConfig, url, maxAuth, authorization);
                    ////else
                       UpdateBalance(oCompany, item, listConfig, url, maxAuth, authorization);

                }
                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void SendOrder(Company oCompany, BalanceClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {


                string body = JsonConvert.SerializeObject(item);
                var properties = "*";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendOrder: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado", resp.companiesid.ToString());
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateBalance(oCompany, item, listConfig);
                        logger.Error("SendOrder " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateState(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendOrder " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateState(item, oCompany, "E", msg);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateBalance(Company oCompany, BalanceClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
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
                //settings.ContractResolver = new CustomResolver(new[] { "currencycode", "description", "ponum", "potype", "purchaseagent", "revisionnum", "priority", "sendersysid",
                //"shipto","billto","vendor","poline"});

                var jsonBody = JsonConvert.SerializeObject(item);

                var properties = "*";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, jsonBody, properties, "PATCH", "MERGE");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("UpdateBalance: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateBalance(oCompany, item, listConfig);
                        logger.Error("UpdateBalance " + response.Content);
                        UpdateState(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("UpdateBalance " + response.Content);
                        UpdateState(item, oCompany, "E", resp.Error.message);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateState(BalanceClass item, Company oCompany, string state, string message, string id = "")
        {
            Documents oOrders = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);
            try
            {

                if (!oOrders.GetByKey(int.Parse(item.CodeSAP)))
                {
                    logger.Error("oOrders no encontrado.");

                }

                oOrders.UserFields.Fields.Item(Constants.U_EXX_MAX_STD).Value = state;
                oOrders.UserFields.Fields.Item(Constants.U_EXX_MAX_MSJ).Value = message;

                if (!string.IsNullOrEmpty(id))
                    oOrders.UserFields.Fields.Item(Constants.U_EXX_MAX_ID).Value = id;

                int result = oOrders.Update();
                if (result != 0)
                {
                    logger.Error(" Error al actualizar Order: " + item.CodeSAP + " : " + oCompany.GetLastErrorDescription());
                }
                else
                {
                    logger.Info("Order actualizado " + item.CodeSAP);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                Marshal.ReleaseComObject(oOrders);
                if (oOrders != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static List<BalanceClass> GetBalances(Company oCompany, List<ConfigClass> listConfig)
        {
            List<BalanceClass> list = new List<BalanceClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT TOP 10
OC.""DocEntry"",
'47205' as ""U_EXX_MAX_ID"",
'TPE' as ""orgid"",
'TP01' as ""siteid"",
'A-002' as ""binnum"",
'30' as ""curbal"",
--OC1.""ItemCode"" 
'AXE0000045' as ""itemnum"",
'ITEMSET' as ""itemsetid"",
--""WhsCode"" 
'004' as ""location"",
'SAP' as ""sendersysid"",
IFNULL(IT.""U_EXX_MAX_ID"",'') as ""IdMaximo""
FROM OIGN OC
JOIN IGN1 OC1 ON OC1.""DocEntry""=OC.""DocEntry""
INNER JOIN OITM IT ON IT.""ItemCode""=OC1.""ItemCode""

WHERE
OC.""U_EXX_MAX_STD""='P' AND
""CANCELED""='N' --AND IFNULL(IT.""U_EXX_MAX_ID"",'')<>''
ORDER BY ""DocEntry""

";
                recordset.DoQuery(query);
                
                
                var _docEntry = "";
                int cont = 1;
                while (!recordset.EoF)
                {
                    BalanceClass doc = new BalanceClass();

                    doc.CodeSAP = recordset.Fields.Item("DocEntry").Value.ToString();
                 
                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value;
                    doc.orgid = (string)recordset.Fields.Item("orgid").Value;
                    doc.siteid = (string)recordset.Fields.Item("siteid").Value;                  
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value;
                    doc.binnum = (string)recordset.Fields.Item("binnum").Value;
                    doc.curbal = (string)recordset.Fields.Item("curbal").Value;
                    doc.itemnum = (string)recordset.Fields.Item("itemnum").Value;
                    doc.itemsetid = (string)recordset.Fields.Item("itemsetid").Value;
                    doc.location = (string)recordset.Fields.Item("location").Value;
                                    

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
