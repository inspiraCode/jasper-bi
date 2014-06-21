using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Configuration;
using System.Data;
using CommonAdminPaq;
using System.IO;

namespace WSAdminPaqWrapper.Miner
{
    public class CatEmpresa
    {
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string RutaEmpresa { get; set; }

        public static List<CatEmpresa> GetEmpresas(AdminPaqLib lib)
        {
            List<CatEmpresa> result = new List<CatEmpresa>();
            int connEmpresas, dbResponse, fieldResponse;

            if (lib.DataDirectory.Contains(':'))
            {
                lib.DataDirectory = lib.DataDirectory.Replace(':', ' ');
                lib.DataDirectory = lib.DataDirectory.Trim();
            }
                
            if (!Directory.Exists(lib.DataDirectory))
            {
                throw new Exception("Unable to validate existance of data directory: " + lib.DataDirectory);
            }

            connEmpresas = AdminPaqLib.dbLogIn("", lib.DataDirectory);

            if (connEmpresas == 0)
            {
                ErrLogger.Log("No se pudo crear conexión a la tabla de Empresas de AdminPAQ.");
                throw new Exception("Unable to create connection to " + lib.DataDirectory
                    + " AdminPaq reported connection result as " + connEmpresas.ToString());
            }

            string TABLE_EMPRESA = "MGW00001";
            string INDEX_NAME = "PRIMARYKEY";

            int idEmpresa = 0;

            dbResponse = AdminPaqLib.dbGetTopNoLock(connEmpresas, "MGW00001", INDEX_NAME);
            while (dbResponse == 0)
            {
                CatEmpresa empresa = new CatEmpresa();

                fieldResponse = AdminPaqLib.dbFieldLong(connEmpresas, TABLE_EMPRESA, 1, ref idEmpresa);
                empresa.IdEmpresa = idEmpresa;

                StringBuilder nombreEmpresa = new StringBuilder(151);
                fieldResponse = AdminPaqLib.dbFieldChar(connEmpresas, TABLE_EMPRESA, 2, nombreEmpresa, 151);
                string sNombreEmpresa = nombreEmpresa.ToString(0, 150).Trim();
                empresa.NombreEmpresa = sNombreEmpresa;

                StringBuilder rutaEmpresa = new StringBuilder(254);
                fieldResponse = AdminPaqLib.dbFieldChar(connEmpresas, TABLE_EMPRESA, 3, rutaEmpresa, 254);
                string sRutaEmpresa = rutaEmpresa.ToString(0, 253).Trim();
                empresa.RutaEmpresa = sRutaEmpresa;

                result.Add(empresa);
                dbResponse = AdminPaqLib.dbSkip(connEmpresas, TABLE_EMPRESA, INDEX_NAME, 1);
            }

            AdminPaqLib.dbLogOut(connEmpresas);
            return result;
        }
    }
}
