using System.Data;
using System.Diagnostics;
using VisanBC25.Modelos;

namespace VisanBC25
{
    public static class Funciones
    {
        public static void KillAll()
        {
            string MiProceso = Process.GetCurrentProcess().ProcessName;
            int i;

            i = MiProceso.IndexOf(".");
            if (i > 0) MiProceso = MiProceso.Substring(0, i - 1);

            foreach (Process pr in Process.GetProcessesByName(MiProceso))
            {
                if (pr.Id != Process.GetCurrentProcess().Id) pr.Kill();
            }
        }



        public static string Menu(string Version)
        {
            Console.Clear();
            Console.WriteLine($"{new string('-', 70)}{Version}");
            Console.WriteLine(" 0. Test");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine(" 1. Formas y Términos de Pago\t\t 3. VAT Posting Setup");
            Console.WriteLine(" 2. Países\t\t\t\t 4. FA Posting Group");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("10. Clientes\t\t\t\t15. AF Depr. Book");
            Console.WriteLine("11. Provedores\t\t\t\t16. Dir. Envío");
            Console.WriteLine("12. Bancos\t\t\t\t17. Bancos Clientes");
            Console.WriteLine("13. Cuentas Contables\t\t\t18. Bancos Provedores");
            Console.WriteLine("14. Activos Fijos");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("50. Facturas Venta\t\t\t51. Facturas Compra");
            Console.WriteLine("52. Diario\t\t\t\t59. Diario Saldos Iniciales");
            Console.WriteLine("53. Remesas\t\t\t\t54. Órdenes de Pago");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("60. Productos Básica \t\t\t61.Productos Completa");
            Console.WriteLine("62. Productos Completa json");
            Console.WriteLine("69. Item Test");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("70. Coste y Amortización AF");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("99. Salir");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("(+) al final = Borrar Datos Existentes");
            Console.WriteLine();
            Console.Write("Pulse Opción: ");

            string Opc = Console.ReadLine();
            if (string.IsNullOrEmpty(Opc)) Opc = "99";

            string Borrar = string.Empty;

            if (Opc.Substring(Opc.Length - 1, 1) == "+")
            {
                Borrar = "+";
                Opc = Opc.Substring(0, Opc.Length - 1);
            }

            switch (Opc)
            {
                case "99": return $"SALIR{Borrar}";
                case "0": return $"TEST";
                case "1": return $"FPAGO{Borrar}";
                case "2": return $"PAISES{Borrar}";
                case "3": return $"VAT-SETUP{Borrar}";
                case "4": return $"FA-POST{Borrar}";
                case "10": return $"CLIENTES{Borrar}";
                case "11": return $"PROVEEDORES{Borrar}";
                case "12": return $"BANCOS{Borrar}";
                case "13": return $"CUENTAS{Borrar}";
                case "14": return $"ACTIVOS{Borrar}";
                case "15": return $"FA-BOOK{Borrar}";
                case "16": return $"DIRECCIONES{Borrar}";
                case "17": return $"BANCOS-CLI{Borrar}";
                case "18": return $"BANCOS-PRO{Borrar}";
                case "50": return $"FAC-V{Borrar}";
                case "51": return $"FAC-C{Borrar}";
                case "52": return $"DIARIO{Borrar}";
                case "53": return $"REMESA{Borrar}";
                case "54": return $"OPAGO{Borrar}";
                case "59": return $"SALDOS{Borrar}";
                case "60": return $"ITEM-B{Borrar}";
                case "61": return $"ITEM-C{Borrar}";
                case "62": return $"ITEM-J{Borrar}";
                case "70": return $"COSTE-AF{Borrar}";
                case "69": return $"ITEM-TEST";
            }

            return "ERROR";
        }


        public static bool Leer_Parametros(ref m_Datos Datos, string Opcion)
        {
            bool OK = true;

            try
            {
                Datos = new m_Datos();
                Datos.Folder = System.Configuration.ConfigurationManager.AppSettings["FOLDER"];
                Datos.Log = System.Configuration.ConfigurationManager.AppSettings["LOG"];
                Datos.LogDetalle = System.Configuration.ConfigurationManager.AppSettings["LOG-DETALLE"];
                Datos.Version = System.Configuration.ConfigurationManager.AppSettings["VERSION"];
                Datos.User = System.Configuration.ConfigurationManager.AppSettings["USER"]; ;
                Datos.Password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"];

                Datos.Server = System.Configuration.ConfigurationManager.AppSettings["SERVER"];
                Datos.Database = System.Configuration.ConfigurationManager.AppSettings["DATABASE"];

                if (Opcion == "GL")
                {
                    Datos.Company = System.Configuration.ConfigurationManager.AppSettings["GL-COMPANY"];
                    Datos.CompanyWeb = System.Configuration.ConfigurationManager.AppSettings["GL-COMPANY-WEB"];
                    Datos.Codeunit = System.Configuration.ConfigurationManager.AppSettings["GL-CODEUNIT"];
                    Datos.ServerWeb = System.Configuration.ConfigurationManager.AppSettings["GL-SERVER-WEB"];
                    Datos.Puerto = int.Parse(System.Configuration.ConfigurationManager.AppSettings["GL-PUERTO"]);
                    Datos.UserBC = System.Configuration.ConfigurationManager.AppSettings["GL-USER-BC"]; ;
                    Datos.PasswdBC = System.Configuration.ConfigurationManager.AppSettings["GL-PASSWD-BC"];
                    Datos.DomainBC = System.Configuration.ConfigurationManager.AppSettings["GL-DOMAIN-BC"];
                    Datos.Instancia = System.Configuration.ConfigurationManager.AppSettings["GL-INSTANCIA"];
                }

                if (Opcion == "ITEM")
                {
                    Datos.Company = System.Configuration.ConfigurationManager.AppSettings["ITEM-COMPANY"];
                    Datos.CompanyWeb = System.Configuration.ConfigurationManager.AppSettings["ITEM-COMPANY-WEB"];
                    Datos.Codeunit = System.Configuration.ConfigurationManager.AppSettings["ITEM-CODEUNIT"];
                    Datos.ServerWeb = System.Configuration.ConfigurationManager.AppSettings["ITEM-SERVER-WEB"];
                    Datos.Puerto = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ITEM-PUERTO"]);
                    Datos.UserBC = System.Configuration.ConfigurationManager.AppSettings["ITEM-USER-BC"]; ;
                    Datos.PasswdBC = System.Configuration.ConfigurationManager.AppSettings["ITEM-PASSWD-BC"];
                    Datos.DomainBC = System.Configuration.ConfigurationManager.AppSettings["ITEM-DOMAIN-BC"];
                    Datos.Instancia = System.Configuration.ConfigurationManager.AppSettings["ITEM-INSTANCIA"];
                }

                Datos.Estado = true;
            }
            catch (Exception ex)
            {
                Datos.Estado = false;
                Datos.Error = $@"Error al leer parámetros iniciales: {ex.Message}";
            }
            return Datos.Estado;
        }


        public static async Task<string> Log(m_Datos Datos, string Texto)
        {
            string Estado = "OK";
            StreamWriter sr;

            try
            {
                sr = new StreamWriter(Datos.Log, true);
                sr.WriteLine($@"{Datos.Version}  {DateTime.Now.ToString("dd.MM.yy HH:mm:ss")} {Texto}");
                sr.Close();
            }
            catch (Exception ex)
            {
                Estado = $"Error Generar log: {ex.Message}";
            }

            return Estado;
        }

        public static async Task<string> Log_Detalle(m_Datos Datos, String xTipo, string xDocumento, string xEstado, string xError)
        {
            string Estado = "OK";
            StreamWriter sr;
            string html = string.Empty;

            try
            {
                if (!File.Exists(Datos.LogDetalle))
                {
                    html = $@"<html><head></head><body><FONT FACE='Trebuchet' SIZE=4 COLOR=black><br/><br/><br/>
                            <table width= '800' border='1'><thead><tr>
                            <th bgcolor='#5D7B9D'><font color='#fff'>Fecha</font></th>
                            <th bgcolor='#5D7B9D'><font color='#fff'>Tipo Doc.</font></th>
                            <th bgcolor='#5D7B9D'><font color='#fff'>Documento</font></th>
                            <th bgcolor='#5D7B9D'><font color='#fff'>Estado</font></th>
                            <th bgcolor='#5D7B9D'><font color='#fff'>Error</font></th>
                            </tr></thead> ";
                }

                html += $@"<tr><td align=Left>{DateTime.Today.ToString("d")}</td>
                            <td align=left>{xTipo}</td>
                            <td align=left>{xDocumento}</td>
                            <td align=Center>{xEstado}</td>                            
                            <td align=Left>{xError}</td></tr>";

                sr = new StreamWriter(Datos.LogDetalle, true);
                sr.WriteLine(html.Replace("'", Convert.ToChar(34).ToString()));
                sr.Close();
            }
            catch (Exception ex)
            {
                Estado = $"Error Generar log Detalle: {ex.Message}";
            }

            return Estado;
        }

        public static async Task<string> Crear_Select(m_Datos Datos, string xTabla)
        {
            string Campos = String.Empty;
            string SqlSelect = String.Empty;
            string tt = String.Empty;
            Sql s = new Sql();

            tt = $"SELECT [CampoNAV] FROM [BC25] WHERE [TablaBC] = '{xTabla}' and [TablaNAV] <> '' AND [CampoNAV] <> '' Group by [CampoNAV]";

            await s.Cargar_TableData(Datos, tt);
            if (s.mSql.Estado)
            {
                foreach (DataRow row in s.mSql.TableData.Rows)
                {
                    if (Campos.Length > 0) Campos += ",";
                    Campos += $"[{row["CampoNav"]}]";
                }
            }

            SqlSelect = $"SELECT {Campos} FROM [{Datos.Company}${xTabla}]";
            return SqlSelect;
        }

        public static string Nombres_Columnas(string Columna)
        {
            Columna = Columna.Replace(" ", "_");
            Columna = Columna.Replace("á", "a");
            Columna = Columna.Replace("é", "e");
            Columna = Columna.Replace("í", "i");
            Columna = Columna.Replace("ó", "o");
            Columna = Columna.Replace("u", "u");
            Columna = Columna.Replace(",", "_");

            return (Columna);
        }

        public static string Quitar_Correlation(string xRespuesta)
        {
            int Pos = xRespuesta.IndexOf("Correlation");
            if (Pos > 0)
            {
                xRespuesta = xRespuesta.Substring(0, Pos - 1);
            }

            return xRespuesta;
        }

        public static bool Columna_Vacia(DataRow row, DataColumn column)
        {
            bool vacia = false;

            try
            {
                switch (column.DataType.Name)
                {
                    case "String":
                        vacia = (row[column].ToString().Trim() == string.Empty);
                        break;
                    case "Boolean" or "Byte":
                        vacia = (int.Parse(row[column].ToString()) == 0);
                        break;
                    case "Decimal":
                        vacia = (decimal.Parse(row[column].ToString()) == 0);
                        break;
                    case "Double":
                        vacia = (double.Parse(row[column].ToString()) == 0);
                        break;
                    case "Single":
                        vacia = (Single.Parse(row[column].ToString()) == 0);
                        break;
                    case "Int16":
                        vacia = (Int16.Parse(row[column].ToString()) == 0);
                        break;
                    case "Int32":
                        vacia = (Int32.Parse(row[column].ToString()) == 0);
                        break;
                    case "Int64":
                        vacia = (Int64.Parse(row[column].ToString()) == 0);
                        break;
                    case "DateTime":
                        vacia = (DateTime.Parse(row[column].ToString()).Year < 2000);
                        break;
                }
            }
            catch (Exception ex)
            {
                vacia = true;
                Funciones.Log(new m_Datos(), $"Error al comprobar si columna {column.ColumnName} está vacía: {ex.Message}");
            }

            return vacia;
        }

        public static async Task<bool> Insertar_Error(m_Datos Datos, Sql s, string xTabla, string xClave, string Texto)
        {
            Texto = Texto.Replace("'", " ");
            string tt = $"INSERT INTO [{Datos.Company}$Errores BC25] ([Fecha], [Tabla], [Clave], [Error]) VALUES ( GETDATE(), '{xTabla}', '{xClave}', '{Texto}')";
            if (!await s.Ejecutar_SQL(Datos, tt))
            {
                await Funciones.Log(Datos, "Error Insertar Errores");
            }

            await Funciones.Log(Datos, $"Error Migración {xTabla}   {xClave}\n {Texto}");

            return (true);
        }

        public static void Clear_Row(ref DataRow row, DataTable TableData)
        {
            int n = -1;

            foreach (DataColumn column in TableData.Columns)
            {
                n++;

                if (column.DataType == typeof(string))
                {
                    row[n] = row[n].ToString().Replace("'", " ");
                    row[n] = row[n].ToString().Replace(Convert.ToChar(34).ToString(), " ");
                }
            }
        }

        public static void Crear_Diccionario(ref Dictionary<string, object> rowAsDictionary, DataRow row, DataTable TableData)
        {
            rowAsDictionary = new Dictionary<string, object>();

            string columna = string.Empty;
            try
            {
                foreach (DataColumn column in TableData.Columns)
                {
                    columna = column.ColumnName;
                    if (!row.IsNull(column))
                    {
                        if (!Funciones.Columna_Vacia(row, column))
                        {
                            rowAsDictionary[Funciones.Nombres_Columnas(column.ColumnName)] = row[column].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = $"Error al crear diccionario de la fila {columna}: {ex.Message}";
            }
        }

        public static string Limpiar_Lineas_json(string xJson)
        {
            xJson = xJson.Replace(@"\r\n", "");
            //xJson = xJson.Replace(@"\", "");
            xJson = xJson.Replace(@"""UD"": ""[{ ", @"""UD"": [{ ");
            xJson = xJson.Replace(@"}]""", @"}]");
            xJson = xJson.Replace(@"""[{", @"[{");
            xJson = xJson.Replace(@"'", "");
            xJson = xJson.Replace(@"\\", @"/");

            return (xJson);
        }
    }
}
