using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using WSAdminPaqWrapper.Miner;
using Npgsql;
using System.Diagnostics;

namespace WSAdminPaqWrapper.Process
{
    public class FactSales : Factoreable
    {
        public DimSellers Seller { get; set; }
        public double SoldToday { get; set; }
        public double SoldWeek { get; set; }
        public double SoldMonth { get; set; }

        private List<FactSales> gruposVenta = new List<FactSales>();
        public List<FactSales> GruposVenta { get { return gruposVenta; } }

        public void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn)
        {
            if (gruposVenta.Count > 0)
                return;

            List<DimSellers> sellers = DimSellers.GetSellers(conn);

            foreach (DimSellers seller in sellers)
            {
                FactSales sale = new FactSales();
                sale.Seller = seller;
                sale.SoldMonth = 0;
                sale.SoldWeek = 0;
                sale.SoldToday = 0;

                gruposVenta.Add(sale);
            }
        }
    }
}
