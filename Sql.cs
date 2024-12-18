using VisanBC25.Modelos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Data;

namespace VisanBC25
{
    public class Sql
    {
        public m_SQL mSql = new m_SQL();


        private async Task<bool>Abrir_BBDD(m_Datos Datos)
        {

            if (mSql.SqlConn == null) mSql.SqlConn = new SqlConnection();

            if (mSql.SqlConn.State != ConnectionState.Open)
            {
                try
                {
                    string Cadena = $"Server={Datos.Server};database={Datos.Database};User Id={Datos.User};Password={Datos.Password}" +
                                    ";MultipleActiveResultSets=true;ConnectRetryCount=20;timeout=36000;Trust Server Certificate=true";

                    mSql.SqlConn = new SqlConnection(Cadena);
                    mSql.SqlConn.Open();
                }
                catch (Exception ex)
                {
                    mSql.Estado = false;
                    mSql.Error = $"Error apertura conexión Base de Datos: {ex.Message}";
                }
            }

            return (mSql.Estado); 
        }


        private async Task<bool> Cerrar_BBDD()
        {
            try
            {
               if (mSql.SqlConn != null)
                {
                    if (mSql.SqlConn.State != ConnectionState.Open) mSql.SqlConn.Close();
                }
            }
            catch( Exception Ex)
            {
                mSql.Estado = false;
                mSql.Error = "Error Cerrar conexión Base de Datos: " + Ex.Message;
            }

            return (mSql.Estado);
        }

        public async Task <bool> Crear_Datareader(m_Datos Datos, string tt)
        {

            await Abrir_BBDD(Datos);
            
            if (mSql.Estado)
            {
                try
                {
                    
                    SqlCommand oComm = new SqlCommand();
                    oComm.Connection = mSql.SqlConn;
                    oComm.CommandText = tt;
                    mSql.Reader = oComm.ExecuteReader();
                }
                catch (Exception ex)
                {
                    mSql.Estado = false;
                    mSql.Error = $@"Error al crear datareader: {ex.Message}
                                 Sql: { tt}";
                }
            }

            return (mSql.Estado);
        }

        public async Task<bool> Cargar_TableData(m_Datos Datos, string tt)
        {

            await Abrir_BBDD(Datos);

            if (mSql.Estado)
            {
                try
                {
                    SqlCommand oComm = new SqlCommand();
                    oComm.Connection = mSql.SqlConn;
                    oComm.CommandText = tt;
                    mSql.Reader = oComm.ExecuteReader();
                    mSql.TableData = new DataTable();
                    mSql.TableData.Load(mSql.Reader);
                    mSql.Reader.Close();
                }
                catch (Exception ex)
                {
                    mSql.Estado = false;
                    mSql.Error = $@"Error al crear TableData: {ex.Message}
                                 Sql: {tt}";
                }
            }

            if (mSql.Estado) mSql.Estado = await Cerrar_BBDD();

            return (mSql.Estado);
        }

        public async Task<bool> Ejecutar_SQL(m_Datos Datos, string tt)
        {

            await Abrir_BBDD(Datos);

            if (mSql.Estado)
            {
                try
                {
                    SqlCommand oComm = new SqlCommand();
                    oComm.Connection = mSql.SqlConn;
                    oComm.CommandText = tt;
                    oComm.ExecuteNonQuery();
                    oComm.Dispose();
                }
                catch (Exception ex)
                {
                    mSql.Estado = false;
                    mSql.Error = $@"Error al ejecutar SQL: {ex.Message}
                                 Sql: {tt}";
                }
            }

            if (mSql.Estado) mSql.Estado = await Cerrar_BBDD();

            return (mSql.Estado);
        }

        public async Task<bool> Crear_Dataset(m_Datos Datos, string tt)
        {

            await Abrir_BBDD(Datos);

            if (mSql.Estado)
            {
                try
                {
                    mSql.ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(tt, mSql.SqlConn);
                    adapter.Fill(mSql.ds);
                }
                catch (Exception ex)
                {
                    mSql.Estado = false;
                    mSql.Error = $@"Error al crear Dataset: {ex.Message}
                                 Sql: {tt}";
                }
            }

            if (mSql.Estado) mSql.Estado = await Cerrar_BBDD();

            return (mSql.Estado);
        }

    }
}
