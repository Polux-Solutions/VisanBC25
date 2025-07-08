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
    public class AF
    {
        public async Task<m_Datos> Traspasar_Coste(m_Datos Datos, bool Borrar)
        {
            int Counter = 0;
            int CounterOK = 0;
            TimeSpan start = DateTime.Now.TimeOfDay;
            TimeSpan stop = DateTime.Now.TimeOfDay;

            Datos.Company = "VISAN Ind_ zootécnicas, S_L_";

            if (Borrar)
            {
                m_Borrar BorrarClass = new m_Borrar();

                string xjson = JsonConvert.SerializeObject(BorrarClass, Formatting.Indented);
                m_Respuesta RespuestaWs = new m_Respuesta();

                RespuestaWs = await oData.WsJson(Datos, xjson, "Borrar_Diario_AF");

                if (RespuestaWs.Ok)
                {
                    Console.WriteLine($"\r\n\r\nBorrados: {RespuestaWs.Respuesta} Registros");
                    await Funciones.Log(Datos, $"Borrar Diario AF: {RespuestaWs.Respuesta} Borrados");

                }
                else
                {
                    Console.WriteLine($"\r\n\r\nNo se han podido Borrar Registros AF: {RespuestaWs.Respuesta}");
                    await Funciones.Log(Datos, $"Error Borrando Diario AF: {RespuestaWs.Respuesta}");
                    Datos.Estado = false;
                    Datos.Error = $"No se han podido Borrar Registros Diario AF: {RespuestaWs.Respuesta}";
                    return Datos;
                }
            }


            string tt = $"SELECT [FA No_], [Fecha Ult Amortizacion BC], [Acquisition Date], [Last Acquisition Cost Date], [Last Depreciation Date], [Acquisition Cost], [Depreciation] " +
                        $" FROM [{Datos.Company}$Saldos amortiza Migrac] WHERE [Exportado BC25] IN (0)";

            Sql s = new Sql();
            await s.Cargar_TableData(Datos, tt);
            if (s.mSql.Estado)
            {
                try
                {
                    Console.WriteLine($"\r\n\r\n");

                    if (s.mSql.TableData.Rows.Count == 0) Console.WriteLine($"No hay Líneas de coste para transferir\r\n\r\n");

                    foreach (DataRow row in s.mSql.TableData.Rows)
                    {
                        Counter++;

                        Dictionary<string, object> rowAsDictionary = new Dictionary<string, object>();
                        Funciones.Crear_Diccionario(ref rowAsDictionary, row, s.mSql.TableData);

                        string xjson = JsonConvert.SerializeObject(rowAsDictionary, Formatting.Indented);
                        xjson = Funciones.Limpiar_Lineas_json(xjson);

                        m_Respuesta RespuestaWs = new m_Respuesta();
                        RespuestaWs = await oData.WsJson(Datos, xjson, "Diario_AF");
                        if (RespuestaWs.Ok)
                        {
                            CounterOK++;
                            await Marcar_Diario_AF_Exportado(Datos, s, row["FA No_"].ToString());
                        }
                        else
                            await Funciones.Insertar_Error(Datos, s, "Saldos amortiza Migrac", row["FA No_"].ToString(), RespuestaWs.Respuesta);


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


        private async Task<bool>Marcar_Diario_AF_Exportado(m_Datos Datos, Sql s, string xAF)
        {
            string tt = $@"UPDATE [{Datos.Company}$Saldos amortiza Migrac]  
                            SET [Exportado BC25] = 1
                            WHERE [FA No_] = '{xAF}'";

            if (!await s.Ejecutar_SQL(Datos, tt))
            {
                await Funciones.Log(Datos, $"Error Marcar Exportado {s.mSql.Error}");
            }

            return (true);
        }
    }
}
