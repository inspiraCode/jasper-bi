using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Process;
using Npgsql;
using WSAdminPaqWrapper.Loader;
using CommonAdminPaq;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Specialized;
using WSAdminPaqWrapper.Config;
using System.Globalization;

namespace WSAdminPaqWrapper.Miner
{
    public class DocsMiner
    {
        public List<FactVencido> Vencidos { get; set; }
        public List<FactPorVencer> PorVencer { get; set; }
        public List<FactCobranza> Cobranza { get; set; }
        public List<FactSales> Ventas { get; set; }

        public const string TABLE_NAME = "MGW10008";
        public const string INDEX = "CFECHA";

        public const string CA_TABLE_NAME = "MGW10009";
        public const string CA_INDEX = "IDOCTOCA01";

        public void Execute(CatEmpresa empresa, NpgsqlConnection conn, EventLog log)
        {
            int connDocos, dbResponse, fieldResponse;
            DateTime today = DateTime.Today;
            DateTime fromDate = today.AddYears(-1);
            DateTime toDate = today;
            DateTime dueDate = today;
            string sFromDate = fromDate.ToString("yyyyMMdd");

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, string> concepts = new Dictionary<int, string>();
            Dictionary<int, DimClientes> customers = new Dictionary<int, DimClientes>();

            int cancelado = 0, devuelto = 0, impreso = 0, conceptId = 0, companyId = 0, currencyId = 0, idAgente = 0;
            StringBuilder sbFechaDoc = new StringBuilder(9);
            StringBuilder sbFechaVto = new StringBuilder(9);
            string sFechaDoc, sFechaVto, companyCode, currencyName, concept;
            double saldo = 0, cambio = 0;

            bool esVenta = false, esDevolucion = false, esCredito = false, esPago = false;

            connDocos = AdminPaqLib.dbLogIn("", empresa.RutaEmpresa);
            if (connDocos == 0)
            {
                throw new Exception("Unable to work with route [" + empresa.RutaEmpresa + "]");
            }

            List<string> conceptosCobro = ConceptosFactura(empresa.IdEmpresa);
            List<string> conceptosAbono = ConceptosAbono(empresa.IdEmpresa);
            string[] codigosVenta = GetCodigosVenta(empresa, log);
            string[] codigosDevolucion = GetCodigosDevolucion(empresa, log);

            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TABLE_NAME, INDEX, sFromDate);

            while (dbResponse != 0 && fromDate.CompareTo(DateTime.Today) < 0)
            {
                fromDate = fromDate.AddDays(1);
                sFromDate = fromDate.ToString("yyyyMMdd");
                dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TABLE_NAME, INDEX, sFromDate);
            }

            log.WriteEntry("GENERANDO DATOS DE VENTAS A PARTIR DE LA FECHA: " + sFromDate);

            if(dbResponse != 0)
                log.WriteEntry("NO FUE POSIBLE ENCONTRAR NINGúN REGISTRO, LA BASE DE DATOS ANUNCIó EL ERROR: " 
                    + dbResponse.ToString(), EventLogEntryType.Warning);

            int cancelados = 0, devueltos = 0, otros = 0, no_impresos = 0, no_co = 0, no_cliente = 0, valido = 0;
            while (dbResponse == 0)
            {
                esVenta = false;
                esDevolucion = false;
                esCredito = false;
                esPago = false;

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 26, ref cancelado);
                if (cancelado != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    cancelados++;
                    continue;
                }

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 27, ref devuelto);
                if (devuelto != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    devueltos++;
                    continue;
                }

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 3, ref conceptId);
                if (!concepts.ContainsKey(conceptId))
                {
                    concept = GetDocumentConceptCode(conceptId, empresa.RutaEmpresa);
                    concepts.Add(conceptId, concept);
                }

                concept = concepts[conceptId];

                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    otros++;
                    continue;
                }

                esVenta = codigosVenta != null && codigosVenta.Contains(concept);
                esDevolucion = codigosDevolucion != null && codigosDevolucion.Contains(concept);
                esCredito = conceptosCobro.Contains(concept);
                esPago = conceptosAbono.Contains(concept);

                if (!esVenta && !esDevolucion && !esCredito && !esPago)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    otros++;
                    continue;
                }

                if (esVenta)
                {
                    AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 25, ref impreso);
                    if (impreso == 0)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                        no_impresos++;
                        continue;
                    }
                }

                if(esCredito)
                    AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 44, ref saldo);
                else
                    AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 31, ref saldo);

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = GetCurrencyName(currencyId, empresa.RutaEmpresa);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }

                currencyName = currencies[currencyId];

                if (!currencyName.ToUpper().Contains("PESO"))
                {
                    AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 16, ref cambio);
                    saldo = saldo * cambio;
                }

                if (esDevolucion)
                    saldo = saldo * -1;

                AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 6, sbFechaDoc, 9);
                sFechaDoc = sbFechaDoc.ToString().Substring(0, 8).Trim();
                
                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 10, ref idAgente);

                AdminPaqDocument doco = new AdminPaqDocument();
                doco.IsCredit = esCredito;
                doco.IsSale = esVenta || esDevolucion;
                doco.IsPayment = esPago;
                doco.Amount = saldo;
                doco.SellerId = idAgente;
                doco.DocumentDate = DateTime.ParseExact(sFechaDoc, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                // SI ES CREDITO NECESITO LA FECHA DE VTO.
                if (esCredito)
                {
                    AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 11, sbFechaVto, 9);
                    sFechaVto = sbFechaVto.ToString().Substring(0, 8).Trim();

                    doco.DueDate = DateTime.ParseExact(sFechaVto, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                    // detect client assignment
                    fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 7, ref companyId);

                    if (!customers.ContainsKey(companyId))
                    {
                        companyCode = GetCompanyCode(companyId, empresa.RutaEmpresa);

                        if (companyCode == null)
                        {
                            dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                            no_co++;
                            continue;
                        }
                        DimClientes cliente = DimClientes.GetCliente(empresa.IdEmpresa, companyCode, null);
                        if (cliente == null)
                        {
                            dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                            no_cliente++;
                            continue;
                        }
                        customers.Add(companyId, cliente);
                    }

                    DimClientes customer = customers[companyId];
                    doco.Client = customer;
                }

                OrganizeDoco(doco, log);
                valido++;

                dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
            }

            log.WriteEntry("Resumen de colección de datos: cancelados=" + cancelados + ";devueltos=" + devueltos + ";otros=" + otros + ";no_impresos=" + no_impresos + ";no_co=" + no_co + ";no_cliente=" + no_cliente + ";valido=" + valido);
            AdminPaqLib.dbLogOut(connDocos);
        }

        private void OrganizeDoco(AdminPaqDocument document, EventLog log)
        {
            if (document.IsCredit)
            {
                bool due = document.DueDate.CompareTo(DateTime.Today) < 0;
                if (due)
                    MergeDueCreditDocument(document);
                else
                    MergeNotDueCreditDocument(document);
            }

            if (document.IsPayment)
            {
                MergeCollected(document);
            }

            if (document.IsSale)
            {
                MergeSale(document, log);
            }

        }

        private void MergeSale(AdminPaqDocument document, EventLog log)
        {
            MergeAgentSale(document, log);
            MergeCollectedSale(document);
        }

        private void MergeAgentSale(AdminPaqDocument document, EventLog log)
        {
            bool found = false;
            int monthDeltaDays = (DateTime.Today.Day - 1)*-1;
            DateTime BOM = DateTime.Today.AddDays(monthDeltaDays);

            if (document.DocumentDate.CompareTo(BOM) < 0)
                return;

            int weekStartDelta = 1 - (int)DateTime.Today.DayOfWeek;
            DateTime weekStart = DateTime.Today.AddDays(weekStartDelta);

            foreach (FactSales sale in Ventas)
            {
                if (sale.Seller.ApId == document.SellerId)
                {
                    if (document.DocumentDate.Month == DateTime.Today.Month)
                        sale.SoldMonth += document.Amount;

                    if (document.DocumentDate.CompareTo(weekStart) >= 0)
                        sale.SoldWeek += document.Amount;

                    if (document.DocumentDate.CompareTo(DateTime.Today) == 0)
                        sale.SoldToday += document.Amount;

                    found = true;
                    break;
                }
            }
            if (!found)
                log.WriteEntry("No se encontró el agente de venta al cual pertenece uno de los registros: " + document.SellerId, EventLogEntryType.Warning);
        }

        public void MergeCollectedSale(AdminPaqDocument document)
        {
            foreach (FactCobranza fact in Cobranza)
            {
                if (document.DocumentDate.Month != (int)fact.Month.IndiceMes) continue;
                if (document.DocumentDate.Year != fact.Month.YYYY) continue;

                fact.Sold += document.Amount;
            }
        }

        private void MergeCollected(AdminPaqDocument document)
        {
            foreach (FactCobranza fact in Cobranza)
            {
                if (document.DocumentDate.Month != (int)fact.Month.IndiceMes) continue;
                if (document.DocumentDate.Year != fact.Month.YYYY) continue;

                fact.Collected += document.Amount;
                break;
            }
        }

        private void MergeDueCreditDocument(AdminPaqDocument document)
        {
            foreach (FactVencido fact in Vencidos)
            {
                if (fact.Cliente.CodigoCliente == document.Client.CodigoCliente && fact.Cliente.IdEmpresa == document.Client.IdEmpresa)
                {
                    int startFac = fact.GrupoVencimiento.Inicio == 0 ? 1 : fact.GrupoVencimiento.Inicio;
                    DateTime startFactDate = DateTime.Today.AddDays(-startFac);
                    DateTime endFactDate = DateTime.Today.AddDays(-fact.GrupoVencimiento.Fin);

                    bool InfiniteGroup = fact.GrupoVencimiento.Fin == 0;
                    bool BeforeGroupStartDate = document.DueDate.CompareTo(startFactDate) <= 0;
                    bool AfterGroupEndDate = document.DueDate.CompareTo(endFactDate) >= 0;

                    if ((InfiniteGroup && BeforeGroupStartDate)
                        || (AfterGroupEndDate && BeforeGroupStartDate))
                    {
                        fact.Saldo = fact.Saldo + document.Amount;
                    }
                }
            }
        }

        private void MergeNotDueCreditDocument(AdminPaqDocument document)
        {
            foreach (FactPorVencer fact in PorVencer)
            {
                if (fact.Cliente.CodigoCliente == document.Client.CodigoCliente && fact.Cliente.IdEmpresa == document.Client.IdEmpresa)
                {
                    DateTime startFactDate = DateTime.Today.AddDays(fact.GrupoVencimiento.Inicio);
                    DateTime endFactDate = DateTime.Today.AddDays(fact.GrupoVencimiento.Fin);

                    bool InfiniteGroup = fact.GrupoVencimiento.Fin == 0;
                    bool AfterGroupStartDate = document.DueDate.CompareTo(startFactDate) >= 0;
                    bool BeforeGroupEndDate = document.DueDate.CompareTo(endFactDate) <= 0;

                    if ((InfiniteGroup && AfterGroupStartDate)
                        || (BeforeGroupEndDate && AfterGroupStartDate))
                    {
                        fact.Saldo = fact.Saldo + document.Amount;
                    }
                }
            }
        }

        private string GetCompanyCode(int companyId, string filePath)
        {
            int connCompany, dbResponse, fqResponse;
            StringBuilder sbCompanyCode = new StringBuilder(31);
            string key, result = null;

            connCompany = AdminPaqLib.dbLogIn("", filePath);

            if (connCompany == 0) return null;

            key = companyId.ToString().PadLeft(11);

            dbResponse = AdminPaqLib.dbGetNoLock(connCompany, "MGW10002", "PRIMARYKEY", key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, "MGW10002", 2, sbCompanyCode, 31);
                result = sbCompanyCode.ToString().Substring(0, 30).Trim();
            }

            AdminPaqLib.dbLogOut(connCompany);
            return result;
        }

        private string GetCurrencyName(int currencyId, string filePath)
        {
            string response = null;
            int connCurrency, dbResponse, fqResponse;
            StringBuilder sbCurrencyName = new StringBuilder(61);
            string key, currencyName;

            connCurrency = AdminPaqLib.dbLogIn("", filePath);
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

        private List<string> ConceptosAbono(int empresaId)
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
            cmd.Parameters["@id"].Value = empresaId;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                conceptos.Add(dr["codigo_concepto"].ToString());
            }

            dr.Close();

            conn.Close();
            return conceptos;
        }

        private List<string> ConceptosFactura(int empresaId)
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
            cmd.Parameters["@id"].Value = empresaId;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                conceptos.Add(dr["codigo_concepto"].ToString());
            }

            dr.Close();

            conn.Close();
            return conceptos;
        }

        private string GetDocumentConceptCode(int conceptId, string rutaEmpresa)
        {
            string response = null;
            int connDocos, dbResponse, fqResponse;
            StringBuilder sConceptCode = new StringBuilder(31);
            string key, conceptCode;

            connDocos = AdminPaqLib.dbLogIn("", rutaEmpresa);
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

        private string[] GetCodigosVenta(CatEmpresa empresa, EventLog log)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 19, 3);
                return null;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 20, 3);
                return null;
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

                if (empresa.NombreEmpresa.Equals(clientConfig.NombreEmpresa))
                {
                    log.WriteEntry("Sale codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.ConceptosVenta + "]", EventLogEntryType.Information, 22, 3);
                    return clientConfig.ConceptosVenta.Split(',');
                }
            }

            return null;
        }

        private string[] GetCodigosDevolucion(CatEmpresa empresa, EventLog log)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 23, 4);
                return null;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 24, 4);
                return null;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 25, 4);
                    continue;
                }

                if (empresa.NombreEmpresa.Equals(clientConfig.NombreEmpresa))
                {
                    log.WriteEntry("Return codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.ConceptosDevolucion + "]", EventLogEntryType.Information, 26, 4);
                    return clientConfig.ConceptosDevolucion.Split(',');
                }
            }

            return null;
        }
    }

    public class AdminPaqDocument
    {
        public Boolean IsSale { get; set; }
        public Boolean IsCredit { get; set; }
        public Boolean IsPayment { get; set; }
        public double Amount { get; set; }
        public int SellerId { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime DueDate { get; set; }
        public DimClientes Client { get; set; }
    }

}
