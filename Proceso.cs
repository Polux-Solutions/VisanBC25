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
    public class Proceso
    {
        public async Task<bool> Bucle(m_Datos Datos, List<string> Opciones)
        {
            foreach (string Opcion in Opciones)
            {
                switch (Opcion)
                {
                    case "TEST":
                        Datos = await Test(Datos);
                        break;
                    case "FPAGO":
                        await Traspasar(Datos, "TPAGO");
                        await Traspasar(Datos, "FPAGO");
                        break;
                    case "PAISES" or "CLIENTES" or "PROVEEDORES" or "BANCOS" or "ACTIVOS" or "CUENTAS" or "DIRECCIONES" or "BANCOS-CLI" or "BANCOS-PRO":
                        await Traspasar(Datos, Opcion);
                        break;
                    case "DIARIO":
                        Diario d = new Diario();
                        await d.Traspasar_Diario(Datos);
                        break;
                    case "FAC-V":
                        FacturasVentas e = new FacturasVentas();
                        await e.Traspasar_Facturas_Ventas(Datos);
                        break;
                    case "FAC-C":
                        FacturasCompras f = new FacturasCompras();
                        await f.Traspasar_Facturas_Compras(Datos);
                        break;
                }

                if (!Datos.Estado)
                {
                    string EstadoLog = await Funciones.Log(Datos, Datos.Error);
                    if (EstadoLog != "OK") Console.WriteLine(EstadoLog);
                }
            }

            return (Datos.Estado);
        }


        private async Task<m_Datos> Test(m_Datos Datos)
        {
            m_Respuesta RespuestaWs = new m_Respuesta();
            RespuestaWs = await oData.WsJson(Datos, "", "Test");

            Console.Out.WriteLine($"\n\nEstado: {RespuestaWs.Ok}\n\n");
            if (!RespuestaWs.Ok) Console.Out.WriteLine($"Error: {RespuestaWs.Respuesta}\n\n");
            return (Datos);
        }

        private async Task<m_Datos> Traspasar(m_Datos Datos, string Opcion)
        {
            string tt = string.Empty;
            string tabla = string.Empty;  
            string funcion = string.Empty;
            string clave = string.Empty;

            switch (Opcion)
            {
                case "TPAGO":
                    funcion = "Payment_Terms";
                    tabla = "Payment Terms";
                    clave = "Code";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "FPAGO":
                    funcion = "Payment_Method";
                    tabla = "Payment Method";
                    clave = "Code";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "PAISES": 
                    funcion = "Paises";
                    tabla = "Country_Region";
                    clave = "Code";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "CLIENTES":
                    tabla = "Customer";
                    funcion = "Clientes";
                    clave = "No_";
                    tt = await Funciones.Crear_Select(Datos, tabla);
                    //tt += $" WHERE [No_] = '40001'";
                    break;
                case "PROVEEDORES":
                    tabla = "Vendor";
                    funcion = "Proveedores";
                    clave = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "BANCOS":
                    tabla = "Bank Account";
                    funcion = "Bancos";
                    clave = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "CUENTAS":
                    tabla = "G_L Account";
                    funcion = "Cuentas";
                    clave = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "ACTIVOS":
                    tabla = "Fixed Asset";
                    funcion = "Activos";
                    clave = "No_";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "DIRECCIONES":
                    tabla = "Ship-to Address";
                    funcion = "ShipTo";
                    clave = "Code";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "BANCOS-CLI":
                    tabla = "Customer Bank Account";
                    funcion = "Bancos_Clientes";
                    clave = "Code";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
                case "BANCOS-PRO":
                    tabla = "Vendor Bank Account";
                    funcion = "Bancos_Proveedores";
                    clave = "Code";

                    tt = await Funciones.Crear_Select(Datos, tabla);
                    break;
            }

            Sql s = new Sql();
            await s.Cargar_TableData(Datos, tt);
            if (s.mSql.Estado)
            {
                try
                {
                    Console.WriteLine($"\r\n\r\n");
                    int Counter = 0;
                    int CounterOK = 0;
                    TimeSpan start = DateTime.Now.TimeOfDay;
                    TimeSpan stop = DateTime.Now.TimeOfDay;
                    Single Porcentaje = 0;
                    DataRow row = null;
                    
                    for (int i = 0; i < s.mSql.TableData.Rows.Count; i++)
                    {
                        row = s.mSql.TableData.Rows[i];
                        Counter++;

                        Funciones.Clear_Row(ref row, s.mSql.TableData);

                        if (Counter % 100 == 0)
                        {
                            Porcentaje = (Counter * 100) / s.mSql.TableData.Rows.Count;
                            stop = DateTime.Now.TimeOfDay- start;
                            Console.Write($"\r  Registros: {Counter}/{s.mSql.TableData.Rows.Count}    {Porcentaje}%      OK:{CounterOK}   Error: {Counter-CounterOK}        {stop.Minutes}:{stop.Seconds}.{stop.Milliseconds}");
                        }

                        Dictionary<string, object> rowAsDictionary = new Dictionary<string, object>();
                        Funciones.Crear_Diccionario(ref rowAsDictionary, row, s.mSql.TableData);

                        string xjson = JsonConvert.SerializeObject(rowAsDictionary, Formatting.Indented);
                        m_Respuesta RespuestaWs = new m_Respuesta();

                        RespuestaWs = await oData.WsJson(Datos, xjson, funcion);

                        if (RespuestaWs.Ok) CounterOK++;
                        if (!RespuestaWs.Ok) await Funciones.Insertar_Error(Datos, s, tabla, row[clave].ToString(), RespuestaWs.Respuesta);
                    }
                    stop = DateTime.Now.TimeOfDay - start;
                    Console.WriteLine($"\r\nFin Transferencia Registros: {Counter}   OK: {CounterOK}  Error: {Counter-CounterOK}    Tiempo:{stop.Minutes}:{stop.Seconds}.{stop.Milliseconds}");
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

            return (Datos);
        }
    }
}
