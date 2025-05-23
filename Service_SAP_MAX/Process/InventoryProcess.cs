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
    public class InventoryProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(InventoryProcess));
        internal static void Process(ref Company oCompany, List<ConfigClass> listConfig)
        {
            try
            {
                var listSN = GetInventory(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_INVENTORY).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in listSN)
                {
                    if (string.IsNullOrEmpty(item.idMaximo))
                        SendInventory(oCompany, item, listConfig, url, maxAuth, authorization);
                    else
                        UpdateInventory(oCompany, item, listConfig, url, maxAuth, authorization);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.Message);
            }
        }

        private static void SendInventory(Company oCompany, InventoryClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
        {
            try
            {


                string body = JsonConvert.SerializeObject(item);
                var properties = "orgid, siteid, itemnum, itemsetid, itemtype, location, issueunit, orderunit, binnum, category, costtype, glaccount,sendersysid, status, inventoryid, invbalances.invbalancesid";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, body, properties);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<InventoryResponseClass>(response.Content);
                    logger.Info("SendInventory: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado", resp.inventoryid.ToString());
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateInventory(oCompany, item, listConfig);
                        logger.Error("SendInventory " + response.Content);
                        string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                        UpdateState(item, oCompany, "E", msg);
                    }
                    else
                    {
                        logger.Error("SendInventory " + response.Content);
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

        private static void UpdateInventory(Company oCompany, InventoryClass item, List<ConfigClass> listConfig, string url, string maxAuth, string authorization)
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

                var properties = "orgid, siteid, itemnum, itemsetid, itemtype, location, issueunit, orderunit, binnum, category, costtype, glaccount,sendersysid, status, inventoryid, invbalances.invbalancesid";

                var response = RestHelper.SendRest(url, RestSharp.Method.Post, maxAuth, authorization, jsonBody, properties, "PATCH", "MERGE");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resp = JsonConvert.DeserializeObject<CompaniesResponseClass>(response.Content);
                    logger.Info("SendInventory: Respuesta " + response.Content);
                    UpdateState(item, oCompany, "S", "Enviado");
                }
                else
                {


                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    if (resp.Error.message.Contains("ya existe"))
                    {
                        //UpdateInventory(oCompany, item, listConfig);
                        logger.Error("SendInventory " + response.Content);
                        UpdateState(item, oCompany, "E", resp.Error.message);
                    }
                    else
                    {
                        logger.Error("SendInventory " + response.Content);
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

        private static void UpdateState(InventoryClass item, Company oCompany, string state, string message, string id = "")
        {
            BusinessPartners obusinessPartners = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            try
            {

                if (!obusinessPartners.GetByKey(item.CodeSAP))
                {
                    logger.Error("obusinessPartners no encontrado.");

                }

                obusinessPartners.UserFields.Fields.Item(Constants.U_EXX_MAX_STD+"DI").Value = state;
                obusinessPartners.UserFields.Fields.Item(Constants.U_EXX_MAX_MSJ).Value = message;

                if (!string.IsNullOrEmpty(id))
                    obusinessPartners.UserFields.Fields.Item(Constants.U_EXX_MAX_ID+"IV").Value = id;

                int result = obusinessPartners.Update();
                if (result != 0)
                {
                    logger.Error(" Error al actualizar BP: " + item.CodeSAP + " : " + oCompany.GetLastErrorDescription());
                }
                else
                {
                    logger.Info("BP actualizado " + item.CodeSAP);
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

        private static List<InventoryClass> GetInventory(Company oCompany, List<ConfigClass> listConfig)
        {
            List<InventoryClass> list = new List<InventoryClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT TOP 100 
                                    ""ItemCode"",
                                    ""U_EXX_MAX_IDIV"",
                                    '' as ""binnum"",
                                    '004' as ""location"",
                                    CASE WHEN IFNULL(""SalUnitMsr"",'')='NIU' THEN 'UNI' ELSE IFNULL(""SalUnitMsr"",'') END as ""issueunit"",
                                    ""ItemCode"" as ""itemnum"",
                                    'ITEMSET' as ""itemsetid"",
                                    'ARTÍCULO' as ""itemtype"",
                                    'PTE'  as ""category"",
                                    'MEDIO'  as ""costtype"",
                                    '' as ""glaccount"",                               
                                    CASE WHEN IFNULL(""BuyUnitMsr"",'')='NIU' THEN 'UNI' ELSE IFNULL(""BuyUnitMsr"",'') END as ""orderunit"",
                                    'SAP-BO' as ""sendersysid"",
                                    'ACTIVO' as ""status"",
                                    --""U_EXX_MAX_GLA""
                                    '0' as ""avgcost"",
                                    '' as ""conditioncode"",
                                    '' as ""controlacc"",
                                    '' as ""invcostadjacc"",
                                    '' as ""shrinkageacc"",
                                    'TPE' as ""orgid"",
                                    'TP01' as ""siteid"",
                                    '' as ""curbal""
                                    FROM OITM CR
                                   
                                    WHERE 
                                    ""U_EXX_MAX_STDI""='P' 
                                    AND ""InvntItem""='Y'  AND ""ItemCode""='111000004'
";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    InventoryClass doc = new InventoryClass();
                    doc.CodeSAP = (string)recordset.Fields.Item("ItemCode").Value;
                    doc.idMaximo = (string)recordset.Fields.Item(Constants.U_EXX_MAX_ID+"IV").Value;
                    doc.orgid = (string)recordset.Fields.Item("orgid").Value;
                    doc.siteid = (string)recordset.Fields.Item("siteid").Value;
                    doc.itemnum = (string)recordset.Fields.Item("itemnum").Value;
                    doc.itemtype = (string)recordset.Fields.Item("itemtype").Value;
                    doc.location = (string)recordset.Fields.Item("location").Value;
                    doc.issueunit = (string)recordset.Fields.Item("issueunit").Value;
                    doc.orderunit = (string)recordset.Fields.Item("orderunit").Value;
                    doc.binnum = (string)recordset.Fields.Item("binnum").Value;
                    doc.category = (string)recordset.Fields.Item("category").Value;
                    doc.costtype = (string)recordset.Fields.Item("costtype").Value;
                    doc.glaccount = (string)recordset.Fields.Item("glaccount").Value;
                    doc.sendersysid = (string)recordset.Fields.Item("sendersysid").Value;
                    doc.status = (string)recordset.Fields.Item("status").Value;

                    Invcost _invcost = new Invcost();
                    _invcost.avgcost = (string)recordset.Fields.Item("avgcost").Value; ;
                    _invcost.conditioncode = (string)recordset.Fields.Item("conditioncode").Value; ;
                    _invcost.glaccount= (string)recordset.Fields.Item("glaccount").Value; ;
                    _invcost.controlacc = (string)recordset.Fields.Item("controlacc").Value; ;
                    _invcost.invcostadjacc = (string)recordset.Fields.Item("invcostadjacc").Value; ;
                    _invcost.shrinkageacc = (string)recordset.Fields.Item("shrinkageacc").Value; ;

                    doc.invcost = new List<Invcost>();
                    doc.invcost.Add(_invcost);

                    Invbalance _invBalance = new Invbalance();
                    _invBalance.curbal = (string)recordset.Fields.Item("curbal").Value; ;
                    _invBalance.binnum = (string)recordset.Fields.Item("binnum").Value; ;
                    _invBalance.lotnum = "";// (string)recordset.Fields.Item("lotnum").Value; ;
                    _invBalance.conditioncode = "";

                    doc.invbalances = new List<Invbalance>();
                    doc.invbalances.Add(_invBalance);

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
