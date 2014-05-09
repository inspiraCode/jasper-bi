using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Process;
using WSAdminPaqWrapper.Loader;
using CommonAdminPaq;
using System.Configuration;
using Npgsql;

namespace WSAdminPaqWrapper.Miner
{
    public abstract class BaseBalance
    {
        public const string TABLE_NAME = "MGW10008";
        public const string INDEX = "CFECHAVE01";

        public abstract void FillFacts(ref List<FactVencimiento> facts, string rutaEmpresa, int empresaId);
        protected abstract void FillFact(DimClientes cliente, DateTime dueDate, double amount, ref List<FactVencimiento> facts);

        protected string GetCurrencyName(int currencyId, string filePath)
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

        protected string GetCompanyCode(int companyId, string filePath)
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

        protected string GetDocumentConceptCode(int docId, string filePath)
        {
            string response = null;
            int connDocos, dbResponse, fqResponse;
            StringBuilder sConceptCode = new StringBuilder(31);
            string key, conceptCode;

            connDocos = AdminPaqLib.dbLogIn("", filePath);
            if (connDocos == 0)
            {
                return response;
            }

            key = docId.ToString().PadLeft(11);
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

        protected List<string> ConceptosFactura(int empresaId)
        {
            List<string> conceptos = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings[Configuration.Common.MONFOLL].ConnectionString;
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


    }
}
