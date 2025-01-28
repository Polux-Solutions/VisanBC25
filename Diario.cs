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
using System.Diagnostics.Metrics;

namespace VisanBC25
{
    public class Diario
    {
        public async Task<m_Datos> Traspasar_Diario(m_Datos Datos, bool Saldos, bool Borrar)
        {
            int Counter = 0;
            int CounterOK = 0;
            TimeSpan start = DateTime.Now.TimeOfDay;
            TimeSpan stop = DateTime.Now.TimeOfDay;

            if (Saldos || Borrar)
            {
                m_Borrar BorrarClass = new m_Borrar();

                string xjson = JsonConvert.SerializeObject(BorrarClass, Formatting.Indented);
                m_Respuesta RespuestaWs = new m_Respuesta();

                if (Saldos)
                {
                    RespuestaWs = await oData.WsJson(Datos, xjson, "Borrar_Diario_Saldos");
                }
                else
                {
                    RespuestaWs = await oData.WsJson(Datos, xjson, "Borrar_Diario");
                }

                if (RespuestaWs.Ok)
                {
                    Console.WriteLine($"\r\n\r\nBorrados: {RespuestaWs.Respuesta} Registros");
                    await Funciones.Log(Datos, $"Borrar Diario: {RespuestaWs.Respuesta} Borrados");

                }
                else
                {
                    Console.WriteLine($"\r\n\r\nNo se han podido Borrar Registros: {RespuestaWs.Respuesta}");
                    await Funciones.Log(Datos, $"Error Borrando Diario (Saldos={Saldos}): {RespuestaWs.Respuesta}");
                    Datos.Estado = false;
                    Datos.Error = $"No se han podido Borrar Registros: {RespuestaWs.Respuesta}";
                    return Datos;
                }
            }


            string tt = await Funciones.Crear_Select(Datos, "Gen_ Journal Line");

            if (Saldos)
            {
                tt += $" WHERE [Exportado BC25] = 0 AND [Document No_] <> '' AND [Journal Batch Name] IN ('MIGRACION', 'MIGRACPROV')";
            }
            else
            {
                tt += $" WHERE [Exportado BC25] = 0 AND [Document No_] <> '' AND NOT [Journal Batch Name] IN ('MIGRACION', 'MIGRACPROV') AND YEAR([Posting Date])>=2025";
            }

            Sql s = new Sql();
            await s.Cargar_TableData(Datos, tt);
            if (s.mSql.Estado)
            {
                try
                {
                    Console.WriteLine($"\r\n\r\n");

                    foreach (DataRow row in s.mSql.TableData.Rows)
                    {
                        Counter++;

                        Dictionary<string, object> rowAsDictionary = new Dictionary<string, object>();
                        Funciones.Crear_Diccionario(ref rowAsDictionary, row, s.mSql.TableData);

                        string xjson = JsonConvert.SerializeObject(rowAsDictionary, Formatting.Indented);
                        m_Respuesta RespuestaWs = new m_Respuesta();
                        RespuestaWs = await oData.WsJson(Datos, xjson, "Diario");
                        if (RespuestaWs.Ok)
                        {
                            CounterOK++;
                            await Marcar_Diario_Exportado(Datos, s, row["Journal Template Name"].ToString(), row["Journal Batch Name"].ToString(), int.Parse(row["Line No_"].ToString()));
                        }
                        else
                            await Funciones.Insertar_Error(Datos, s, "Gen. Journal Line", row["Line No_"].ToString(), RespuestaWs.Respuesta);


                        if (Counter % 10 == 0)
                        {
                            await Task.Delay(1000);
                            stop = DateTime.Now.TimeOfDay - start;
                            Console.Write($"\rRegistros: {Counter}/{s.mSql.TableData.Rows.Count}  OK: {CounterOK}   Error: {Counter - CounterOK}    Tiempo: {stop.Hours}:{stop.Minutes}:{stop.Seconds}");
                        }
                    }

                    stop = DateTime.Now.TimeOfDay - start;
                    Console.WriteLine($"\r\nFin Transferencia Registros: {Counter}   OK: {CounterOK}  Error: {Counter - CounterOK}    Tiempo:{stop.Minutes}:{stop.Seconds}.{stop.Milliseconds}");

                }
                catch (Exception ex)
                {
                    Datos.Estado = false;
                    Datos.Error = $"Error Importación Diario: {ex.Message}";
                }
            }
            else
            {
                Datos.Estado = s.mSql.Estado;
                Datos.Error = s.mSql.Error;
            }

            return (Datos);
        }


        private async Task<bool>Marcar_Diario_Exportado(m_Datos Datos, Sql s, string xTemplate, string xBatch, int xLinea)
        {
            string tt = $@"UPDATE [{Datos.Company}$Gen_ Journal Line]  
                            SET [Exportado BC25] = 1
                            WHERE [Journal Template Name] = '{xTemplate}'                             
                              AND [Journal Batch Name] = '{xBatch}' 
                              AND [Line No_] = {xLinea.ToString()}";

            if (!await s.Ejecutar_SQL(Datos, tt))
            {
                await Funciones.Log(Datos, "Error Insertar Errores");
            }

            return (true);
        }
    }
}
