using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Miner;
using Npgsql;
using System.Configuration;
using System.Diagnostics;
using CommonAdminPaq;
using WSAdminPaqWrapper.Loader;
using System.Collections.Specialized;
using WSAdminPaqWrapper.Config;

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
            log.WriteEntry(empresas.Count + " found empresas in AdminPaq.", EventLogEntryType.Information, 5, 2);
            
            DeleteCollection(conn);
            log.WriteEntry("Cleaning of collection records succeed..", EventLogEntryType.Information, 6, 2);
            DeleteSales(conn);
            log.WriteEntry("Cleaning of sale records succeed..", EventLogEntryType.Information, 7, 2);

            foreach (CatEmpresa empresa in empresas)
            {
                if (!IsEnterpriseConfigured(log, empresa.NombreEmpresa)) continue;

                log.WriteEntry("Downloading from AdminPaq: " + empresa.RutaEmpresa, EventLogEntryType.Information, 8, 2);
                // DIM ETLs
                List<CatCliente> clientes = CatCliente.GetClientes(empresa.RutaEmpresa);
                log.WriteEntry(clientes.Count + " clientes found for " + empresa.NombreEmpresa + " in AdminPaq", EventLogEntryType.Information, 9, 2);
                ETLClientes.Execute(empresa.IdEmpresa, empresa.NombreEmpresa, clientes, conn);

                List<CatSeller> sellers = CatSeller.GetSellers(empresa.RutaEmpresa);
                log.WriteEntry(sellers.Count + " agents found for " + empresa.NombreEmpresa + " in monfoll", EventLogEntryType.Information, 10, 2);
                ETLSellers.Execute(empresa.IdEmpresa, empresa.NombreEmpresa, sellers, conn);

                ETLMeses.Execute(conn);

                // FACT Preparation
                FactVencido vencido = new FactVencido();
                vencido.Prepare(empresa.IdEmpresa, empresa.RutaEmpresa, conn);
                log.WriteEntry(string.Format("Prepared due documents for {0}", empresa.NombreEmpresa), EventLogEntryType.Information, 11, 2);

                FactPorVencer porVencer = new FactPorVencer();
                porVencer.Prepare(empresa.IdEmpresa, empresa.RutaEmpresa, conn);
                log.WriteEntry(string.Format("Prepared documents about to due for {0}", empresa.NombreEmpresa), EventLogEntryType.Information, 12, 2);

                FactCobranza cobranza = new FactCobranza();
                cobranza.Prepare(empresa.IdEmpresa, empresa.RutaEmpresa, conn);
                log.WriteEntry(string.Format("Prepared collection documents for {0}", empresa.NombreEmpresa), EventLogEntryType.Information, 13, 2);

                FactSales factSale = new FactSales();
                factSale.Prepare(empresa.IdEmpresa, empresa.RutaEmpresa, conn);
                log.WriteEntry(string.Format("Prepared sale documents for {0}", empresa.NombreEmpresa), EventLogEntryType.Information, 14, 2);

                // FILL FACTS
                DocsMiner dMiner = new DocsMiner();
                dMiner.Vencidos = vencido.GruposVencimiento;
                dMiner.PorVencer = porVencer.GruposVencimiento;
                dMiner.Cobranza = cobranza.GruposCobranza;
                dMiner.Ventas = factSale.GruposVenta;

                log.WriteEntry(string.Format("Mining documents for {0} started", empresa.NombreEmpresa), EventLogEntryType.Information, 15, 2);
                dMiner.Execute(empresa, conn, log);
                log.WriteEntry(string.Format("Mining documents for {0} completed", empresa.NombreEmpresa), EventLogEntryType.Information, 16, 2);

                MainLoader loader = new MainLoader();
                loader.Vencidos = dMiner.Vencidos;
                loader.PorVencer = dMiner.PorVencer;
                loader.Cobranza = dMiner.Cobranza;
                loader.Ventas = dMiner.Ventas;

                log.WriteEntry(string.Format("Loading documents for {0} started", empresa.NombreEmpresa), EventLogEntryType.Information, 17, 2);
                loader.Load(empresa.IdEmpresa, conn);
                log.WriteEntry(string.Format("Loading documents for {0} completed", empresa.NombreEmpresa), EventLogEntryType.Information, 18, 2);
            }

            conn.Close();
        }

        private static bool IsEnterpriseConfigured(EventLog log, string enterpriseName)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 19, 3);
                return false;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 20, 3);
                return false;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 21, 3);
                    continue;
                }

                if (clientConfig.NombreEmpresa.Equals(enterpriseName))
                {
                    return true;
                }
            }

            return false;
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
    }
}
