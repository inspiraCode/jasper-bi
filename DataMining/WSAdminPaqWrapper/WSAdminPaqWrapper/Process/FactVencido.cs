using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using WSAdminPaqWrapper.Loader;
using WSAdminPaqWrapper.Miner;

namespace WSAdminPaqWrapper.Process
{
    public class FactVencido : FactVencimiento
    {
        private List<FactVencido> gruposVencimiento = new List<FactVencido>();
        public List<FactVencido> GruposVencimiento { get { return gruposVencimiento; } }

        protected override void DeleteFacts(int idEmpresa, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            // Remove last year old months
            string sqlString = "DELETE " +
                "FROM fact_vencido " +
                "WHERE id_cliente IN (SELECT id_cliente FROM dim_clientes WHERE id_empresa=" + idEmpresa + ")";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.ExecuteNonQuery();
        }


        public override void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn)
        {
            DeleteFacts(idEmpresa, conn);

            List<DimClientes> clientes = DimClientes.GetAll(idEmpresa, conn);
            List<DimGrupoVencimiento> vencimientos = DimGrupoVencimiento.Vencimientos(conn);

            foreach (DimClientes cliente in clientes)
            {
                foreach (DimGrupoVencimiento vencimiento in vencimientos)
                {
                    FactVencido fact = new FactVencido();
                    fact.Cliente = cliente;
                    fact.GrupoVencimiento = vencimiento;
                    fact.Saldo = 0;

                    gruposVencimiento.Add(fact);
                }
            }
        }
    }
}
