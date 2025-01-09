using VisanBC25.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace VisanBC25
{
    public class oData
    {
        public static async Task<m_Respuesta> WsJson(m_Datos Datos, string xJson, string xFuncion)
        {
            m_Respuesta respuestaWs = new m_Respuesta();
            respuestaWs.Ok = false;

            string miJson = string.Empty;
            
            if (xJson != string.Empty) miJson = "{'xjson':'" + xJson + "'}";

            try
            {
                HttpWebRequest request = await CreateWebRequestOData(Datos, xFuncion);

                if (!string.IsNullOrEmpty(miJson))
                {
                    byte[] bodyBytes = Encoding.UTF8.GetBytes(miJson);
                    request.ContentLength = bodyBytes.Length;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                    }
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {

                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string ContentString = rd.ReadToEnd();

                        JObject rss = JObject.Parse(ContentString);
                        string resultado = (string)rss["value"];
                        respuestaWs.Ok = true;
                        respuestaWs.Respuesta = resultado;
                        return respuestaWs;
                    }
                }
            }
            catch (WebException ex)
            {
                string exOriginal = ex.Message;

                try
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {

                        string sRespuesta = reader.ReadToEnd();
                        dynamic jsonObject = JsonConvert.DeserializeObject(sRespuesta);
                        string errorMessage = jsonObject.error.message;
                        respuestaWs.Respuesta = Funciones.Quitar_Correlation(errorMessage);
                    }
                }
                catch (Exception)
                {
                    respuestaWs.Respuesta = exOriginal;
                }

            }
            catch (Exception ex)
            {
                respuestaWs.Respuesta = ex.Message;
            }

            return respuestaWs;
        }



        private static async Task<HttpWebRequest> CreateWebRequestOData(m_Datos Datos, string xFuncion)
        {
            string CadenaConexion = $@"http://{Datos.ServerWeb}:{Datos.Puerto}/{Datos.Instancia}/ODataV4/{Datos.Codeunit}_{xFuncion}?company={Datos.CompanyWeb}";

            NetworkCredential credentials = new NetworkCredential(Datos.UserBC, Datos.PasswdBC, Datos.DomainBC);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(CadenaConexion);
            webRequest.Credentials = credentials;
            webRequest.ContentType = "application/json;charset=\"utf-8\"";
            webRequest.Accept = "application/json";
            webRequest.Method = "POST";
            webRequest.Timeout = 60000;
            return webRequest;
        }
    }
}
