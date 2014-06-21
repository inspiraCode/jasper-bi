using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Miner;
using Npgsql;
using System.Configuration;
using System.Diagnostics;
using CommonAdminPaq;

namespace WSAdminPaqWrapper.Process
{
    public class Main
    {
        public static void Execute(EventLog log, AdminPaqLib lib)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            List<CatEmpresa> empresas = CatEmpresa.GetEmpresas(lib);
            log.WriteEntry(empresas.Count + " found empresas in AdminPaq.");
            List<FactCobranza> cobranzas = null;
            List<FactSales> sales = null;

            DeleteCollection(conn);
            DeleteSales(conn);

            foreach (CatEmpresa empresa in empresas)
            {
                log.WriteEntry("Downloading from AdminPaq: " + empresa.RutaEmpresa);
                // DIM ETLs
                List<CatCliente> clientes = CatCliente.GetClientes(empresa.RutaEmpresa);
                log.WriteEntry(clientes.Count + " clientes found for " + empresa.NombreEmpresa + " in AdminPaq");
                ETLClientes.Execute(empresa.IdEmpresa, empresa.NombreEmpresa, clientes, conn);

                List<CatSeller> sellers = CatSeller.GetSellers(empresa.RutaEmpresa);
                log.WriteEntry(sellers.Count + " agents found for " + empresa.NombreEmpresa + " in monfoll");
                ETLSellers.Execute(empresa.IdEmpresa, empresa.NombreEmpresa, sellers, conn);

                ETLMeses.Execute(conn);

                // FACT ETLs.
                FactVencido vencido = new FactVencido();
                vencido.Execute(empresa.IdEmpresa, empresa.RutaEmpresa, conn);

                FactPorVencer porVencer = new FactPorVencer();
                porVencer.Execute(empresa.IdEmpresa, empresa.RutaEmpresa, conn);

                FactCobranza entityCobranza = new FactCobranza();

                if (cobranzas == null)
                    cobranzas = FactCobranza.GetFactByEnterprise(empresa, conn, log);
                else
                    MergeCollection(ref cobranzas, FactCobranza.GetFactByEnterprise(empresa, conn, log));

                if (sales == null)
                    sales = FactSales.GetFactByEnterprise(empresa, conn, log);
                else
                    MergeSales(ref sales, FactSales.GetFactByEnterprise(empresa, conn, log));

            }

            if (cobranzas != null)
            {
                foreach (FactCobranza fact in cobranzas)
                {
                    FactUncollectable incobrable = new FactUncollectable(fact.Month);
                    fact.Uncollectable = incobrable.Uncollectable;
                    AddCollection(fact, conn);
                }
            }
            else 
            {
                log.WriteEntry("No information found in database.", EventLogEntryType.Warning);
            }

            if (sales != null)
            {
                foreach (FactSales sale in sales)
                {
                    AddSale(sale, conn);
                }
            }
            else 
            {
                log.WriteEntry("No information found in database for sales chart", EventLogEntryType.Warning);
            }

            conn.Close();
        }

        private static void DeleteSales(NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            // Remove last year old months
            string sqlString = "DELETE " +
                "FROM fact_sales;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.ExecuteNonQuery();
        }

        private static void DeleteCollection(NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            // Remove last year old months
            string sqlString = "DELETE " +
                "FROM fact_collection;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.ExecuteNonQuery();
        }

        private static void AddSale(FactSales fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_sales(seller_id, sold_today, sold_week, sold_month) " +
                "VALUES(@seller, @hoy, @semana, @mes);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@seller", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@hoy", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@semana", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@seller"].Value = fact.Seller.IdSeller;
            cmd.Parameters["@hoy"].Value = fact.SoldToday;
            cmd.Parameters["@semana"].Value = fact.SoldWeek;
            cmd.Parameters["@mes"].Value = fact.SoldMonth;

            cmd.ExecuteNonQuery();
        }

        private static void AddCollection(FactCobranza fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_collection(id_mes, vendido, cobrado, incobrable)" +
                "VALUES(@mes, @vendido, @cobrado, @incobrable);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@vendido", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@cobrado", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@incobrable", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@mes"].Value = fact.Month.IdMes;
            cmd.Parameters["@vendido"].Value = fact.Sold;
            cmd.Parameters["@cobrado"].Value = fact.Collected;
            cmd.Parameters["@incobrable"].Value = fact.Uncollectable;

            cmd.ExecuteNonQuery();
        }

        private static void MergeSales(ref List<FactSales> destination, List<FactSales> source)
        { 
            foreach(FactSales fact in destination)
            {
                FactSales sourceFact = source.Find(x => (x.Seller.IdSeller == fact.Seller.IdSeller));
                if (sourceFact != null)
                {
                    fact.SoldToday += sourceFact.SoldToday;
                    fact.SoldWeek += sourceFact.SoldWeek;
                    fact.SoldMonth += sourceFact.SoldMonth;
                }
            }
        }

        private static void MergeCollection(ref List<FactCobranza> destination, List<FactCobranza> source)
        {
            foreach (FactCobranza fact in destination)
            {
                FactCobranza sourceFact = source.Find(x => (x.Month.IdMes == fact.Month.IdMes));
                if (sourceFact != null)
                {
                    fact.Collected = fact.Collected + sourceFact.Collected;
                    fact.Sold = fact.Sold + sourceFact.Sold;
                    fact.Uncollectable = fact.Uncollectable + sourceFact.Uncollectable;
                }   
            }
        }

    }
}
