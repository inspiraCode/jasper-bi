using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using Npgsql;
using WSAdminPaqWrapper.Miner;
using System.Data;
using System.Configuration;

namespace WSAdminPaqWrapper.Process
{
    public class FactCobranza
    {
        public DimMeses Month { get; set; }
        public double Sold { get; set; }
        public double Collected { get; set; }
        public double Uncollectable { get; set; }


        internal static List<FactCobranza> GetFactByEnterprise(CatEmpresa empresa, NpgsqlConnection conn)
        {
            List<DimMeses> meses = DimMeses.GetMeses(conn);
            List<FactCobranza> cobranzas = new List<FactCobranza>();

            foreach(DimMeses mes in meses)
            {
                FactCobranza cobranza = new FactCobranza();
                cobranza.Month = mes;
                cobranza.Sold = 0;
                cobranza.Collected = 0;
                cobranza.Uncollectable = 0;

                cobranzas.Add(cobranza);
            }

            SalesPicker sales = new SalesPicker();
            sales.Empresa = empresa;
            sales.Facts = cobranzas;
            sales.FillFacts();

            return sales.Facts;
        }
    }
}
