using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Process;
using CommonAdminPaq;
using System.Configuration;
using Npgsql;
using WSAdminPaqWrapper.Loader;
using System.Globalization;

namespace WSAdminPaqWrapper.Miner
{
    public class DueBalances : BaseBalance
    {

        public override void FillFacts(ref List<FactVencimiento> facts, string rutaEmpresa, int empresaId)
        {
            int connDocos, dbResponse, fieldResponse;
            DateTime today = DateTime.Today;
            DateTime fromDate = today.AddYears(-1);
            DateTime toDate = today.AddDays(-1);
            DateTime dueDate = today;
            string sFromDate = fromDate.ToString("yyyyMMdd");
            string sToDate = toDate.ToString("yyyyMMdd");

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, string> concepts = new Dictionary<int, string>();
            Dictionary<int, DimClientes> customers = new Dictionary<int, DimClientes>();

            int cancelled = 0, conceptId = 0, companyId = 0, currencyId = 0;
            StringBuilder sbFechaVenc = new StringBuilder(9);
            string sFechaVenc, companyCode, currencyName, concept;
            double saldo = 0, cambio = 0;

            connDocos = AdminPaqLib.dbLogIn("", rutaEmpresa);
            if (connDocos == 0)
            {
                throw new Exception("Unable to work with route [" + rutaEmpresa + "]");
            }

            List<string> conceptosCobro = ConceptosFactura(empresaId);

            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TABLE_NAME, INDEX, sFromDate);
            
            while(dbResponse==0)
            {
                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 11, sbFechaVenc, 9);
                sFechaVenc = sbFechaVenc.ToString().Substring(0, 8).Trim();
                bool parsed = DateTime.TryParseExact(sFechaVenc, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate);

                if (!parsed)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }
                
                if (!(sFechaVenc.CompareTo(sToDate) < 0)) break;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 26, ref cancelled);
                if (cancelled != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 3, ref conceptId);
                if (!concepts.ContainsKey(conceptId))
                {
                    concept = GetDocumentConceptCode(conceptId, rutaEmpresa);
                    concepts.Add(conceptId, concept);
                }

                concept = concepts[conceptId];

                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                
                if (!conceptosCobro.Contains(concept))
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }
                
                // detect client assignment
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 7, ref companyId);

                if (!customers.ContainsKey(companyId))
                {
                    companyCode = GetCompanyCode(companyId, rutaEmpresa);

                    if (companyCode == null)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                        continue;
                    }
                    DimClientes cliente = DimClientes.GetCliente(empresaId, companyCode, null);
                    if (cliente == null)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                        continue;
                    }
                    customers.Add(companyId, cliente);
                }

                DimClientes customer = customers[companyId];
                

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 44, ref saldo);

                if (saldo == 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = GetCurrencyName(currencyId, rutaEmpresa);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }

                currencyName = currencies[currencyId];

                if (!currencyName.ToUpper().Contains("PESO"))
                {
                    fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 16, ref cambio);
                    saldo = saldo * cambio;
                }

                // detect due group assignment
                FillFact(customer, dueDate, saldo, ref facts);

                dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
            }
        }

        protected override void FillFact(DimClientes cliente, DateTime dueDate, double amount, ref List<FactVencimiento> facts)
        {
            foreach (FactVencido fact in facts)
            {
                if (fact.Cliente.CodigoCliente == cliente.CodigoCliente && fact.Cliente.IdEmpresa == cliente.IdEmpresa)
                {
                    int startFac = fact.GrupoVencimiento.Inicio == 0 ? 1 : fact.GrupoVencimiento.Inicio;
                    DateTime startFactDate = DateTime.Today.AddDays(-startFac);
                    DateTime endFactDate = DateTime.Today.AddDays(-fact.GrupoVencimiento.Fin);

                    bool InfiniteGroup = fact.GrupoVencimiento.Fin == 0;
                    bool BeforeGroupStartDate = dueDate.CompareTo(startFactDate) <= 0;
                    bool AfterGroupEndDate = dueDate.CompareTo(endFactDate) > 0;

                    if ((InfiniteGroup && BeforeGroupStartDate)
                        || (AfterGroupEndDate && BeforeGroupStartDate))
                    {
                        fact.Saldo = fact.Saldo + amount;
                    }
                }
            }
        }
    }
}
