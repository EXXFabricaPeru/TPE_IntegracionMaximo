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
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Process
{
    public class ItemsProcess
    {

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ItemsProcess));
        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listSN = GetItems(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_ITEM).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listSN)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendItem(oCompany, item, listConfig, url, maxAuth, authorization);
                    else
                        UpdateItem(oCompany, item, listConfig, url, maxAuth, authorization);

                }

                var listServ = GetServices(oCompany, listConfig);
                url = listConfig.Where(t => t.Code == Constants.URL_SERVICE).FirstOrDefault().Value;

                foreach (var item in listServ)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendItem(oCompany, item, listConfig, url, maxAuth, authorization);
                    else
                        UpdateItem(oCompany, item, listConfig, url, maxAuth, authorization);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }



        private static void SendItem(Company oCompany, ItemClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {


                string body = JsonConvert.SerializeObject(item);
                var properties = "commodity, commoditygroup, description, issueunit, itemnum, itemsetid, itemtype, lottype, orderunit, rotating, sendersysid, status, itemid";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<ItemResponseClass>(response.Content);
                    logger.Info("SendItem: Respuesta " + response.Content);
                    UpdateItemSAP(item, oCompany, "S", "Enviado", resp.itemid.ToString());
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateItem(oCompany, item, listConfig);
                        logger.Error("SendItem " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateItemSAP(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendItem " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateItemSAP(item, oCompany, "E", msg);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateItem(Company oCompany, ItemClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
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
                settings.ContractResolver = new CustomResolver(new[] { "itemnum", "itemsetid", "itemtype", "lottype", "rotating", "sendersysid" });

                var jsonBody = JsonConvert.SerializeObject(item,settings);

                var properties = "commodity, commoditygroup, description, issueunit, itemnum, itemsetid, itemtype, lottype, orderunit, rotating, sendersysid, status";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, jsonBody, properties, "PATCH", "MERGE");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendItem: Respuesta " + response.Content);
                    UpdateItemSAP(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateItem(oCompany, item, listConfig);
                        logger.Error("SendItem " + response.Content);
                        UpdateItemSAP(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("SendItem " + response.Content);
                        UpdateItemSAP(item, oCompany, "E", resp.Error.message);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateItemSAP(ItemClass item, Company oCompany, string state, string message, string id = "")
        {
            Items oItems = (Items)oCompany.GetBusinessObject(BoObjectTypes.oItems);
            try
            {

                if (!oItems.GetByKey(item.ItemCode))
                {
                    logger.Error("oItems no encontrado.");

                }

                oItems.UserFields.Fields.Item(Constants.U_EXX_MAX_STD).Value = state;
                oItems.UserFields.Fields.Item(Constants.U_EXX_MAX_MSJ).Value = message;

                if (!string.IsNullOrEmpty(id))
                    oItems.UserFields.Fields.Item(Constants.U_EXX_MAX_ID).Value = id;

                int result = oItems.Update();
                if (result != 0)
                {
                    logger.Error(" Error al actualizar Item: " + item.ItemCode + " : " + oCompany.GetLastErrorDescription());
                }
                else
                {
                    logger.Info("Item actualizado " + item.ItemCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                Marshal.ReleaseComObject(oItems);
                if (oItems != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static List<ItemClass> GetItems(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ItemClass> list = new List<ItemClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT TOP 100 
                                    ""ItemCode"",
                                    ""U_EXX_MAX_ID"",
                                    'S006001' as ""commodity"",
                                    'F006' as ""commoditygroup"",
                                    IFNULL(""ItmsGrpCod"",0) as ""itc_tipo"",
                                    '01' as ""itc_rubro"",
                                    ""ItemName"" as ""description"",
                                    CASE WHEN IFNULL(""SalUnitMsr"",'')='NIU' THEN 'UNI' ELSE IFNULL(""SalUnitMsr"",'') END as ""issueunit"",
                                    ""ItemCode"" as ""itemnum"",
                                    'ITEMSET' as ""itemsetid"",
                                    'ARTÍCULO' as ""itemtype"",
                                    CASE WHEN ""ManBtchNum"" ='N' THEN 'SINLOT' ELSE 'LOT' END   as ""lottype"",
                                    CASE WHEN IFNULL(""BuyUnitMsr"",'')='NIU' THEN 'UNI' ELSE IFNULL(""BuyUnitMsr"",'') END as ""orderunit"",
                                    0 as ""rotating"",
                                    'SAP-BO' as ""sendersysid"",
                                    'ACTIVO' as ""status""
                                    FROM OITM CR
                                   
                                    WHERE 
                                    ""{Constants.U_EXX_MAX_STD}""='P' 
                                    AND ""InvntItem""='Y' AND ""ItemCode""='111000004'
                                    

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    ItemClass doc = new ItemClass();
                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value;
                    doc.ItemCode = (string)recordset.Fields.Item("ItemCode").Value;
                    doc.commodity = (string)recordset.Fields.Item("commodity").Value;
                    doc.commoditygroup = (string)recordset.Fields.Item("commoditygroup").Value;
                    doc.itc_tipo = "101"; recordset.Fields.Item("itc_tipo").Value.ToString();
                    doc.itc_rubro = (string)recordset.Fields.Item("itc_rubro").Value;
                    doc.description = (string)recordset.Fields.Item("description").Value;
                    doc.issueunit = (string)recordset.Fields.Item("issueunit").Value;
                    doc.itemnum = (string)recordset.Fields.Item("itemnum").Value;
                    doc.itemsetid = (string)recordset.Fields.Item("itemsetid").Value;
                    doc.itemtype = (string)recordset.Fields.Item("itemtype").Value;
                    doc.lottype = (string)recordset.Fields.Item("lottype").Value;
                    doc.orderunit = (string)recordset.Fields.Item("orderunit").Value;
                    doc.rotating = (int)recordset.Fields.Item("rotating").Value;
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value;
                    doc.status = (string)recordset.Fields.Item("status").Value;

                    Itemorginfo info = new Itemorginfo();
                    info.orgid = "TPE";
                    info.status = "ACTIVO";
                    info.category = "PTE";

                    doc.itemorginfo = new List<Itemorginfo>();
                    doc.itemorginfo.Add(info);

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

        private static List<ServiceClass> GetServices(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ServiceClass> list = new List<ServiceClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT TOP 100 
                                    ""ItemCode"",
                                    ""U_EXX_MAX_ID"",
                                    'S001' as ""commodity"",
                                    'S' as ""commoditygroup"",
                                    ""ItemName"" as ""description"",
                                    CASE WHEN IFNULL(""SalUnitMsr"",'')='NIU' THEN 'UNI' ELSE IFNULL(""SalUnitMsr"",'') END as ""issueunit"",
                                    ""ItemCode"" as ""itemnum"",
                                    'ITEMSET' as ""itemsetid"",
                                    'SERV.EST' as ""itemtype"",
                                    CASE WHEN ""ManBtchNum"" ='N' THEN 'SINLOT' ELSE 'LOT' END   as ""lottype"",
                                    CASE WHEN IFNULL(""BuyUnitMsr"",'')='NIU' THEN 'UNI' ELSE IFNULL(""BuyUnitMsr"",'') END as ""orderunit"",
                                    'SAP-BO' as ""sendersysid"",
                                    'ACTIVO' as ""status"",
                                    0 as ""inspectionrequired"",
                                    ""U_EXX_MAX_GLA""
                                    FROM OITM CR
                                   
                                    WHERE 
                                    ""{Constants.U_EXX_MAX_STD}""='P' 
                                    AND ""InvntItem""='N' AND ""ItemCode""='716101001'
                                    

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    ServiceClass doc = new ServiceClass();
                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID).Value;
                    doc.ItemCode = (string)recordset.Fields.Item("ItemCode").Value;
                    doc.commodity = (string)recordset.Fields.Item("commodity").Value;
                    doc.commoditygroup = (string)recordset.Fields.Item("commoditygroup").Value;
                    doc.description = (string)recordset.Fields.Item("description").Value;
                    doc.itemnum = (string)recordset.Fields.Item("itemnum").Value;
                    doc.itemsetid = (string)recordset.Fields.Item("itemsetid").Value;
                    doc.itemtype = (string)recordset.Fields.Item("itemtype").Value;
                    doc.lottype = (string)recordset.Fields.Item("lottype").Value;
                    doc.orderunit = (string)recordset.Fields.Item("orderunit").Value;
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value;
                    doc.status = (string)recordset.Fields.Item("status").Value;
                    doc.inspectionrequired = (int)recordset.Fields.Item("inspectionrequired").Value;
                    doc.taxexempt = 0;

                    ItemorginfoService info = new ItemorginfoService();
                    info.orgid = "TPE";
                    info.status = "ACTIVO";
                    info.category = "NE";
                    info.glaccount = (string)recordset.Fields.Item("U_EXX_MAX_GLA").Value; ;

                    doc.itemorginfo = new List<ItemorginfoService>();
                    doc.itemorginfo.Add(info);

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


        private static void SendItem(Company oCompany, ServiceClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {


                string body = JsonConvert.SerializeObject(item);
                var properties = "commodity, commoditygroup, description, itemnum, itemsetid, itemtype, lottype, orderunit, inspectionrequired, sendersysid, status, itemid";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<ItemResponseClass>(response.Content);
                    logger.Info("SendItem: Respuesta " + response.Content);
                    UpdateItemSAP(item, oCompany, "S", "Enviado", resp.itemid.ToString());
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateItem(oCompany, item, listConfig);
                        logger.Error("SendItem " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateItemSAP(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendItem " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateItemSAP(item, oCompany, "E", msg);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateItem(Company oCompany, ServiceClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
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
                settings.ContractResolver = new CustomResolver(new[] { "itemnum", "itemsetid", "itemtype", "lottype", "rotating", "sendersysid" });

                var jsonBody = JsonConvert.SerializeObject(item, settings);

                var properties = "commodity, commoditygroup, description, issueunit, itemnum, itemsetid, itemtype, lottype, orderunit, rotating, sendersysid, status";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, jsonBody, properties, "PATCH", "MERGE");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendItem: Respuesta " + response.Content);
                    UpdateItemSAP(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateItem(oCompany, item, listConfig);
                        logger.Error("SendItem " + response.Content);
                        UpdateItemSAP(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("SendItem " + response.Content);
                        UpdateItemSAP(item, oCompany, "E", resp.Error.message);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void UpdateItemSAP(ServiceClass item, Company oCompany, string state, string message, string id = "")
        {
            Items oItems = (Items)oCompany.GetBusinessObject(BoObjectTypes.oItems);
            try
            {

                if (!oItems.GetByKey(item.ItemCode))
                {
                    logger.Error("oItems no encontrado.");

                }

                oItems.UserFields.Fields.Item(Constants.U_EXX_MAX_STD).Value = state;
                oItems.UserFields.Fields.Item(Constants.U_EXX_MAX_MSJ).Value = message;

                if (!string.IsNullOrEmpty(id))
                    oItems.UserFields.Fields.Item(Constants.U_EXX_MAX_ID).Value = id;

                int result = oItems.Update();
                if (result != 0)
                {
                    logger.Error(" Error al actualizar Item: " + item.ItemCode + " : " + oCompany.GetLastErrorDescription());
                }
                else
                {
                    logger.Info("Item actualizado " + item.ItemCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                Marshal.ReleaseComObject(oItems);
                if (oItems != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
