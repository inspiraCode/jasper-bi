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
                    LoadDim(empresa, seller, dSeller, conn);
            }
        }

        private static void UpdateDim(string empresa, CatSeller cat, DimSellers dim, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE dim_sellers " +
                "SET agent_code=@codigo, " +
                "agent_name=@nombre " +
                "WHERE ap_id=@id " +
                "AND id_empresa=@empresa;";


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

        private static void LoadDim(string empresa, CatSeller cat, DimSellers dim, NpgsqlConnection conn)
        {

        }
    }
}
