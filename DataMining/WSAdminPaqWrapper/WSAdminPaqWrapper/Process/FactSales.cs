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
    public class FactSales
    {
        public DimSellers Seller { get; set; }
        public double SoldToday { get; set; }
        public double SoldWeek { get; set; }
        public double SoldMonth { get; set; }

        public static List<FactSales> GetFactByEnterprise(CatEmpresa empresa, NpgsqlConnection conn, EventLog log)
        {
            List<DimSellers> sellers = DimSellers.GetSellers(conn);
            List<FactSales> sales = new List<FactSales>();
            
            foreach(DimSellers seller in sellers)
            {
                FactSales sale = new FactSales();
                sale.Seller = seller;
                sale.SoldMonth = 0;
                sale.SoldWeek = 0;
                sale.SoldToday = 0;

                sales.Add(sale);
            }

            SalesPicker salesPicker = new SalesPicker();
            salesPicker.Empresa = empresa;
            salesPicker.Sales = sales;
            salesPicker.FillSellers(log);

            return salesPicker.Sales;
        }
    }
}
