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
        public async Task<m_Datos> Traspasar_Diario(m_Datos Datos)
        {
            int Counter = 0;
            int CounterOK = 0;
            TimeSpan start = DateTime.Now.TimeOfDay;
            TimeSpan stop = DateTime.Now.TimeOfDay;

            string tt = await Funciones.Crear_Select(Datos, "Gen_ Journal Line");

            tt += $" WHERE [Exportado BC25] = 0";

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
                            Console.Write($"\r  Registros: {Counter}/{s.mSql.TableData.Rows.Count}  OK: {CounterOK}   Error: {Counter - CounterOK}    Tiempo: {stop.Hours}:{stop.Minutes}:{stop.Seconds}");
                        }
                    }
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
