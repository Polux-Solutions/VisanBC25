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
    public class Item
    {
        public async Task<m_Datos> Traspasar_Item(m_Datos Datos,  bool Borrar)
        {
            bool BorradoOK = true;

            if (Borrar)
            {
                m_Borrar BorrarClass = new m_Borrar();

                BorrarClass.TablaId = 27;
                
                string xjson = JsonConvert.SerializeObject(BorrarClass, Formatting.Indented);
                m_Respuesta RespuestaWs = new m_Respuesta();

                RespuestaWs = await oData.WsJson(Datos, xjson, "Borrar_Item");

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

                string tt = @$"SELECT   IT.[% por merma],IT.[Adicionar Cód_ Antioxidante],IT.[Adiciones],IT.[Agrupamiento],IT.[Allow Invoice Disc_],IT.[Allow Online Adjustment],IT.[Almacén Proveedor],IT.[Almacén reventa],
                                        IT.[Alquilable S_N],IT.[Alternative Item No_],IT.[Automatic Ext_ Texts],IT.[Base Unit of Measure],IT.[BigBag],IT.[Blocked],IT.[Budget Profit],IT.[Budget Quantity],IT.[Budgeted Amount],
                                        IT.[Bultos por Tipo envasado],ART.[Caducidad],IT.[Caducidad en meses],IT.[Caducidad en meses Antigua],IT.[Caducidad mínima],IT.[Caducidad_Minima],IT.[Cajas_Pale],IT.[Calidad Stock],
                                        IT.[Cantidad Reservada],IT.[Capas_Pale],ART.[Caras_Palet_Etiquetar],IT.[Cartón en base pale],IT.[Clasificación],IT.[Cliente Reventas],IT.[Cód_ actividad],IT.[Cód_ almacén],IT.[Cód_ almacén 2],
                                        IT.[Cód_ almacén Externo],IT.[Cód_ almacén Obsoleto],IT.[Cod_ Consum_ Envasado],IT.[Cod_ Consum_ Paletizado],IT.[Cód_ Etiqueta Caja],IT.[Cód_ Etiqueta Palet],IT.[Cód_ Etiqueta Saco],
                                        IT.[Cód_ Mat_ Prima Resultante],ART.[Cod_Etiq_Imprenta],ART.[Cod_Etiq_Imprenta_OLD],ART.[Cod_Fmt_Inyector],ART.[Cod_Formato],IT.[Codif_ Saco],IT.[Codigo EAN],IT.[Código elemento alquiler],
                                        IT.[Código envase],ART.[Código_envase],ART.[Codigo_Fabricante],ART.[Colores],IT.[Commission Group],IT.[Common Item No_],IT.[Contrato],IT.[Control],IT.[Control humedades en compra],
                                        IT.[Cost is Adjusted],IT.[Cost Regulation %],IT.[Coste Indirecto],IT.[Costes financieros],IT.[Costes manipulacion],IT.[Costing Method],IT.[Country_Region of Origin Code],
                                        IT.[Country_Region Purchased Code],IT.[Creado por PRESTO s_n],IT.[Created From Nonstock Item],IT.[Critical],IT.[Customer code],IT.[Descr_ esp_ Producto Web],IT.[Descripción ampliada],
                                        IT.[Description],IT.[Description 2],IT.[Digest],IT.[Discrete Order Quantity],IT.[Distribución],IT.[DUN 14],ART.[DUN14],IT.[Durability],IT.[Duty Code],IT.[Duty Due %],IT.[Duty Unit Conversion],
                                        ART.[EAN13],IT.[ECOEMBE],IT.[Envase],IT.[Envase de OLD],IT.[Envase en],IT.[Enviar a InLog],IT.[Enviar Codbar a Inlog],IT.[Es Antioxidante],IT.[Es Lata],IT.[Es Pack],IT.[Eti_Inyector],
                                        IT.[Etiqueta automática en saco],IT.[Etiqueta en Caja],IT.[Etiqueta en palet],IT.[Etiqueta manual],IT.[Etiqueta saco],ART.[EtiquetaEmbalajeManPath],ART.[EtiquetaEmbalajePath],
                                        ART.[EtiquetaPiezaPath],ART.[Etq_Caja],ART.[Etq_Inyector],ART.[Etq_Palet],ART.[Etq_Saco],IT.[Expiration Calculation],IT.[Exportar OL],IT.[Factor Porc_ Sacos],IT.[Fecha Actualiza Reventas],
                                        IT.[Fecha Cambio Caducidad],IT.[Fecha Obsoleto],IT.[Fecha Revisión],IT.[Fecha_Hora_Ult_Mod],IT.[Filtro Inventario],IT.[Flushing Method],ART.[Forma],IT.[Formato del mes],IT.[Formato Inyector],
                                        IT.[Fórmula],IT.[Fotomecánica],IT.[Freight Type],IT.[Gen_ Prod_ Posting Group],IT.[Global Dimension 1 Code],IT.[Global Dimension 2 Code],IT.[Gross Weight],IT.[Height],IT.[Humedad],IT.[Idioma],
                                        IT.[Idioma caducidad],ART.[Ignorar],IT.[Imprimir caducidad],IT.[Imprimir codificador de lote],IT.[Imprimir lote],IT.[Imprimir nº fabricante],IT.[Include Inventory],IT.[Indirect Cost %],
                                        IT.[Inventory Posting Group],IT.[Inventory Value Zero],IT.[Item Category Code],IT.[Item Disc_ Group],IT.[Item Tracking Code],IT.[Kg_Pale],ART.[Kg_Palet],IT.[Kg_Palet SGA],ART.[Kg_Saco],
                                        IT.[Kg_Saco SGA],IT.[kilos plastico],IT.[Kilos por caja],IT.[Kilos por envase],ART.[Kilos_caja],IT.[Last Counting Period Update],IT.[Last Date Modified],IT.[Last Direct Cost],
                                        IT.[Last Unit Cost Calc_ Date],IT.[Lead Time Calculation],IT.[Length],IT.[Lot Nos_],IT.[Lot Size],IT.[Low-Level Code],IT.[Manufacturer Code],IT.[Manufacturing Policy],IT.[Marca],
                                        IT.[Material envase],IT.[Maximum Inventory],IT.[Maximum Order Quantity],IT.[Mayor coste manipulación],ART.[Medidas],IT.[Medidas envase],IT.[Mermas],IT.[Minimum Order Quantity],
                                        IT.[Molienda (E_kg)],IT.[Mosaico],IT.[Muelle],IT.[Muestras],IT.[Net Weight],IT.[Next Counting Period],IT.[No Editable],IT.[No Inventariable],IT.[Nº Matriz],IT.[No pedir],
                                        IT.[No repercutible Imp Plast],IT.[Nº sacos_pale],IT.[No Visible en Web],IT.[No_],IT.[No_ 2],IT.[No_ Adhesivos Caja],IT.[No_ Adhesivos Palet],IT.[No_ Adhesivos Saco],IT.[No_ Series],
                                        IT.[Notas],IT.[Notas 1],IT.[Notas 2],IT.[Notas 3],IT.[Notas 4],IT.[Obsoleto],IT.[Order Multiple],IT.[Order Tracking Policy],IT.[Overhead Rate],IT.[Pale plastificado],IT.[PE],
                                        IT.[Phys Invt Counting Period Code],IT.[Plástico cubre palet],IT.[Porcent_ Antioxid_ (%)],IT.[Precio Compra],IT.[Precio de referencia],IT.[Precio formulación],IT.[Precio punto verde],
                                        IT.[Price Includes VAT],IT.[Price Unit Conversion],IT.[Price_Profit Calculation],IT.[Prioridad],IT.[Product Group Code],IT.[Production BOM No_],IT.[Producto Ecommerce],IT.[Producto EDI],
                                        IT.[Producto fabricado en Visan],IT.[Producto Web],IT.[Profit %],IT.[Programa Envasado Cb],IT.[Programa Envasado CS],IT.[Programa Envasado Sb],IT.[Programa Envasado U1],IT.[Programa Envasado U2],
                                        IT.[Programa Paletizado Cb],IT.[Programa Paletizado CS],IT.[Programa Paletizado Sb],IT.[Programa Paletizado U1],IT.[Programa Paletizado U2],IT.[Purch_ Unit of Measure],IT.[Put-away Template Code],
                                        IT.[Put-away Unit of Measure Code],IT.[Recargo de formulación],IT.[Reorder Cycle],IT.[Reorder Point],IT.[Reorder Quantity],IT.[Reordering Policy],IT.[Replenishment System],IT.[Replicado],
                                        IT.[Reserve],IT.[Revendido Completamente],IT.[Rolled-up Cap_ Overhead Cost],IT.[Rolled-up Capacity Cost],IT.[Rolled-up Material Cost],IT.[Rolled-up Mfg_ Ovhd Cost],
                                        IT.[Rolled-up Subcontracted Cost],IT.[Rounding Precision],IT.[Routing No_],ART.[Sacos_Caja],ART.[Sacos_Palet],IT.[Safety Lead Time],IT.[Safety Stock Quantity],IT.[Sales Unit of Measure],
                                        IT.[Scrap %],IT.[Search Description],IT.[Serial Nos_],IT.[Service Item Group],IT.[Shelf_Bin No_],IT.[Single-Level Cap_ Ovhd Cost],IT.[Single-Level Capacity Cost],IT.[Single-Level Material Cost],
                                        IT.[Single-Level Mfg_ Ovhd Cost],IT.[Single-Level Subcontrd_ Cost],IT.[Special Equipment Code],IT.[SQLCaducidad],IT.[SQLEnvaseDe],IT.[Standard Cost],IT.[Statistics Group],
                                        IT.[Stock seguridad BigBag],IT.[Stock Seguridad Fórmula],IT.[Stock Seguridad Ref],IT.[Subfamilia],IT.[Tariff No_],IT.[Tax Group Code],ART.[Tip_ producto],IT.[Tipo Adhesivo],IT.[Tipo cierre],
                                        IT.[Tipo de cierre envasado],IT.[Tipo de envasado],IT.[Tipo de envase envasado],IT.[Tipo envase],IT.[Tipo producto],IT.[Ultimo precio formulación],IT.[Unit Cost],IT.[Unit List Price],
                                        IT.[Unit Price],IT.[Unit Volume],IT.[Units per Parcel],IT.[Use Cross-Docking],ART.[Var_logistica],IT.[Variable logística],IT.[Varios (E_kg)],IT.[VAT Bus_ Posting Gr_ (Price)],
                                        IT.[VAT Prod_ Posting Group],IT.[Vendor Item No_],IT.[Vendor No_],IT.[Visado],IT.[Width]
                                FROM {Datos.Company}$Item IT inner join {Datos.Company}$Articulos ART on ART.[No_] = IT.[No_]";
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

                            RespuestaWs = await oData.WsJson(Datos, xjson, "Item");

                            if (RespuestaWs.Ok) CounterOK++;
                            if (!RespuestaWs.Ok)
                            {
                                string Key = row["No_"].ToString();
                                await Funciones.Insertar_Error(Datos, s, "Item", Key, RespuestaWs.Respuesta);
                            }
                        }
                        stop = DateTime.Now.TimeOfDay - start;
                        Console.WriteLine($"\r\nFin Transferencia Registros: {Counter}   OK: {CounterOK}  Error: {Counter - CounterOK}    Tiempo:{stop.Minutes}:{stop.Seconds}.{stop.Milliseconds}");
                    }
                    catch (Exception ex)
                    {
                        Datos.Estado = false;
                        Datos.Error = $"Error Traspaso Item: {ex.Message}";
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
