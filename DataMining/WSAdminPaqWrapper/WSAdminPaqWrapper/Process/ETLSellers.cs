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

        }

        private static void LoadDim(string empresa, CatSeller cat, DimSellers dim, NpgsqlConnection conn)
        {

        }
    }
}
