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
    public class FacturasCompras
    {

        public async Task<m_Datos> Traspasar_Facturas_Compras(m_Datos Datos, bool Borrar)
        {
            int Counter = 0;
            int Counter2 = 0;
            int CounterOK = 0;
            TimeSpan start = DateTime.Now.TimeOfDay;
            TimeSpan stop = DateTime.Now.TimeOfDay;

            
            if (Borrar)
            {
                m_Borrar BorrarClass = new m_Borrar();

                string xjson = JsonConvert.SerializeObject(BorrarClass, Formatting.Indented);
                m_Respuesta RespuestaWs = new m_Respuesta();

                RespuestaWs = await oData.WsJson(Datos, xjson, "Borrar_Facturas_Compra");

                if (RespuestaWs.Ok)
                {
                    Console.WriteLine($"\r\n\r\nBorrados: {RespuestaWs.Respuesta} Registros");
                    await Funciones.Log(Datos, $"Facturas Venta: {RespuestaWs.Respuesta} Borradas");
                }
                else
                {
                    Console.WriteLine($"\r\n\r\nNo se han podido Borrar Registros: {RespuestaWs.Respuesta}");
                    await Funciones.Log(Datos, $"Error Borrando Facturas Compra: {RespuestaWs.Respuesta}");

                    Datos.Estado = false;
                    Datos.Error = $"No se han podido Borrar Registros: {RespuestaWs.Respuesta}";
                    return Datos;
                }
            }


            string tt = await Funciones.Crear_Select(Datos, "Purchase Header");
            tt += @$" WHERE [Document Type] in (2,3) AND [Exportado BC25] = 0 AND YEAR([Posting Date])>=2025
                     AND EXISTS ( SELECT * FROM [{Datos.Company}$Purchase Line]
                                    WHERE [Document Type] = [{Datos.Company}$Purchase Header].[Document Type] and [Document No_] = [{Datos.Company}$Purchase Header].[No_])";

            Sql s = new Sql();
            await s.Cargar_TableData(Datos, tt);
            if (s.mSql.Estado)
            {
                s.mSql.TableData.Columns.Add("Lineas", typeof(String));
                try
                {
                    DataRow row = null;

                    Console.WriteLine($"\r\n\r\n");

                    for (int i = 0; i < s.mSql.TableData.Rows.Count; i++)
                    {
                        Counter++;
                        row = s.mSql.TableData.Rows[i];

                        Funciones.Clear_Row(ref row, s.mSql.TableData);

                        Sql t = new Sql();
                        tt = await Funciones.Crear_Select(Datos, "Purchase Line");
                        tt += $" WHERE [Document Type] = '{row["Document Type"].ToString()}' and [Document No_] = '{row["No_"].ToString()}'";
                        await t.Cargar_TableData(Datos, tt);

                        string ArrayLineas = "[";

                        DataRow row2 = null;

                        for (int ii = 0; ii < t.mSql.TableData.Rows.Count; ii++)
                        {
                            Counter2++;

                            row2 = t.mSql.TableData.Rows[ii];

                            Dictionary<string, object> rowAsDictionary1 = new Dictionary<string, object>();
                            Funciones.Crear_Diccionario(ref rowAsDictionary1, row2, t.mSql.TableData);

                            string Elemento = string.Empty;
                            Elemento = JsonConvert.SerializeObject(rowAsDictionary1, Formatting.Indented);

                            if (ArrayLineas != "[") ArrayLineas += ",";
                            ArrayLineas += Elemento;
                        }
                        ArrayLineas += "]";

                        row["Lineas"] = ArrayLineas;

                        Dictionary<string, object> rowAsDictionary = new Dictionary<string, object>();
                        Funciones.Crear_Diccionario(ref rowAsDictionary, row, s.mSql.TableData);

                        string xjson = JsonConvert.SerializeObject(rowAsDictionary, Formatting.Indented);
                        xjson = Funciones.Limpiar_Lineas_json(xjson);

                        m_Respuesta RespuestaWs = new m_Respuesta();
                        RespuestaWs = await oData.WsJson(Datos, xjson, "Facturas_Compra");
                        if (RespuestaWs.Ok)
                        {
                            CounterOK++;
                            await Marcar_Documento_Exportado(Datos, s, int.Parse(row["Document Type"].ToString()), row["No_"].ToString());
                            await Funciones.Log_Detalle(Datos, "Factura Venta", row["No_"].ToString(), "OK", "");
                        }
                        else
                        {
                            await Funciones.Insertar_Error(Datos, s, "Sales Header", row["No_"].ToString(), RespuestaWs.Respuesta);
                            await Funciones.Log_Detalle(Datos, "Factura Venta", row["No_"].ToString(), "NOK", RespuestaWs.Respuesta);
                        }

                        stop = DateTime.Now.TimeOfDay - start;
                        Console.Write($"\rRegistros: {Counter}/{s.mSql.TableData.Rows.Count}  Líneas:{Counter2}        OK: {CounterOK}   Error: {Counter - CounterOK}    Tiempo: {stop.Hours}:{stop.Minutes}:{stop.Seconds}");
                    }
                }
                catch (Exception ex)
                {
                    Datos.Estado = false;
                    Datos.Error = $"Error Importación Facturas de Compras: {ex.Message}";
                }
            }
            else
            {
                Datos.Estado = s.mSql.Estado;
                Datos.Error = s.mSql.Error;
            }

            return (Datos);
        }


        private async Task<bool> Marcar_Documento_Exportado(m_Datos Datos, Sql s, int xDocumentType, string xFacturaNo)
        {
            string tt = $@"UPDATE [{Datos.Company}$Purchase Header]  
                            SET [Exportado BC25] = 1
                            WHERE [Document Type] = '{xDocumentType}'                             
                              AND [No_] = '{xFacturaNo}'";

            if (!await s.Ejecutar_SQL(Datos, tt))
            {
                await Funciones.Log(Datos, "Error Marcar Factura/Abono de Compras");
            }

            return (true);
        }



    }
}
