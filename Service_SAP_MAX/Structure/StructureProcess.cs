using SAPbobsCOM;
using Service_SAP_MAX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Structure
{
    public class StructureProcess
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StructureProcess));

        public static void ValidateStructure()
        {
            Company oCompany = new Company();

            try
            {
                var isConnect = ConnectSAP.conectCompany(ref oCompany);

                if (isConnect)
                {
                    Process(ref oCompany);
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


        internal static void Process(ref Company oCompany)
        {
            try
            {
                if (CreateUserField(oCompany, "POR1", "EXX_MAX_OT", "OT MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado POR1 ");
                }
                return;
                
                if (CreateUserField(oCompany, "POR1", "EXX_MAX_LINE", "Línea MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado POR1 ");
                }
                return;
                if (CreateUserField(oCompany, "IGN1", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado OPOR ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "IGN1", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado OPOR");
                }
                if (CreateUserField(oCompany, "IGN1", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado IGN1 ");
                }
                return;
                if (CreateUserField(oCompany, "OPOR", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado OPOR ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "OPOR", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado OPOR");
                }
                if (CreateUserField(oCompany, "OPOR", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado OPOR ");
                }

                return;

                if (CreateUserFieldValidaValueYN(oCompany, "@" + Constants.TABLE_ACCT, "EXX_MAX_ACT", "Activo", BoFieldTypes.db_Alpha, 20))
                {
                    Console.WriteLine("Campo EXX_MAX_ACT creado TABLE_ACCT");
                }
                return;

                if (CreateUserField(oCompany, "OITM", "EXX_MAX_IDIV", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado OITM ");
                }
                return;
                if (CreateUserFieldValidaValuePE(oCompany, "OITM", "EXX_MAX_STDI", "Estado Envio MAX INV", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_STDI creado OITM ");
                }
                return;
                if (CreateUserField(oCompany, "OITM", "EXX_MAX_CACT", "Cuenta Control", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_GLA creado OITM ");
                }

                if (CreateUserField(oCompany, "OITM", "EXX_MAX_CLM", "Cuenta LM", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_CLM creado OITM ");
                }


               
                if (CreateUserField(oCompany, "OITM", "EXX_MAX_GLA", "GL Account Maximo", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_GLA creado OITM ");
                }
                return;
                if (CreateUserField(oCompany, "OITM", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado OITM ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "OITM", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado OITM");
                }
                if (CreateUserField(oCompany, "OITM", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado OITM ");
                }

                return;

                if (CreateUserField(oCompany, "OCRD", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado OCRD ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "OCRD", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado OCRD");
                }
                if (CreateUserField(oCompany, "OCRD", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado OCRD ");
                }

                return;


                if (CreateUserFieldValidaValuePE(oCompany, "ORTT", "EXX_MAX_STD2", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado ORTT");
                }
                return;

                if (CreateUserField(oCompany, "ORTT", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado ORTT ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "ORTT", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado ORTT");
                }
                if (CreateUserField(oCompany, "ORTT", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado ORTT ");
                }

                return;
                if (CreateUserTable(oCompany, Constants.TABLE_ACCT, "Tabla Cuentas", BoUTBTableType.bott_NoObject))
                {
                    logger.Info("Tabla creada exitosamente.");
                }


                if (CreateUserField(oCompany, "@" + Constants.TABLE_ACCT, "EXX_GL1", "GL1", BoFieldTypes.db_Alpha, 100))
                {
                    logger.Info("Campo EXX_GL1 creado.");
                }
                if (CreateUserField(oCompany, "@" + Constants.TABLE_ACCT, "EXX_GL2", "GL2", BoFieldTypes.db_Alpha, 100))
                {
                    logger.Info("Campo EXX_GL2 creado.");
                }
                if (CreateUserField(oCompany, "@" + Constants.TABLE_ACCT, "EXX_GL3", "GL3", BoFieldTypes.db_Alpha, 100))
                {
                    logger.Info("Campo EXX_GL3 creado.");
                }
                if (CreateUserField(oCompany, "@" + Constants.TABLE_ACCT, "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado TABLE_ACCT ");
                }
                if (CreateUserField(oCompany, "@" + Constants.TABLE_ACCT, "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado TABLE_ACCT ");
                }
                if (CreateUserFieldValidaValuePE(oCompany, "@" + Constants.TABLE_ACCT, "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 20))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado TABLE_ACCT");
                }

                if (CreateUserTable(oCompany, Constants.TABLE_CONF, "Conf. SIP NAVIS", BoUTBTableType.bott_NoObject))
                {
                    logger.Info("Tabla creada exitosamente.");
                }

                //if (CreateUserTable(oCompany, Constants.TABLE_STA_SIP_FE, "Estado FE SIP NAVI", BoUTBTableType.bott_NoObject))
                //{
                //    logger.Info("Tabla creada exitosamente.");
                //}
                if (CreateUserField(oCompany, "@" + Constants.TABLE_CONF, "EXX_VALUE", "Valor", BoFieldTypes.db_Alpha, 200))
                {
                    logger.Info("Campo EXX_VALUE creado.");
                }

                //if (CreateUserField(oCompany, "@" + Constants.TABLE_STA_SIP_FE, "EXX_REF", "Estado SIP", BoFieldTypes.db_Alpha, 50))
                //{
                //    logger.Info("Campo EXX_REF creado.");
                //}

                //if (CreateUserFieldWithLink(oCompany, "OINV", "EXX_STA_SIP", "Estado FE SIP", Constants.TABLE_STA_SIP_FE, 50))
                //{
                //    Console.WriteLine("Campo EXX_STA_SIP creado");
                //}

                //if (CreateUserField(oCompany, "OINV", "EXX_MSJ_SIP", "Mensaje FE SIP", BoFieldTypes.db_Alpha, 249))
                //{
                //    Console.WriteLine("Campo EXX_MSJ_SIP creado");
                //}


                //if (CreateUserFieldValidaValueYN(oCompany, "OINV", "EXX_SIP_XML", "Estado Envio XML", BoFieldTypes.db_Alpha, 10))
                //{
                //    Console.WriteLine("Campo EXX_MSJ_SIP creado");
                //}
                //if (CreateUserFieldValidaValueYN(oCompany, "OINV", "EXX_SIP_PDF", "Estado Envio PDF", BoFieldTypes.db_Alpha, 10))
                //{
                //    Console.WriteLine("Campo EXX_MSJ_SIP creado");
                //}


                if (CreateUserField(oCompany, "OACT", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado OACT ");
                }
                if (CreateUserField(oCompany, "OACT", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado OACT ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "OACT", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado OACT");
                }



                if (CreateUserField(oCompany, "OPRC", "EXX_MAX_MSJ", "Mensaje MAX", BoFieldTypes.db_Alpha, 249))
                {
                    Console.WriteLine("Campo EXX_MAX_MSJ creado OPRC ");
                }

                if (CreateUserFieldValidaValuePE(oCompany, "OPRC", "EXX_MAX_STD", "Estado Envio MAX", BoFieldTypes.db_Alpha, 10))
                {
                    Console.WriteLine("Campo EXX_MAX_STD creado OPRC");
                }
                if (CreateUserField(oCompany, "OPRC", "EXX_MAX_ID", "ID MAX", BoFieldTypes.db_Alpha, 100))
                {
                    Console.WriteLine("Campo EXX_MAX_ID creado OPRC ");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static bool CreateUserFieldValidaValuePE(Company oCompany, string tableName, string fieldName, string fieldDescription, BoFieldTypes fieldType, int fieldSize)
        {
            UserFieldsMD oUserField = (UserFieldsMD)oCompany.GetBusinessObject(BoObjectTypes.oUserFields);
            try
            {
                oUserField.TableName = tableName;
                oUserField.Name = fieldName;
                oUserField.Description = fieldDescription;
                oUserField.Type = fieldType;

                if (fieldType == BoFieldTypes.db_Alpha || fieldType == BoFieldTypes.db_Memo)
                {
                    oUserField.Size = fieldSize;
                }


                oUserField.ValidValues.Value = "P";
                oUserField.ValidValues.Description = "Pendiente";
                oUserField.ValidValues.Add();

                oUserField.ValidValues.Value = "S";
                oUserField.ValidValues.Description = "Enviado";
                oUserField.ValidValues.Add();

                oUserField.ValidValues.Value = "E";
                oUserField.ValidValues.Description = "Error";
                oUserField.ValidValues.Add();

                oUserField.DefaultValue = "P";

                int result = oUserField.Add();
                if (result != 0)
                {
                    logger.Error("Error al crear campo: " + oCompany.GetLastErrorDescription());
                    return false;
                }

                return true;
            }
            finally
            {
                // 🔹 Liberar memoria del objeto COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserField);
                oUserField = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static bool CreateUserFieldValidaValueYN(Company oCompany, string tableName, string fieldName, string fieldDescription, BoFieldTypes fieldType, int fieldSize)
        {
            UserFieldsMD oUserField = (UserFieldsMD)oCompany.GetBusinessObject(BoObjectTypes.oUserFields);
            try
            {
                oUserField.TableName = tableName;
                oUserField.Name = fieldName;
                oUserField.Description = fieldDescription;
                oUserField.Type = fieldType;

                if (fieldType == BoFieldTypes.db_Alpha || fieldType == BoFieldTypes.db_Memo)
                {
                    oUserField.Size = fieldSize;
                }


                oUserField.ValidValues.Value = "Y";
                oUserField.ValidValues.Description = "Si";
                oUserField.ValidValues.Add();

                oUserField.ValidValues.Value = "N";
                oUserField.ValidValues.Description = "No";
                oUserField.ValidValues.Add();

                oUserField.DefaultValue = "N";

                int result = oUserField.Add();
                if (result != 0)
                {
                    logger.Error("Error al crear campo: " + oCompany.GetLastErrorDescription());
                    return false;
                }

                return true;
            }
            finally
            {
                // 🔹 Liberar memoria del objeto COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserField);
                oUserField = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        static bool CreateUserTable(Company oCompany, string tableName, string tableDescription, BoUTBTableType tableType)
        {
            UserTablesMD oUserTable = (UserTablesMD)oCompany.GetBusinessObject(BoObjectTypes.oUserTables);
            try
            {


                if (oUserTable.GetByKey(tableName))
                {
                    logger.Info($"La tabla {tableName} ya existe.");
                    return false;
                }

                oUserTable.TableName = tableName;
                oUserTable.TableDescription = tableDescription;
                oUserTable.TableType = tableType;

                int result = oUserTable.Add();
                if (result != 0)
                {
                    logger.Error("Error al crear tabla: " + oCompany.GetLastErrorDescription());
                    return false;
                }

                return true;
            }
            finally
            {
                // 🔹 Liberar memoria del objeto COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTable);
                oUserTable = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }

        static bool CreateUserField(Company oCompany, string tableName, string fieldName, string fieldDescription, BoFieldTypes fieldType, int fieldSize)
        {
            UserFieldsMD oUserField = (UserFieldsMD)oCompany.GetBusinessObject(BoObjectTypes.oUserFields);
            try
            {
                oUserField.TableName = tableName;
                oUserField.Name = fieldName;
                oUserField.Description = fieldDescription;
                oUserField.Type = fieldType;

                if (fieldType == BoFieldTypes.db_Alpha || fieldType == BoFieldTypes.db_Memo)
                {
                    oUserField.Size = fieldSize;
                }

                int result = oUserField.Add();
                if (result != 0)
                {
                    logger.Error("Error al crear campo: " + oCompany.GetLastErrorDescription());
                    return false;
                }

                return true;
            }
            finally
            {
                // 🔹 Liberar memoria del objeto COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserField);
                oUserField = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }

        static bool CreateUserFieldWithLink(Company oCompany, string tableName, string fieldName, string fieldDescription, string linkedTable, int size)
        {
            UserFieldsMD oUserField = (UserFieldsMD)oCompany.GetBusinessObject(BoObjectTypes.oUserFields);

            try
            {
                oUserField.TableName = tableName;
                oUserField.Name = fieldName;
                oUserField.Description = fieldDescription;
                oUserField.Type = BoFieldTypes.db_Alpha;
                oUserField.Size = size; // Ajustar según el tamaño de la clave en la tabla de usuario

                // 🔹 Vinculación con la tabla de usuario
                oUserField.LinkedTable = linkedTable;

                int result = oUserField.Add();
                if (result != 0)
                {
                    logger.Error("Error al crear campo vinculado: " + oCompany.GetLastErrorDescription());
                    return false;
                }

                return true;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserField);
                oUserField = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


    }
}
