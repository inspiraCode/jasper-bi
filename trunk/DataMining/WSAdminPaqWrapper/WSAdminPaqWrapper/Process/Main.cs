using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Miner;
using Npgsql;
using System.Configuration;

namespace WSAdminPaqWrapper.Process
{
    public class Main
    {
        public static void Execute()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            string connectionString = ConfigurationManager.ConnectionStrings[Configuration.Common.JASPER].ConnectionString;
            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            List<CatEmpresa> empresas = CatEmpresa.GetEmpresas();
            foreach (CatEmpresa empresa in empresas)
            {
                // DIM ETLs
                ETLClientes.Execute(empresa.IdEmpresa, empresa.NombreEmpresa, CatCliente.GetClientes(empresa.RutaEmpresa), conn);
                ETLMeses.Execute(conn);

                // FACT ETLs.
                FactVencido vencido = new FactVencido();
                vencido.Execute(empresa.IdEmpresa, empresa.RutaEmpresa, conn);

                FactPorVencer porVencer = new FactPorVencer();
                porVencer.Execute(empresa.IdEmpresa, empresa.RutaEmpresa, conn);
            }

            conn.Close();
        }
    }
}
