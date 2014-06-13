using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Miner;
using Npgsql;
using WSAdminPaqWrapper.Loader;

namespace WSAdminPaqWrapper.Process
{
    class ETLSellers
    {
        internal static void Execute(int idEmpresa, string empresa, List<CatSeller> sellers, NpgsqlConnection conn)
        { 
            foreach(CatSeller seller in sellers)
            {
                DimSellers dSeller = DimSellers.GetSellerByAdminPaqId(conn, seller.IdVendedor, idEmpresa);
                bool loaded = dSeller != null;
                if(loaded)
                    UpdateDim(empresa, seller, dSeller, conn);
                else
                    LoadDim(empresa, idEmpresa, seller, conn);
            }
        }

        private static void UpdateDim(string empresa, CatSeller cat, DimSellers dim, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE dim_sellers " +
                "SET agent_code=@codigo, " +
                "agent_name=@nombre " +
                "WHERE seller_id=@id;";


            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            cmd.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@codigo"].Value = cat.CodigoVendedor;
            cmd.Parameters["@nombre"].Value = cat.NombreVendedor;
            cmd.Parameters["@id"].Value = dim.IdSeller;

            cmd.ExecuteNonQuery();
        }

        private static void LoadDim(string empresa, int idEmpresa, CatSeller cat, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO dim_sellers (ap_id, agent_code, agent_name, empresa, id_empresa)" +
                "VALUES(@ap_id, @codigo, @nombre, @empresa, @idEmpresa);";


            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@ap_id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
            cmd.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@ap_id"].Value = cat.IdVendedor;
            cmd.Parameters["@codigo"].Value = cat.CodigoVendedor;
            cmd.Parameters["@nombre"].Value = cat.NombreVendedor;
            
            cmd.Parameters["@empresa"].Value = empresa;
            cmd.Parameters["@idEmpresa"].Value = idEmpresa;

            cmd.ExecuteNonQuery();
        }
    }
}
