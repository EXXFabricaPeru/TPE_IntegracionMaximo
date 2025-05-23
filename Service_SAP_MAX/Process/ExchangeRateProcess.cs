using Newtonsoft.Json;
using RestSharp;
using SAPbobsCOM;
using Service_SAP_MAX.Entities;
using Service_SAP_MAX.Entities.Response;
using Service_SAP_MAX.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Process
{
    public class ExchangeRateProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ExchangeRateProcess));

        internal static void Process(ref SAPbobsCOM.Company oCompany, List<Entities.ConfigClass> listConfig)
        {
            try
            {
                var list = GetExchangeRate(oCompany, listConfig);
                var url = listConfig.Where(t => t.Code == Constants.URL_EXC_RATE).FirstOrDefault().Value;
                var maxAuth = listConfig.Where(t => t.Code == Constants.MAX_AUTH).FirstOrDefault().Value;
                var authorization = listConfig.Where(t => t.Code == Constants.AUTHORIZATION).FirstOrDefault().Value;


                foreach (var item in list)
                {
                    SendRate(item, oCompany, listConfig, url, maxAuth, authorization, true);
                }

                list = GetExchangeRate2(oCompany, listConfig);

                foreach (var item in list)
                {
                    SendRate(item, oCompany, listConfig, url, maxAuth, authorization);
                }

            }
            catch (Exception ex)
            {

                logger.Error(ex.Message, ex);
            }


        }

        private static List<ExchangeClass> GetExchangeRate2(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ExchangeClass> list = new List<ExchangeClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT 
                                ""RateDate"" AS ""Fecha"",
                                ""Currency"" AS ""Moneda"",
                                ""Rate"" AS ""exchangerate"",
                                ""U_EXX_MAX_STD2""
                                FROM ""ORTT""
                                WHERE ""RateDate"" = '{DateTime.Now.ToString("yyyy-MM-dd")}' --2025-04-02'  
                                AND  ""U_EXX_MAX_STD2""='P'
                    ORDER BY ""Currency"";

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    ExchangeClass doc = new ExchangeClass();
                    doc.orgid = "TPE";// (int)recordset.Fields.Item("DocEntry").Value;

                    doc.currencycodeto = (string)recordset.Fields.Item("Moneda").Value;
                    doc.currencycode = "PEN"; //(string)recordset.Fields.Item("Moneda").Value;
                    doc.exchangerate = decimal.Parse(recordset.Fields.Item("exchangerate").Value.ToString());
                    doc.exchangerate2 = decimal.Parse(recordset.Fields.Item("exchangerate").Value.ToString());
                    doc.activedate = (DateTime)recordset.Fields.Item("Fecha").Value;// DateTime.Now;// DateTime.Parse("2025-02-04T08:00:00-05:00");//recordset.Fields.Item("glcomp04").Value.ToString();
                    doc.expiredate = (DateTime)recordset.Fields.Item("Fecha").Value;// DateTime.Now;//DateTime.Parse("2025-02-04T08:00:00-05:00");//;"TPE";//(string)recordset.Fields.Item("TableName").Value;
                    doc.enterby = "SAP";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.enterdate = (DateTime)recordset.Fields.Item("Fecha").Value;
                    doc.memo = "Envío SAP";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.expiredate = doc.expiredate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    doc.exchangerate = Math.Round(1 / doc.exchangerate, 3);
                    doc.exchangerate2 = Math.Round(1 / doc.exchangerate2, 3);
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

        private static void SendRate(ExchangeClass item, Company oCompany, List<ConfigClass> listConfig, string url, string maxAuth, string authorization, bool normal = false)
        {
            try
            {

                var jsonBody = JsonConvert.SerializeObject(item);
                var properties = "currencycode, currencycodeto, enterby, enterdate, activedate, expiredate, exchangerate, exchangerate2, orgid, memo";
                var response = RestHelper.SendRest(url, Method.Post, maxAuth, authorization, jsonBody, properties);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var resp = JsonConvert.DeserializeObject<ExchangeResponseClass>(response.Content);

                    if (normal)
                        UpdateState("S", "Enviado", oCompany, item);
                    else
                    {
                        UpdateState("S", "Enviado", oCompany, item, "2");
                    }
                }
                else
                {
                    var resp = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    logger.Error(response.Content);
                    string msg = resp.Error.message.Length > 249 ? resp.Error.message.Substring(0, 249) : resp.Error.message;
                    if (response.Content.Contains("ya existe"))
                    {
                        if (normal)
                            UpdateState("S","Enviado", oCompany, item);
                        else
                            UpdateState("S", "Enviado", oCompany, item, "2");
                    }

                    else
                    {
                        if (normal)
                            UpdateState("E", msg, oCompany, item);
                        else
                            UpdateState("E", msg, oCompany, item, "2");
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static void UpdateState(string state, string message, Company oCompany, ExchangeClass item, string id = "")
        {
            Recordset recordset = null;
            try
            {
                string query = "";
                string setId = "";
                if (!string.IsNullOrEmpty(id))
                    setId = $@"2";

                query = $"UPDATE \"ORTT\" SET \"{Constants.U_EXX_MAX_STD}{setId}\" = '{state}',\"{Constants.U_EXX_MAX_MSJ}\" = '{message}' WHERE \"Currency\"='{item.currencycode}' AND \"RateDate\"='{item.activedate.ToString("yyyy-MM-dd")}' ";


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

        private static List<ExchangeClass> GetExchangeRate(Company oCompany, List<ConfigClass> listConfig)
        {
            List<ExchangeClass> list = new List<ExchangeClass>();
            Recordset recordset = null;
            try
            {
                recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = $@"SELECT 
                                ""RateDate"" AS ""Fecha"",
                                ""Currency"" AS ""Moneda"",
                                ""Rate"" AS ""exchangerate"",
                                ""U_EXX_MAX_STD""
                                FROM ""ORTT""
                                WHERE ""RateDate"" = '{DateTime.Now.ToString("yyyy-MM-dd")}' --2025-04-02'  
                                AND  ""U_EXX_MAX_STD""='E'
                                ORDER BY ""Currency"";

";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    ExchangeClass doc = new ExchangeClass();
                    doc.orgid = "TPE";// (int)recordset.Fields.Item("DocEntry").Value;
                    doc.currencycode = (string)recordset.Fields.Item("Moneda").Value;
                    doc.currencycodeto = "PEN"; //(string)recordset.Fields.Item("Moneda").Value;
                    doc.exchangerate = decimal.Parse(recordset.Fields.Item("exchangerate").Value.ToString());
                    doc.exchangerate2 = decimal.Parse(recordset.Fields.Item("exchangerate").Value.ToString());
                    doc.activedate = (DateTime)recordset.Fields.Item("Fecha").Value;// DateTime.Now;// DateTime.Parse("2025-02-04T08:00:00-05:00");//recordset.Fields.Item("glcomp04").Value.ToString();
                    doc.expiredate = (DateTime)recordset.Fields.Item("Fecha").Value;// DateTime.Now;//DateTime.Parse("2025-02-04T08:00:00-05:00");//;"TPE";//(string)recordset.Fields.Item("TableName").Value;
                    doc.enterby = "SAP";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.enterdate = (DateTime)recordset.Fields.Item("Fecha").Value;
                    doc.memo = "Envío SAP";// (string)recordset.Fields.Item("U_SMC_ESTADO_FE").Value;
                    doc.expiredate = doc.expiredate.AddHours(23).AddMinutes(59).AddSeconds(59);

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
