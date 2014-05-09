using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Configuration;
using System.Data;

namespace WSAdminPaqWrapper.Loader
{
    public class DimClientes
    {
        public int IdCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public bool EsLocal { get; set; }
        public string Empresa { get; set; }
        public int IdEmpresa { get; set; }

        public static List<DimClientes> GetAll(int idEmpresa, NpgsqlConnection conn)
        {
            List<DimClientes> result = new List<DimClientes>();

            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente, codigo_cliente, nombre_cliente, es_local, empresa " +
                "FROM dim_clientes " +
                "WHERE id_empresa=@id;";

            if (conn == null || !(conn.State == ConnectionState.Open))
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Configuration.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                didOpenConnection = true;
            }

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = idEmpresa;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DimClientes cliente = new DimClientes();

                cliente.IdCliente = int.Parse(dr["id_cliente"].ToString());
                cliente.CodigoCliente = dr["codigo_cliente"].ToString();
                cliente.RazonSocial = dr["nombre_cliente"].ToString();
                cliente.EsLocal = bool.Parse(dr["es_local"].ToString());
                cliente.Empresa = dr["empresa"].ToString();
                cliente.IdEmpresa = idEmpresa;

                result.Add(cliente);
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }

        public static DimClientes GetClienteById(int idCliente, NpgsqlConnection conn)
        {
            DimClientes result = null;
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT codigo_cliente, nombre_cliente, es_local, empresa, id_empresa " +
                "FROM dim_clientes " +
                "WHERE id_cliente=@id;";

            if (conn == null || !(conn.State == ConnectionState.Open))
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Configuration.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                didOpenConnection = true;
            }

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = idCliente;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = new DimClientes();

                result.CodigoCliente = dr["codigo_cliente"].ToString();
                result.RazonSocial = dr["nombre_cliente"].ToString();
                result.EsLocal = bool.Parse(dr["es_local"].ToString());
                result.Empresa = dr["empresa"].ToString();
                result.IdEmpresa = int.Parse(dr["id_empresa"].ToString());
                result.IdCliente = idCliente;
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }

        public static DimClientes GetCliente(int idEmpresa, string codigoCliente, NpgsqlConnection conn)
        {
            DimClientes result = null;
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente, codigo_cliente, nombre_cliente, es_local, empresa, id_empresa " +
                "FROM dim_clientes " +
                "WHERE codigo_cliente=@cliente " +
                "AND id_empresa=@idEmpresa;";

            if (conn == null || !(conn.State == ConnectionState.Open))
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Configuration.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                didOpenConnection = true;
            }

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Varchar, 11);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@cliente"].Value = codigoCliente;
            cmd.Parameters["@idEmpresa"].Value = idEmpresa;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = new DimClientes();
                result.IdCliente = int.Parse(dr["id_cliente"].ToString());
                result.RazonSocial = dr["nombre_cliente"].ToString();
                result.EsLocal = bool.Parse(dr["es_local"].ToString());
                result.Empresa = dr["empresa"].ToString();

                result.CodigoCliente = codigoCliente;
                result.IdEmpresa = idEmpresa;
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }

    }
}
