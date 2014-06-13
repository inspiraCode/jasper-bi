using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Process;
using System.Configuration;
using System.Collections.Specialized;
using WSAdminPaqWrapper.Config;
using CommonAdminPaq;
using System.Globalization;
using Npgsql;
using System.Diagnostics;
using WSAdminPaqWrapper.Loader;

namespace WSAdminPaqWrapper.Miner
{
    public class SalesPicker
    {
        public const string TABLE_NAME = "MGW10008";
        public const string INDEX = "CFECHA";

        public const string CA_TABLE_NAME = "MGW10009";
        public const string CA_INDEX = "IDOCTOCA01";

        public List<FactCobranza> Facts { get; set; }
        public List<FactSales> Sales { get; set; }
        public CatEmpresa Empresa { get; set; }

        private List<string> conceptosCobro;
        private List<string> conceptosAbono;

        private Dictionary<int, string> currencies = new Dictionary<int, string>();
        private Dictionary<int, string> concepts = new Dictionary<int, string>();

        public void FillSellers(EventLog log)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning);
                return;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning);
                    continue;
                }

                if (Empresa.NombreEmpresa.Equals(clientConfig.NombreEmpresa))
                {
                    log.WriteEntry("Retrieving collection details for " + clientConfig.NombreEmpresa);
                    FillSellers(clientConfig.ConceptosVenta.Split(','),
                        clientConfig.ConceptosDevolucion.Split(','));
                }
            }
        }

        public void FillFacts(EventLog log)
        {

            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning);
                return;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection) System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null) 
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning);
                    continue;
                } 

                if (Empresa.NombreEmpresa.Equals(clientConfig.NombreEmpresa))
                {
                    log.WriteEntry("Retrieving collection details for " + clientConfig.NombreEmpresa);
                    conceptosCobro = ConceptosFactura();
                    conceptosAbono = ConceptosAbono();
                    FillFacts(clientConfig.ConceptosVenta.Split(','), 
                        clientConfig.ConceptosDevolucion.Split(','));
                }
            }
        }

        private void FillSellers(string[] codigosVenta, string[] codigosDevolucion)
        {
            int connDocos, dbResponse, fqResponse;

            DateTime today = DateTime.Today;
#if DEBUG
            today = new DateTime(2014, 4, 12);
#endif

            string sFromDate = today.AddMonths(-1).ToString("yyyyMM");
            
            int cancelado = 0, devuelto = 0, impreso = 0, conceptId = 0, currencyId = 0, idAgente = 0;
            double sold = 0, cambio = 0;
            string concept, currencyName, sFechaDoc;
            StringBuilder sbFechaDoc = new StringBuilder(9);

            bool esVenta = false, esDevolucion = false;

            connDocos = AdminPaqLib.dbLogIn("", Empresa.RutaEmpresa);
            if (connDocos == 0)
            {
                throw new Exception("Unable to work with route [" + Empresa.RutaEmpresa + "]");
            }

            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TABLE_NAME, INDEX, sFromDate);
            while (dbResponse == 0)
            {
                esVenta = false;
                esDevolucion = false;

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 26, ref cancelado);
                if (cancelado != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 27, ref devuelto);
                if (devuelto != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 3, ref conceptId);
                if (!concepts.ContainsKey(conceptId))
                {
                    concept = GetDocumentConceptCode(conceptId);
                    concepts.Add(conceptId, concept);
                }

                concept = concepts[conceptId];

                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                esVenta = codigosVenta.Contains(concept);
                esDevolucion = codigosDevolucion.Contains(concept);
                if (esVenta)
                {
                    fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 25, ref impreso);
                    if (impreso == 0)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                        continue;
                    }
                }


                if (!esVenta && !esDevolucion)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fqResponse = AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 31, ref sold);

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = GetCurrencyName(currencyId);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }

                fqResponse = AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 6, sbFechaDoc, 9);
                sFechaDoc = sbFechaDoc.ToString().Substring(0, 8).Trim();

                currencyName = currencies[currencyId];

                if (!currencyName.ToUpper().Contains("PESO"))
                {
                    fqResponse = AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 16, ref cambio);
                    sold = sold * cambio;
                }

                if (esDevolucion)
                    sold = sold * -1;

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 10, ref idAgente);

                FillSellers(idAgente, sFechaDoc, sold);

                dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
            }

            AdminPaqLib.dbLogOut(connDocos);
        }

        private void FillSellers(int idAgente, string sFechaDoc, double sold)
        {
            DateTime today = DateTime.Today;
#if DEBUG
            today = new DateTime(2014, 4, 12);
#endif
            //weekly       
            int weekStartDelta = 1 - (int)today.DayOfWeek;
            DateTime weekStart = today.AddDays(weekStartDelta);

            //Create FactSales object to hold sales by agent.
            foreach (FactSales sale in Sales)
            {
                DateTime saleDate;
                bool parsed = DateTime.TryParseExact(sFechaDoc, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out saleDate);

                if (!parsed) continue;
                if (sale.Seller.ApId == idAgente)
                {
                    if (saleDate.Month == today.Month)
                        sale.SoldMonth += sold;

                    if (saleDate.CompareTo(weekStart) >= 0)
                        sale.SoldWeek += sold;

                    if (saleDate.CompareTo(today) == 0)
                        sale.SoldToday += sold;

                    break;
                }
            }
        }

        private void FillFacts(string[] codigosVenta, string[] codigosDevolucion)
        {
            int connDocos, dbResponse, fqResponse;

            string sFromDate = DateTime.Today.AddYears(-1).ToString("yyyyMM");

            int cancelado = 0, devuelto = 0, impreso = 0, conceptId = 0, currencyId = 0;
            double sold = 0, cambio = 0;
            string concept, currencyName, sFechaDoc;
            StringBuilder sbFechaDoc = new StringBuilder(9);

            bool esVenta = false, esDevolucion = false, esPago = false;

            connDocos = AdminPaqLib.dbLogIn("", Empresa.RutaEmpresa);
            if (connDocos == 0)
            {
                throw new Exception("Unable to work with route [" + Empresa.RutaEmpresa + "]");
            }

            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TABLE_NAME, INDEX, sFromDate);
            while (dbResponse == 0)
            {
                esVenta = false;
                esDevolucion = false;
                esPago = false;

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 26, ref cancelado);
                if (cancelado != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 27, ref devuelto);
                if (devuelto != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 3, ref conceptId);
                if (!concepts.ContainsKey(conceptId))
                {
                    concept = GetDocumentConceptCode(conceptId);
                    concepts.Add(conceptId, concept);
                }

                concept = concepts[conceptId];

                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                esVenta = codigosVenta.Contains(concept);
                esDevolucion = codigosDevolucion.Contains(concept);
                esPago = conceptosAbono.Contains(concept);
                if (esVenta)
                {
                    fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 25, ref impreso);
                    if (impreso == 0)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                        continue;
                    }
                }  
                

                if (!esVenta && !esDevolucion && !esPago)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fqResponse = AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 31, ref sold);

                fqResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = GetCurrencyName(currencyId);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }

                fqResponse = AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 6, sbFechaDoc, 9);
                sFechaDoc = sbFechaDoc.ToString().Substring(0, 8).Trim();

                currencyName = currencies[currencyId];

                if (!currencyName.ToUpper().Contains("PESO"))
                {
                    fqResponse = AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 16, ref cambio);
                    sold = sold * cambio;
                }

                if (esDevolucion)
                    sold = sold * -1;

                if(esVenta || esDevolucion)
                    FillFact(sFechaDoc, sold);

                if (esPago)
                    FillFactPayment(sFechaDoc, sold);

                dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
            }

            AdminPaqLib.dbLogOut(connDocos);
        }

        
        private void FillFactPayment(string sDate, double collected)
        {
            foreach (FactCobranza fact in Facts)
            {
                DateTime saleDate;
                bool parsed = DateTime.TryParseExact(sDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out saleDate);

                if (!parsed) continue;
                if (saleDate.Month != (int)fact.Month.IndiceMes) continue;
                if (saleDate.Year != fact.Month.YYYY) continue;

                fact.Collected += collected;
                break;
            }
        }

        private void FillFact(string sDate, double sold)
        {
            foreach (FactCobranza fact in Facts)
            {
                DateTime saleDate;
                bool parsed = DateTime.TryParseExact(sDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out saleDate);

                if (!parsed) continue;
                if (saleDate.Month != (int)fact.Month.IndiceMes) continue;
                if (saleDate.Year != fact.Month.YYYY) continue;

                fact.Sold += sold;
            }
        }

        private List<string> ConceptosFactura()
        {
            List<string> conceptos = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);

            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT codigo_concepto " +
                "FROM cat_concepto " +
                "WHERE id_empresa=@id AND razon='COBRO';";

            conn.Open();

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = Empresa.IdEmpresa;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                conceptos.Add(dr["codigo_concepto"].ToString());
            }

            dr.Close();

            conn.Close();
            return conceptos;
        }

        private List<string> ConceptosAbono()
        {
            List<string> conceptos = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);

            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT codigo_concepto " +
                "FROM cat_concepto " +
                "WHERE id_empresa=@id AND razon='PAGO';";

            conn.Open();

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = Empresa.IdEmpresa;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                conceptos.Add(dr["codigo_concepto"].ToString());
            }

            dr.Close();

            conn.Close();
            return conceptos;
        }

        private string GetCurrencyName(int currencyId)
        {
            string response = null;
            int connCurrency, dbResponse, fqResponse;
            StringBuilder sbCurrencyName = new StringBuilder(61);
            string key, currencyName;

            connCurrency = AdminPaqLib.dbLogIn("", Empresa.RutaEmpresa);
            if (connCurrency == 0)
            {
                return response;
            }

            key = currencyId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connCurrency, "MGW10034", "PRIMARYKEY", key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connCurrency, "MGW10034", 6, sbCurrencyName, 61);
                currencyName = sbCurrencyName.ToString().Substring(0, 60).Trim();

                response = currencyName;
            }

            AdminPaqLib.dbLogOut(connCurrency);
            return response;
        }

        private string GetDocumentConceptCode(int conceptId)
        {
            string response = null;
            int connDocos, dbResponse, fqResponse;
            StringBuilder sConceptCode = new StringBuilder(31);
            string key, conceptCode;

            connDocos = AdminPaqLib.dbLogIn("", Empresa.RutaEmpresa);
            if (connDocos == 0)
            {
                return response;
            }

            key = conceptId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, "MGW10006", "PRIMARYKEY", key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connDocos, "MGW10006", 2, sConceptCode, 31);
                conceptCode = sConceptCode.ToString().Substring(0, 30).Trim();

                response = conceptCode;
            }

            AdminPaqLib.dbLogOut(connDocos);
            return response;
        }
    }
}
