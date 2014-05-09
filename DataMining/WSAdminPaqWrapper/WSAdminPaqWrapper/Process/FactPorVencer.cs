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

        internal override void Execute(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn)
        {
            List<DimClientes> clientes = DimClientes.GetAll(idEmpresa, conn);
            List<DimGrupoVencimiento> vencimientos = DimGrupoVencimiento.Vencimientos(conn);
            List<FactVencimiento> facts = new List<FactVencimiento>();

            foreach (DimClientes cliente in clientes)
            {
                foreach (DimGrupoVencimiento vencimiento in vencimientos)
                {
                    FactPorVencer fact = new FactPorVencer();
                    fact.Cliente = cliente;
                    fact.GrupoVencimiento = vencimiento;
                    fact.Saldo = 0;

                    facts.Add(fact);
                }
            }

            CurrentBalances balances = new CurrentBalances();
            balances.FillFacts(ref facts, rutaEmpresa, idEmpresa);

            DeleteFacts(idEmpresa, conn);
            foreach (FactPorVencer fact in facts)
            {
                AddFact(fact, conn);
            }
        }

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

        protected override void AddFact(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_por_vencer(id_cliente, id_grupo_vencimiento, saldo_por_vencer)" +
                "VALUES(@cliente, @grupo, @saldo);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Numeric);


            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;
            cmd.Parameters["@saldo"].Value = fact.Saldo;

            cmd.ExecuteNonQuery();
        }
    }
}
