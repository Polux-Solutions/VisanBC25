using VisanBC25.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;
using System.Net;
using System.Data;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Identity.Client;

namespace VisanBC25
{
    public class Maestros
    {
        public async Task<m_Datos> Traspasar_Maestros(m_Datos Datos, string Opcion, bool Borrar)
        {
            string tt = string.Empty;
            string tabla = string.Empty;  
            int tablaId = 0;
            string funcion = string.Empty;
            string clave1 = string.Empty;
            string clave2 = string.Empty;


            switch (Opcion)
            {
                case "TPAGO":
                    funcion = "Payment_Terms";
                    tabla = "Payment Terms";
                    clave1 = "Code";
                    tablaId = 3;
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "FPAGO":
                    funcion = "Payment_Method";
                    tabla = "Payment Method";
                    clave1 = "Code";
                    tablaId = 289;
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "VAT-SETUP":
                    funcion = "Vat_Posting_Setup";
                    tabla = "VAT Posting Setup";
                    tablaId = 325;
                    clave1 = "VAT Bus_ Posting Group";
                    clave2 = "VAT Prod_ Posting Group";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "FA-POST":
                    funcion = "FA_Posting_group";
                    tabla = "FA Posting Group";
                    tablaId = 5606;
                    clave1 = "Code";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "FA-BOOK":
                    funcion = "FA_Depreciation_Book";
                    tabla = "FA Depreciation Book";
                    tablaId = 5612;
                    clave1 = "FA No_";
                    clave2 = "Depreciation Book Code";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "PAISES": 
                    funcion = "Paises";
                    tabla = "Country_Region";
                    tablaId = 9;
                    clave1 = "Code";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "CLIENTES":
                    tabla = "Customer";
                    funcion = "Clientes";
                    tablaId = 18;
                    clave1 = "No_";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    //tt += $" WHERE [No_] = '40001'";
                    break;
                case "PROVEEDORES":
                    tabla = "Vendor";
                    funcion = "Proveedores";
                    tablaId = 23;
                    clave1 = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "BANCOS":
                    tabla = "Bank Account";
                    funcion = "Bancos";
                    clave1 = "No_";
                    tablaId = 270;
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "CUENTAS":
                    tabla = "G_L Account";
                    funcion = "Cuentas";
                    tablaId = 15;
                    clave1 = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "ACTIVOS":
                    tabla = "Fixed Asset";
                    tablaId = 5600;
                    funcion = "Activos";
                    clave1 = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "DIRECCIONES":
                    tabla = "Ship-to Address";
                    tablaId = 222;
                    funcion = "ShipTo";
                    clave1 = "Code";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    tt += $" WHERE EXISTS ( SELECT * FROM [{Datos.Company}$Customer] CU WHERE CU.[No_] = [Customer No_])";
                    break; ;
                case "BANCOS-CLI":
                    tabla = "Customer Bank Account";
                    tablaId = 287;
                    funcion = "Bancos_Clientes";
                    clave1 = "Code";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    tt += $" WHERE EXISTS ( SELECT * FROM [{Datos.Company}$Customer] CU WHERE CU.[No_] = [Customer No_])";
                    break;
                case "BANCOS-PRO":
                    tabla = "Vendor Bank Account";
                    tablaId = 288;
                    funcion = "Bancos_Proveedores";
                    clave1 = "Code";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    tt += $" WHERE EXISTS ( SELECT * FROM [{Datos.Company}$Vendor] CU WHERE CU.[No_] = [Vendor No_])";
                    break;
            }
            
            bool BorradoOK = true;

            if (Borrar)
            {
                m_Borrar BorrarClass = new m_Borrar();

                BorrarClass.TablaId = tablaId;
                
                string xjson = JsonConvert.SerializeObject(BorrarClass, Formatting.Indented);
                m_Respuesta RespuestaWs = new m_Respuesta();

                RespuestaWs = await oData.WsJson(Datos, xjson, "Borrar_Maestro");

                if (RespuestaWs.Ok)
                {
                    Console.WriteLine($"\r\n\r\nBorrados: {RespuestaWs.Respuesta} Registros");
                }
                else
                {
                    BorradoOK = false;
                    Console.WriteLine($"\r\n\r\nNo se han podido Borrar Registros: {RespuestaWs.Respuesta}");
                    Datos.Estado = BorradoOK;
                    Datos.Error = $"No se han podido Borrar Registros: {RespuestaWs.Respuesta}";
                }
            }

            if (BorradoOK)
            {

                Sql s = new Sql();
                await s.Cargar_TableData(Datos, tt);
                if (s.mSql.Estado)
                {
                    try
                    {
                        Console.WriteLine($"\r\n");
                        int Counter = 0;
                        int CounterOK = 0;
                        TimeSpan start = DateTime.Now.TimeOfDay;
                        TimeSpan stop = DateTime.Now.TimeOfDay;
                        Single Porcentaje = 0;
                        DataRow row = null;

                        for (int i = 0; i < s.mSql.TableData.Rows.Count; i++)
                        {
                            if (Counter % 100 == 0)
                            {
                                Porcentaje = (Counter * 100) / s.mSql.TableData.Rows.Count;
                                stop = DateTime.Now.TimeOfDay - start;
                                Console.Write($"\rRegistros: {Counter}/{s.mSql.TableData.Rows.Count}    {Porcentaje}%      OK:{CounterOK}   Error: {Counter - CounterOK}        {stop.Minutes}:{stop.Seconds}.{stop.Milliseconds}");
                            }

                            row = s.mSql.TableData.Rows[i];
                            Counter++;

                            Funciones.Clear_Row(ref row, s.mSql.TableData);

                            Dictionary<string, object> rowAsDictionary = new Dictionary<string, object>();
                            Funciones.Crear_Diccionario(ref rowAsDictionary, row, s.mSql.TableData);

                            string xjson = JsonConvert.SerializeObject(rowAsDictionary, Formatting.Indented);
                            m_Respuesta RespuestaWs = new m_Respuesta();

                            RespuestaWs = await oData.WsJson(Datos, xjson, funcion);

                            if (RespuestaWs.Ok) CounterOK++;
                            if (!RespuestaWs.Ok)
                            {
                                string Key = row[clave1].ToString();
                                if (clave2 != "") Key += $"/{row[clave2].ToString()}";
                                await Funciones.Insertar_Error(Datos, s, tabla, Key, RespuestaWs.Respuesta);
                            }
                        }
                        stop = DateTime.Now.TimeOfDay - start;
                        Console.WriteLine($"\r\nFin Transferencia Registros: {Counter}   OK: {CounterOK}  Error: {Counter - CounterOK}    Tiempo:{stop.Minutes}:{stop.Seconds}.{stop.Milliseconds}");
                    }
                    catch (Exception ex)
                    {
                        Datos.Estado = false;
                        Datos.Error = $"Error Traspaso {Opcion}: {ex.Message}";
                    }
                }
                else
                {
                    Datos.Estado = s.mSql.Estado;
                    Datos.Error = s.mSql.Error;
                }
            }

            return (Datos);
        }
    }
}
