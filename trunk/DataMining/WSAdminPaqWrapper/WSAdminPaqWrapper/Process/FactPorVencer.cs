using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using Npgsql;
using WSAdminPaqWrapper.Miner;

namespace WSAdminPaqWrapper.Process
{
    public class FactPorVencer : FactVencimiento
    {
        private List<FactPorVencer> gruposVencimiento = new List<FactPorVencer>();
        public List<FactPorVencer> GruposVencimiento { get { return gruposVencimiento; } }

        protected override void DeleteFacts(int idEmpresa, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            // Remove last year old months
            string sqlString = "DELETE " +
                "FROM fact_por_vencer " +
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
                    FactPorVencer fact = new FactPorVencer();
                    fact.Cliente = cliente;
                    fact.GrupoVencimiento = vencimiento;
                    fact.Saldo = 0;

                    gruposVencimiento.Add(fact);
                }
            }
        }
    }
}
