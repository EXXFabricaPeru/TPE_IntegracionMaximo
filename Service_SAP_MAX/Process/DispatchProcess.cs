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
    public class DispatchProcess
    {


        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DispatchProcess));
        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listSN = GetDispatchs(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_DELIVERY).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listSN)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendDispatchs(oCompany, item, listConfig, url, maxAuth, authorization);
                    //else
                    //    UpdateOrder(oCompany, item, listConfig, url, maxAuth, authorization);

                }
                listSN = GetDispatchsReturn(oCompany, listConfig);

                //foreach (var item in listSN)
                //{
                //    //if (string.IsNullOrEmpty(item.idMaximo))
                //    //    SendDispatchs(oCompany, item, listConfig, url, maxAuth, authorization);
                //    ////else
                //    ////    
                //    UpdateOrder(oCompany, item, listConfig, url, maxAuth, authorization);

                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void SendDispatchs(Company oCompany, DispatchClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {


                string body = JsonConvert.SerializeObject(item);
                var properties = "*";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    //var resp = JsonConvert.DeserializeObject<DispatchResponseClass>(response.Content);
                    logger.Info("SendDispatchs: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado", "");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateOrder(oCompany, item, listConfig);
                        logger.Error("SendDispatchs " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateState(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendDispatchs " + response.Content);
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

        private static void UpdateOrder(Company oCompany, DispatchClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
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
                    logger.Info("SendDispatchs: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateOrder(oCompany, item, listConfig);
                        logger.Error("SendDispatchs " + response.Content);
                        UpdateState(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("SendDispatchs " + response.Content);
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

        private static void UpdateState(DispatchClass item, Company oCompany, string state, string message, string id = "")
        {
            Documents oOrders = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInventoryGenExit);
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

        private static List<DispatchClass> GetDispatchs(Company oCompany, List<ConfigClass> listConfig)
        {
            List<DispatchClass> list = new List<DispatchClass>();
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
                                   
                                    'TP00955' as ""ponum"",
                                    'N' as ""potype"",
                                    'CESAR.RUIZ' as ""purchaseagent"",
                                    '0' as ""revisionnum"",
                                    '0' as ""priority"",
                                    'SAPBO' as ""sendersysid"",
                                    '01' as ""shipto"",
                                    'DESP' as ""usetype"",
                                    'COMPLETO' as ""status"",
                                    'CONSUMO' as ""itc_tiptrans"",
                                    --LINE
                                     OC1.""U_EXX_MAX_LINE""  as ""invuselinenum"",
                                    CASE WHEN IT.""InvntItem""='Y' THEN 'ARTÍCULO' ELSE 'SERV.EST' END as ""linetype"",
                                    OC1.""ItemCode"" 
                                     as ""itemnum"",
                                    'ITEMSET' as ""itemsetid"",
                                    --OC1.""WhsCode"" 
                                '004' as ""fromstoreloc"",
                                    
                                    OC1.""Quantity"" as ""quantity"",
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
                                    OC1.""Text""as ""remark"",
                                    '' as ""requestnum""
                                    FROM OIGE OC
                                    JOIN IGE1 OC1 ON OC1.""DocEntry""=OC.""DocEntry""
                                    INNER JOIN OITM IT ON IT.""ItemCode""=OC1.""ItemCode""
                                    INNER JOIN OACT AC ON AC.""AcctCode""=OC1.""AcctCode""
                                    WHERE
                                    OC.""U_EXX_MAX_STD""='P' AND
                                    ""CANCELED""='N' AND ""DocNum""=10000013
                                    ORDER BY ""DocEntry""

";
                recordset.DoQuery(query);
                DispatchClass doc = new DispatchClass();

                var _docEntry = "";
                int cont = 1;
                while (!recordset.EoF)
                {


                    doc.CodeSAP = recordset.Fields.Item("DocEntry").Value.ToString();

                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value.ToString();

                    doc.description = (string)recordset.Fields.Item("description").Value.ToString();
                    doc.fromstoreloc = (string)recordset.Fields.Item("fromstoreloc").Value.ToString();
                    doc.orgid = (string)recordset.Fields.Item("orgid").Value.ToString();
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value.ToString();
                    doc.siteid = (string)recordset.Fields.Item("siteid").Value.ToString();
                    doc.status = (string)recordset.Fields.Item("status").Value.ToString();
                    doc.usetype = (string)recordset.Fields.Item("usetype").Value.ToString();
                    doc.itc_tiptrans = (string)recordset.Fields.Item("itc_tiptrans").Value.ToString();

                    doc.invuseline = new List<DispatchClass.Invuseline>();

                    DispatchClass.Invuseline invuseline = new DispatchClass.Invuseline();
                    invuseline.requestnum = (string)recordset.Fields.Item("requestnum").Value.ToString();
                    invuseline.actualdate = (DateTime)recordset.Fields.Item("actualdate").Value;
                    invuseline.conversion = (string)recordset.Fields.Item("conversion").Value.ToString();
                    invuseline.description = (string)recordset.Fields.Item("description").Value.ToString();
                    invuseline.enterby = (string)recordset.Fields.Item("enterby").Value.ToString();
                    invuseline.frombin = "";// (string)recordset.Fields.Item("itc_tiptrans").Value.ToString();
                    invuseline.fromstoreloc = (string)recordset.Fields.Item("fromstoreloc").Value.ToString();
                    invuseline.gldebitacct = (string)recordset.Fields.Item("gldebitacct").Value.ToString();
                    invuseline.invuselinenum = (string)recordset.Fields.Item("invuselinenum").Value.ToString();
                    invuseline.itemnum = (string)recordset.Fields.Item("itemnum").Value.ToString();
                    invuseline.itemsetid = (string)recordset.Fields.Item("itemsetid").Value.ToString();
                    invuseline.linetype = (string)recordset.Fields.Item("linetype").Value.ToString();
                    invuseline.orgid = (string)recordset.Fields.Item("orgid").Value.ToString();
                    invuseline.siteid = (string)recordset.Fields.Item("siteid").Value.ToString();
                    invuseline.quantity = (string)recordset.Fields.Item("quantity").Value.ToString();
                    invuseline.refwo = (string)recordset.Fields.Item("refwo").Value.ToString();
                    invuseline.remark = (string)recordset.Fields.Item("remark").Value.ToString();
                    invuseline.sendersysid = (string)recordset.Fields.Item("sendersysid").Value.ToString();
                    invuseline.usetype = (string)recordset.Fields.Item("usetype").Value.ToString();
                    doc.invuseline.Add(invuseline);

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


        private static List<DispatchClass> GetDispatchsReturn(Company oCompany, List<ConfigClass> listConfig)
        {
            List<DispatchClass> list = new List<DispatchClass>();
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
                                   
                                    'TP00955' as ""ponum"",
                                    'N' as ""potype"",
                                    'CESAR.RUIZ' as ""purchaseagent"",
                                    '0' as ""revisionnum"",
                                    '0' as ""priority"",
                                    'SAPBO' as ""sendersysid"",
                                    '01' as ""shipto"",
                                    'DESP' as ""usetype"",
                                    'COMPLETO' as ""status"",
                                    'CONSUMO' as ""itc_tiptrans"",
                                    --LINE
                                     OC1.""U_EXX_MAX_LINE""  as ""invuselinenum"",
                                    CASE WHEN IT.""InvntItem""='Y' THEN 'ARTÍCULO' ELSE 'SERV.EST' END as ""linetype"",
                                    OC1.""ItemCode"" 
                                     as ""itemnum"",
                                    'ITEMSET' as ""itemsetid"",
                                    --OC1.""WhsCode"" 
                                '004' as ""fromstoreloc"",
                                    
                                    OC1.""Quantity"" as ""quantity"",
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
                                    OC1.""Text""as ""remark"",
                                    '' as ""requestnum""
                                    FROM OIGE OC
                                    JOIN IGE1 OC1 ON OC1.""DocEntry""=OC.""DocEntry""
                                    INNER JOIN OITM IT ON IT.""ItemCode""=OC1.""ItemCode""
                                    INNER JOIN OACT AC ON AC.""AcctCode""=OC1.""AcctCode""
                                    WHERE
                                    OC.""U_EXX_MAX_STD""='P' AND
                                    ""CANCELED""='N' AND ""DocNum""=10000013
                                    ORDER BY ""DocEntry""

";
                recordset.DoQuery(query);
                DispatchClass doc = new DispatchClass();

                var _docEntry = "";
                int cont = 1;
                while (!recordset.EoF)
                {


                    doc.CodeSAP = recordset.Fields.Item("DocEntry").Value.ToString();

                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value.ToString();

                    doc.description = (string)recordset.Fields.Item("description").Value.ToString();
                    doc.fromstoreloc = (string)recordset.Fields.Item("fromstoreloc").Value.ToString();
                    doc.orgid = (string)recordset.Fields.Item("orgid").Value.ToString();
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value.ToString();
                    doc.siteid = (string)recordset.Fields.Item("siteid").Value.ToString();
                    doc.status = (string)recordset.Fields.Item("status").Value.ToString();
                    doc.usetype = (string)recordset.Fields.Item("usetype").Value.ToString();
                    doc.itc_tiptrans = (string)recordset.Fields.Item("itc_tiptrans").Value.ToString();

                    doc.invuseline = new List<DispatchClass.Invuseline>();

                    DispatchClass.Invuseline invuseline = new DispatchClass.Invuseline();
                    invuseline.requestnum = (string)recordset.Fields.Item("requestnum").Value.ToString();
                    invuseline.actualdate = (DateTime)recordset.Fields.Item("actualdate").Value;
                    invuseline.conversion = (string)recordset.Fields.Item("conversion").Value.ToString();
                    invuseline.description = (string)recordset.Fields.Item("description").Value.ToString();
                    invuseline.enterby = (string)recordset.Fields.Item("enterby").Value.ToString();
                    invuseline.frombin = "";// (string)recordset.Fields.Item("itc_tiptrans").Value.ToString();
                    invuseline.fromstoreloc = (string)recordset.Fields.Item("fromstoreloc").Value.ToString();
                    invuseline.gldebitacct = (string)recordset.Fields.Item("gldebitacct").Value.ToString();
                    invuseline.invuselinenum = (string)recordset.Fields.Item("invuselinenum").Value.ToString();
                    invuseline.itemnum = (string)recordset.Fields.Item("itemnum").Value.ToString();
                    invuseline.itemsetid = (string)recordset.Fields.Item("itemsetid").Value.ToString();
                    invuseline.linetype = (string)recordset.Fields.Item("linetype").Value.ToString();
                    invuseline.orgid = (string)recordset.Fields.Item("orgid").Value.ToString();
                    invuseline.siteid = (string)recordset.Fields.Item("siteid").Value.ToString();
                    invuseline.quantity = (string)recordset.Fields.Item("quantity").Value.ToString();
                    invuseline.refwo = (string)recordset.Fields.Item("refwo").Value.ToString();
                    invuseline.remark = (string)recordset.Fields.Item("remark").Value.ToString();
                    invuseline.sendersysid = (string)recordset.Fields.Item("sendersysid").Value.ToString();
                    invuseline.usetype = "DEVOLVER";// (string)recordset.Fields.Item("usetype").Value.ToString();

                    doc.invuseline.Add(invuseline);
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

    }
}
