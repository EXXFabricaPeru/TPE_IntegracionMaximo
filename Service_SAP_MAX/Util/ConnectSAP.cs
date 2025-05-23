using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Util
{
    public class ConnectSAP
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ConnectSAP));
        public static bool conectCompany(ref Company oCompany)
        {
            try
            {
                oCompany.DbPassword = ConfigurationManager.AppSettings["BD_PASS"];// "";
                oCompany.DbUserName = ConfigurationManager.AppSettings["BD_USER"]; //"";
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                oCompany.CompanyDB = ConfigurationManager.AppSettings["BD_NAME"]; //"";
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish;
                oCompany.UserName = ConfigurationManager.AppSettings["SAP_USER"];
                oCompany.Password = ConfigurationManager.AppSettings["SAP_PASS"];
                oCompany.Server = ConfigurationManager.AppSettings["SERVER"];

                if (!oCompany.Connected)
                {
                    int result = oCompany.Connect();
                    if (result != 0)
                    {
                        //Console.WriteLine("Error al conectar a SAP: " + oCompany.GetLastErrorDescription());
                        logger.Error("Error al conectar a SAP: " + oCompany.GetLastErrorDescription());
                        return false;
                    }
                    logger.Info("Conectado a SAP");
                    return true;
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
