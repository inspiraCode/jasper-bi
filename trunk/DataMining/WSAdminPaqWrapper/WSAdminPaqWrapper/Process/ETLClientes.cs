using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Miner;
using Npgsql;
using WSAdminPaqWrapper.Loader;

namespace WSAdminPaqWrapper.Process
{
    class ETLClientes
    {
        internal static void Execute(int idEmpresa, string empresa, List<CatCliente> clientes, NpgsqlConnection conn)
        {
            foreach (CatCliente client in clientes)
            {
                DimClientes dClient = DimClientes.GetCliente(idEmpresa, client.CodigoCliente, conn);
                bool loaded = dClient != null;
                if (loaded)
                    UpdateDim(empresa, client, dClient, conn);
                else
                    LoadDim(empresa, idEmpresa, client, conn);
                
            }

        }

        private static void UpdateDim(string empresa, CatCliente cat, DimClientes dim, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE dim_clientes " +
                "SET codigo_cliente=@codigo, " +
                "nombre_cliente=@nombre, " +
                "es_local=@local, " +
                "empresa=@empresa " +
                "WHERE id_cliente=@id;";
            

            cmd = new NpgsqlCommand(sqlString, conn);
            
            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 11);
            cmd.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
            cmd.Parameters.Add("@local", NpgsqlTypes.NpgsqlDbType.Boolean);
            cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@codigo"].Value = cat.CodigoCliente;
            cmd.Parameters["@nombre"].Value = cat.RazonSocial;
            cmd.Parameters["@local"].Value = cat.EsLocal;
            cmd.Parameters["@empresa"].Value = dim.Empresa;
            cmd.Parameters["@id"].Value = dim.IdCliente;

            cmd.ExecuteNonQuery();
        }

        private static void LoadDim(string empresa, int idEmpresa, CatCliente cat, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO dim_clientes(codigo_cliente, nombre_cliente, es_local, empresa, id_empresa)" +
                "VALUES(@codigo, @nombre, @local, @empresa, @idEmpresa);";


            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 11);
            cmd.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
            cmd.Parameters.Add("@local", NpgsqlTypes.NpgsqlDbType.Boolean);
            cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@codigo"].Value = cat.CodigoCliente;
            cmd.Parameters["@nombre"].Value = cat.RazonSocial;
            cmd.Parameters["@local"].Value = cat.EsLocal;
            cmd.Parameters["@empresa"].Value = empresa;
            cmd.Parameters["@idEmpresa"].Value = idEmpresa;

            cmd.ExecuteNonQuery();
        }
    }
}
