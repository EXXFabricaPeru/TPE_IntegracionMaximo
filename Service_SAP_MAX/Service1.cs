using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using SAPbobsCOM;
using Service_SAP_MAX.Entities;
using Service_SAP_MAX.Process;
using Service_SAP_MAX.Structure;
using Service_SAP_MAX.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Service_SAP_MAX
{
    public partial class Service1 : ServiceBase
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Service1));
        Timer aTimer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SetUpLogger();
            logger.Debug("Inicializando servicio");
            InicializarServicio();
            logger.Info("Servicio iniciado");
            SetTemporizador();
        }

        protected override void OnStop()
        {
            logger.Info("Servicio detenido");
        }

        public static void SetUpLogger()
        {
            try
            {
                string nivel = System.Configuration.ConfigurationManager.AppSettings["LogLevel"];
                //nivel = "DEBUG";
                Hierarchy hierarchy = (Hierarchy)log4net.LogManager.GetRepository();

                PatternLayout patternLayout = new PatternLayout();
                patternLayout.ConversionPattern = "*%-10level %-25date [%logger] [%method] %message %newline";
                patternLayout.ActivateOptions();

                RollingFileAppender roller = new RollingFileAppender();
                roller.AppendToFile = true;
                roller.File = @"Log\.log";
                roller.DatePattern = "'Log_'dd_MM_yyyy";
                roller.Layout = patternLayout;
                roller.MaxSizeRollBackups = 10;
                roller.PreserveLogFileNameExtension = true;
                roller.MaximumFileSize = "5MB";
                roller.RollingStyle = RollingFileAppender.RollingMode.Date;
                roller.StaticLogFileName = false;
                roller.ActivateOptions();
                hierarchy.Root.AddAppender(roller);

                ConsoleAppender console = new ConsoleAppender();
                console.Layout = patternLayout;
                console.ActivateOptions();
                hierarchy.Root.AddAppender(console);

                hierarchy.Root.Level = hierarchy.LevelMap[nivel] == null ? log4net.Core.Level.Error : hierarchy.LevelMap[nivel];
                hierarchy.Configured = true;
            }
            catch { }
        }

        private void SetTemporizador()
        {
            string tiempo = System.Configuration.ConfigurationManager.AppSettings["Temporizador"];
            int intervalo = 60;
            int.TryParse(tiempo, out intervalo);

            aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = intervalo * 1000;
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;

            Company oCompany = new Company();
            try
            {
                logger.Debug("OnTimedEvent");

                var isConnect =  ConnectSAP.conectCompany(ref oCompany);


                if (isConnect)
                {
                    List<ConfigClass> listConfig = GetConfig(oCompany);

                    //GLComponentProcess.Process(ref oCompany, listConfig);
                    //AccountingAccountsProcess.Process(ref oCompany, listConfig);
                    //ExchangeRateProcess.Process(ref oCompany, listConfig);
                    //BusinessPartnerProcess.Process(ref oCompany, listConfig);
                    //ItemsProcess.Process(ref oCompany, listConfig);
                    //InventoryProcess.Process(ref oCompany, listConfig);
                    //PurchaseOrderProcess.Process(ref oCompany, listConfig);
                    //InventoryBalanceProcess.Process(ref oCompany, listConfig);
                    //CostAdjustmentsProcess.Process(ref oCompany, listConfig);
                    //ReceiptsReturnsProcess.Process(ref oCompany, listConfig);
                    //DispatchProcess.Process(ref oCompany, listConfig);
                    //WarehouseTransferProcess.Process(ref oCompany, listConfig);
                }
                else
                {
                    logger.Error("Error al conectar a SAP");
                }
              
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                if (oCompany.Connected)
                    oCompany.Disconnect();

                Marshal.ReleaseComObject(oCompany);
                if (oCompany != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            aTimer.Enabled = true;
        }

        public static void InicializarServicio()
        {
            //StructureProcess.ValidateStructure();
        }

        private static List<ConfigClass> GetConfig(Company oCompany)
        {
            List<ConfigClass> list = new List<ConfigClass>();
            Recordset recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {

                string query = $@"SELECT * FROM ""@{Constants.TABLE_CONF}"" ";
                recordset.DoQuery(query);

                while (!recordset.EoF)
                {
                    ConfigClass doc = new ConfigClass();
                    doc.Code = (string)recordset.Fields.Item("Code").Value;
                    doc.Name = (string)recordset.Fields.Item("Name").Value;
                    doc.Value = (string)recordset.Fields.Item("U_EXX_VALUE").Value;
                    list.Add(doc);
                    recordset.MoveNext();
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);

            }
            finally
            {
                Marshal.ReleaseComObject(recordset);
                if (recordset != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return list;
        }


        public static void Debug()
        {
            //StructureProcess.ValidateStructure();

            Company oCompany = new Company();
            try
            {
                //logger.Debug("OnTimedEvent");

                var isConnect = ConnectSAP.conectCompany(ref oCompany);


                if (isConnect)
                {
                    List<ConfigClass> listConfig = GetConfig(oCompany);

                    GLComponentProcess.Process(ref oCompany, listConfig);
                    AccountingAccountsProcess.Process(ref oCompany, listConfig);
                    ExchangeRateProcess.Process(ref oCompany, listConfig);
                    BusinessPartnerProcess.Process(ref oCompany, listConfig);
                    ItemsProcess.Process(ref oCompany, listConfig);
                    InventoryProcess.Process(ref oCompany, listConfig);
                    PurchaseOrderProcess.Process(ref oCompany, listConfig);
                    InventoryBalanceProcess.Process(ref oCompany, listConfig);
                    CostAdjustmentsProcess.Process(ref oCompany, listConfig);
                    ReceiptsReturnsProcess.Process(ref oCompany, listConfig);
                    DispatchProcess.Process(ref oCompany, listConfig);
                    WarehouseTransferProcess.Process(ref oCompany, listConfig);
                }
                else
                {
                    logger.Error("Error al conectar a SAP");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                if (oCompany.Connected)
                    oCompany.Disconnect();

                Marshal.ReleaseComObject(oCompany);
                if (oCompany != null)
                    oCompany = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
