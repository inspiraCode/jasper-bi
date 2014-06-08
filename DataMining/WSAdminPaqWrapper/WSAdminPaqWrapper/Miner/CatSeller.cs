using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonAdminPaq;

namespace WSAdminPaqWrapper.Miner
{
    public class CatSeller
    {
        public string CodigoVendedor { get; set; }
        public string NombreVendedor { get; set; }
        public int IdVendedor { get; set; }

        public const string TABLE_NAME = "MGW10001";

        public static List<CatSeller> GetSellers(string rutaEmpresa)
        {
            int connSellers = AdminPaqLib.dbLogIn("", rutaEmpresa);

            if(connSellers == 0)
            {
                throw new Exception("Unable to work with route [" + rutaEmpresa + "]");
            }

            List<CatSeller> result = GetSellers(connSellers);
            AdminPaqLib.dbLogOut(connSellers);

            return result;
        }

        private static List<CatSeller> GetSellers(int connectionId)
        {
            List<CatSeller> result = new List<CatSeller>();

            int dbResponse, fqResponse;

            int STEP = 1;
            int CIDAGENTE = 1, CCODIGOA01 = 2, CNOMBREA01 = 3, CTIPOAGE01=5;
            int COLLECTOR_TYPE = 3;
            string PRIMARYKEY = "PRIMARYKEY";

            StringBuilder sbNombreAgente = new StringBuilder(61);
            StringBuilder sbCodigoAgente = new StringBuilder(31);
            int id = 0, type=0;

            dbResponse = AdminPaqLib.dbGetTopNoLock(connectionId, TABLE_NAME, PRIMARYKEY);

            while(dbResponse == 0)
            {
                AdminPaqLib.dbFieldLong(connectionId, TABLE_NAME, CTIPOAGE01, ref type);

                if (type == COLLECTOR_TYPE)
                {
                    dbResponse = AdminPaqLib.dbSkip(connectionId, TABLE_NAME, PRIMARYKEY, STEP);
                    continue;
                }

                CatSeller seller = new CatSeller();
                
                AdminPaqLib.dbFieldLong(connectionId, TABLE_NAME, CIDAGENTE, ref id);
                seller.IdVendedor = id;

                AdminPaqLib.dbFieldChar(connectionId, TABLE_NAME, CCODIGOA01, sbCodigoAgente, 31);
                seller.CodigoVendedor = sbCodigoAgente.ToString().Substring(0, 30).Trim();

                AdminPaqLib.dbFieldChar(connectionId, TABLE_NAME, CNOMBREA01, sbNombreAgente, 61);
                seller.NombreVendedor = sbNombreAgente.ToString().Substring(0, 60).Trim();

                result.Add(seller);

                dbResponse = AdminPaqLib.dbSkip(connectionId, TABLE_NAME, PRIMARYKEY, STEP);
            }

            return result;
        }

    }
}
