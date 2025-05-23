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
    public class ReceiptsReturnsProcess
    {

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ReceiptsReturnsProcess));
        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listSN = GetReceptions(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_RECEIPT).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listSN)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendReceptions(oCompany, item, listConfig, url, maxAuth, authorization);
                    //else
                    //    UpdateOrder(oCompany, item, listConfig, url, maxAuth, authorization);

                }
                listSN = GetReceptionsCanceled(oCompany, listConfig);

                foreach (var item in listSN)
                {
                    //if (string.IsNullOrEmpty(item.idMaximo))
                    //    SendReceptions(oCompany, item, listConfig, url, maxAuth, authorization);
                    ////else
                    ////    
                    UpdateOrder(oCompany, item, listConfig, url, maxAuth, authorization);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void SendReceptions(Company oCompany, ReceiptClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {


                string body = JsonConvert.SerializeObject(item);
                var properties = "*";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<ReceiptResponseClass>(response.Content);
                    logger.Info("SendReceptions: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado","");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateOrder(oCompany, item, listConfig);
                        logger.Error("SendReceptions " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateState(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendReceptions " + response.Content);
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

        private static void UpdateOrder(Company oCompany, ReceiptClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
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
                settings.ContractResolver = new CustomResolver(new[] { "currencycode", "description", "ponum", "potype", "purchaseagent", "revisionnum", "priority", "sendersysid",
                "shipto","billto","vendor","poline"});

                var jsonBody = JsonConvert.SerializeObject(item);

                var properties = "*";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, jsonBody, properties, "PATCH", "MERGE");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendReceptions: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateOrder(oCompany, item, listConfig);
                        logger.Error("SendReceptions " + response.Content);
                        UpdateState(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("SendReceptions " + response.Content);
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

        private static void UpdateState(ReceiptClass item, Company oCompany, string state, string message, string id = "")
        {
            Documents oOrders = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oPurchaseOrders);
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

        private static List<ReceiptClass> GetReceptions(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ReceiptClass> list = new List<ReceiptClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT
                                    OC.""DocEntry"",
                                    OC.""U_EXX_MAX_ID"",
                            OC.""DocDate"" as ""actualdate"",
                            'RAUL.MORAN' as ""enterby"",
                            CURRENT_DATE as   ""enterdate"",
                            '1' as ""conversion"" ,
                            'SAPBO' as    ""externalrefid"",
                            'RECIBO' as ""issuetype"",
                                    'TPE' as ""orgid"",
                                    'TP01' as ""siteid"",
                                    ""Currency"" as ""currencycode"",
                                    IFNULL(""Comments"",'') as ""description"",
                                    'TP00955' as ""ponum"",
                                    'N' as ""potype"",
                                    'CESAR.RUIZ' as ""purchaseagent"",
                                    '0' as ""revisionnum"",
                                    '0' as ""priority"",
                                    'SAP' as ""sendersysid"",
                                    '01' as ""shipto"",
                                    '01' as ""billto"",
                                    'APROB' as ""status"",
                                    OC.""LicTradNum"" as ""vendor"",
                                    --LINE
                                     OC1.""U_EXX_MAX_LINE""  as ""polinenum"",
                                    CASE WHEN IT.""InvntItem""='Y' THEN 'ARTÍCULO' ELSE 'SERV.EST' END as ""linetype"",
                                    OC1.""ItemCode"" 
                                     as ""itemnum"",
                                    'ITEMSET' as ""itemsetid"",
                                    --OC1.""WhsCode"" 
                                '004' as ""storeloc"",
                                    '1' as ""conversion"",
                                    OC1.""Quantity"" as ""orderqty"",
                                    'OT56959' as ""ref_prnum"",
                                      OC1.""U_EXX_MAX_LINE"" as ""ref_prlinenum"",
                                   OC1.""U_EXX_MAX_OT"" as ""refwo"",
                                    OC1.""Price"" as ""unitcost"",
                                    CASE WHEN OC1.""unitMsr""='NIU' THEN 'UNI' ELSE OC1.""unitMsr"" END as ""orderunit"",
                                    ""Dscription"" as ""description"",
                                    IFNULL('01-'||SUBSTRING(AC.""FormatCode"",0,8)||'-'||OC1.""OcrCode""||'-'||OC1.""OcrCode2"",'') as ""gldebitacct"",
                                    '' as ""requestedby"",
                                    CASE WHEN OC1.""TaxCode"" = 'IGV' THEN '18.00' ELSE '0' END  as ""tax1"",
                                    CASE WHEN OC1.""TaxCode"" = 'IGV' THEN 'IGV18' ELSE OC1.""TaxCode"" END as ""tax1code"",
                                    OC1.""Text""as ""remark""
                                    FROM OPDN OC
                                    JOIN PDN1 OC1 ON OC1.""DocEntry""=OC.""DocEntry""
                                    INNER JOIN OITM IT ON IT.""ItemCode""=OC1.""ItemCode""
                                    INNER JOIN OACT AC ON AC.""AcctCode""=OC1.""AcctCode""
                                    WHERE
                                    OC.""U_EXX_MAX_STD""='P' AND
                                    ""CANCELED""='N' AND ""DocNum""=10000050
                                    ORDER BY ""DocEntry""

";
                recordset.DoQuery(query);
                ReceiptClass doc = new ReceiptClass();
                
                var _docEntry = "";
                int cont = 1;
                while (!recordset.EoF)
                {


                    doc.CodeSAP = recordset.Fields.Item("DocEntry").Value.ToString();
                                        
                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value.ToString();

                    doc.actualdate = (DateTime)recordset.Fields.Item("actualdate").Value;
                    doc.conversion = (string)recordset.Fields.Item("conversion").Value.ToString();
                    doc.enterby = (string)recordset.Fields.Item("enterby").Value.ToString();
                    doc.enterdate = (DateTime)recordset.Fields.Item("enterdate").Value;
                    doc.externalrefid = (string)recordset.Fields.Item("externalrefid").Value.ToString();
                    doc.issuetype = (string)recordset.Fields.Item("issuetype").Value.ToString();
                    doc.itemnum = (string)recordset.Fields.Item("itemnum").Value.ToString();
                    doc.itemsetid = (string)recordset.Fields.Item("itemsetid").Value.ToString();
                    doc.linetype = (string)recordset.Fields.Item("linetype").Value.ToString();
                    doc.orderqty = (string)recordset.Fields.Item("orderqty").Value.ToString();
                    doc.receiptquantity = (string)recordset.Fields.Item("orderqty").Value.ToString();
                    doc.qtytoreceive = (string)recordset.Fields.Item("orderqty").Value.ToString();
                    doc.orgid = (string)recordset.Fields.Item("orgid").Value.ToString();
                    doc.ponum = (string)recordset.Fields.Item("ponum").Value.ToString();
                    doc.polinenum = (string)recordset.Fields.Item("polinenum").Value.ToString();
                    doc.porevisionnum = "0";// (string)recordset.Fields.Item("porevisionnum").Value.ToString();
                    doc.positeid = "TP01";// (string)recordset.Fields.Item("positeid").Value.ToString();
                    doc.refwo = (string)recordset.Fields.Item("refwo").Value.ToString();
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value.ToString();
                    doc.sourcesysid = (string)recordset.Fields.Item("sendersysid").Value.ToString();
                    doc.siteid = (string)recordset.Fields.Item("siteid").Value.ToString();
                    doc.status = (string)recordset.Fields.Item("status").Value.ToString();
                    //doc.tolot = (string)recordset.Fields.Item("tolot").Value.ToString();


                    list.Add(doc);
                    cont++;
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


        private static List<ReceiptClass> GetReceptionsCanceled(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ReceiptClass> list = new List<ReceiptClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT
                                    OC.""DocEntry"",
                                    IFNULL(OC.""U_EXX_MAX_ID"",''),
                                    'TPE' as ""orgid"",
                                    'TP01' as ""siteid"",
                                    'CAN' as ""status""

                                    FROM OPOR OC

                                    WHERE
                                    OC.""U_EXX_MAX_STD""='P' AND
                                    IFNULL(OC.""U_EXX_MAX_ID"",'')<>'' AND
                                    ""CANCELED""='Y'
                                    ORDER BY ""DocEntry""

";
                recordset.DoQuery(query);


                var _docEntry = "";
                int cont = 1;
                while (!recordset.EoF)
                {

                    ReceiptClass doc = new ReceiptClass();
                    doc.CodeSAP = (string)recordset.Fields.Item("DocEntry").Value;

                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value;
                    doc.orgid = (string)recordset.Fields.Item("orgid").Value;
                    doc.siteid = (string)recordset.Fields.Item("siteid").Value;
                    doc.status = (string)recordset.Fields.Item("status").Value;


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
