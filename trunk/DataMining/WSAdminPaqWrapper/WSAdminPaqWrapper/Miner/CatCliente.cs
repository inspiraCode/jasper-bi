using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonAdminPaq;

namespace WSAdminPaqWrapper.Miner
{
    public class CatCliente
    {
        public string CodigoCliente { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public bool EsLocal { get; set; }

        public const string TABLE_NAME = "MGW10002";

        public static List<CatCliente> GetClientes(string rutaEmpresa)
        {
            // Generate connection from company route
            int connClientes = AdminPaqLib.dbLogIn("", rutaEmpresa);

            if (connClientes == 0)
            {
                throw new Exception("Unable to work with route [" + rutaEmpresa + "]");
            }

            List<CatCliente> result = GetClientes(connClientes);
            AdminPaqLib.dbLogOut(connClientes);

            return result;
        }

        private static List<CatCliente> GetClientes(int connectionId)
        {
            List<CatCliente> result = new List<CatCliente>();

            int dbResponse, fqResponse;

            int STEP = 1, LOCAL=1;
            int CCODIGOC01 = 2, CRAZONSO01 = 3, CIDVALOR01=14, CRFC=5;
            string PRIMARYKEY = "PRIMARYKEY";

            int location=0;
            StringBuilder sbCompanyCode = new StringBuilder(31);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbRfc = new StringBuilder(21);

            dbResponse = AdminPaqLib.dbGetTopNoLock(connectionId, TABLE_NAME, PRIMARYKEY);

            while (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldLong(connectionId, TABLE_NAME, CIDVALOR01, ref location);

                if (location == 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connectionId, TABLE_NAME, PRIMARYKEY, STEP);
                    continue;
                }

                CatCliente cliente = new CatCliente();
                cliente.EsLocal = (location == LOCAL); // LOCAL=1; FORANEO=2

                fqResponse = AdminPaqLib.dbFieldChar(connectionId, TABLE_NAME, CCODIGOC01, sbCompanyCode, 31);
                cliente.CodigoCliente = sbCompanyCode.ToString().Substring(0, 30).Trim();
                fqResponse = AdminPaqLib.dbFieldChar(connectionId, TABLE_NAME, CRAZONSO01, sbCompanyName, 61);
                cliente.RazonSocial = sbCompanyName.ToString().Substring(0, 60).Trim();
                
                
                fqResponse = AdminPaqLib.dbFieldChar(connectionId, TABLE_NAME, CRFC, sbRfc, 21);
                cliente.RFC = sbRfc.ToString().Substring(0, 20).Trim();

                result.Add(cliente);

                dbResponse = AdminPaqLib.dbSkip(connectionId, TABLE_NAME, PRIMARYKEY, STEP);
            }

            return result;
        }
    }
}
