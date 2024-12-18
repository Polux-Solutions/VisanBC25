using VisanBC25.Modelos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;
using System.Data.Common;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;
using System.Threading.Tasks.Sources;

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



        public static string Menu(ref m_Datos Datos)
        {
            Console.Clear();
            Console.WriteLine(" 0. Test");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(" 1. Formas y Términos de Pago");
            Console.WriteLine(" 2. Países");
            Console.WriteLine(" 3. Clientes");
            Console.WriteLine(" 4. Provedores");
            Console.WriteLine(" 5. Bancos");
            Console.WriteLine(" 6. Cuentas Contables");
            Console.WriteLine(" 7. Activos Fijos");
            Console.WriteLine(" 8. Dir. Envío");
            Console.WriteLine(" 9. Bancos Clientes");
            Console.WriteLine("10. Bancos Provedores");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("50. Diario");
            Console.WriteLine("51. Facturas Venta");
            Console.WriteLine("52. Facturas Compra");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("99. Salir");
            Console.WriteLine("-------------------------------------");
            Console.Write("Pulse Opción: ");
            string Opc = Console.ReadLine();

            switch (Opc)
            {
                case "99": return "SALIR";
                case "0": return "TEST";
                case "1": return "FPAGO";
                case "2": return "PAISES";
                case "3": return "CLIENTES";
                case "4": return "PROVEEDORES";
                case "5": return "BANCOS";
                case "6": return "CUENTAS";
                case "7": return "ACTIVOS";
                case "8": return "DIRECCIONES";
                case "9": return "BANCOS-CLI";
                case "10": return "BANCOS-PRO";
                case "50": return "DIARIO";
                case "51": return "FAC-V";
                case "52": return "FAC-C";
            }

            return "ERROR";
        }


        public static bool Leer_Parametros(ref m_Datos Datos)
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
                Datos.Password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"]; ;
                Datos.Company = System.Configuration.ConfigurationManager.AppSettings["COMPANY"]; ;
                Datos.CompanyWeb = System.Configuration.ConfigurationManager.AppSettings["COMPANY-WEB"]; ;
                Datos.Server = System.Configuration.ConfigurationManager.AppSettings["SERVER"]; ;
                Datos.Database = System.Configuration.ConfigurationManager.AppSettings["DATABASE"]; ;
                Datos.ServerWeb= System.Configuration.ConfigurationManager.AppSettings["SERVER-WEB"]; ;
                Datos.Puerto= int.Parse( System.Configuration.ConfigurationManager.AppSettings["PUERTO"]);
                Datos.Instancia = System.Configuration.ConfigurationManager.AppSettings["INSTANCIA"]; ;
                Datos.Codeunit = System.Configuration.ConfigurationManager.AppSettings["CODEUNIT"]; ;
                Datos.UserBC = System.Configuration.ConfigurationManager.AppSettings["USER-BC"]; ;
                Datos.PasswdBC = System.Configuration.ConfigurationManager.AppSettings["PASSWD-BC"]; ;
                Datos.DomainBC = System.Configuration.ConfigurationManager.AppSettings["DOMAIN-BC"]; ;
                Datos.Estado = true;
            }
            catch (Exception ex)
            {
                Datos.Estado = false;
                Datos.Error = $@"Error al leer parámetros iniciales: {ex.Message}";
            }
            return Datos.Estado;
        }


        public static async Task <string> Log(m_Datos Datos,  string Texto)
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
                if (! File.Exists(Datos.LogDetalle))
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

        public static async Task<string>Crear_Select(m_Datos Datos, string xTabla)
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
                    if (Campos.Length> 0) Campos += ",";
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

            switch (column.DataType.Name)
            {
                case "String": vacia = (row[column].ToString().Trim() == string.Empty);
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

            return vacia;
        }

        public static async Task<bool> Insertar_Error(m_Datos Datos, Sql s, string xTabla, string xClave, string Texto)
        {
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

            foreach(DataColumn column in TableData.Columns)
            {
                n++;

                if (column.DataType == typeof(string))
                {
                    row[n] = row[n].ToString().Replace("'", " ");
                    row[n] = row[n].ToString().Replace(Convert.ToChar(34).ToString(), " ");
                }
            }


        }

        public static void Crear_Diccionario(ref Dictionary<string, object> rowAsDictionary, DataRow row, DataTable TableData )
        {
            rowAsDictionary = new Dictionary<string, object>();

            foreach (DataColumn column in TableData.Columns)
            {
                if (!Funciones.Columna_Vacia(row, column))
                {
                    rowAsDictionary[Funciones.Nombres_Columnas(column.ColumnName)] = row[column];
                }
            }
        }

        public static string Limpiar_Lineas_json(string xJson)
        {
            xJson = xJson.Replace(@"\r\n", "");
            //xJson = xJson.Replace(@"\", "");
            xJson = xJson.Replace(@"""Lineas"": ""[{ ", @"""Lineas"": [{ ");
            xJson = xJson.Replace(@"}]""", @"}]");

            return (xJson);   
        }
    }
}
