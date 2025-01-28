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
                bool Borrar = false;
                string Comando = Opcion;

                if (Comando.Substring(Comando.Length - 1, 1) == "+")
                {
                    Borrar = true;
                    Comando = Comando.Substring(0, Comando.Length - 1);
                }

                switch (Comando)
                {
                    case "TEST":
                        Datos = await Test(Datos);
                        break;
                    case "FPAGO":
                        Maestros m = new Maestros();

                        await m.Traspasar_Maestros(Datos, "TPAGO", Borrar);
                        await m.Traspasar_Maestros(Datos, "FPAGO", Borrar);
                        break;
                    case "PAISES" or "CLIENTES" or "PROVEEDORES" or "BANCOS" or "ACTIVOS" or "CUENTAS" or "DIRECCIONES" or "BANCOS-CLI"
                        or "BANCOS-PRO" or "VAT-SETUP" or "FA-POST" or "FA-BOOK":

                        Maestros n = new Maestros();
                        await n.Traspasar_Maestros(Datos, Comando, Borrar);
                        break;
                    case "DIARIO":
                        Diario d = new Diario();
                        await d.Traspasar_Diario(Datos, false, Borrar);
                        break;
                    case "SALDOS":
                        Diario s = new Diario();
                        await s.Traspasar_Diario(Datos, true, Borrar);
                        break;
                    case "FAC-V":
                        FacturasVentas e = new FacturasVentas();
                        await e.Traspasar_Facturas_Ventas(Datos, Borrar);
                        break;
                    case "FAC-C":
                        FacturasCompras f = new FacturasCompras();
                        await f.Traspasar_Facturas_Compras(Datos, Borrar);
                        break;
                    case "REMESA":
                        Cartera g = new Cartera();
                        await g.Traspasar_Remesas(Datos, Borrar);
                        break;
                    case "OPAGO":
                        Cartera h = new Cartera();
                        await h.Traspasar_Ordenes_Pago(Datos, Borrar);
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
    }
}
