using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Process;
using Npgsql;

namespace WSAdminPaqWrapper.Loader
{
    public class MainLoader
    {
        public List<FactVencido> Vencidos { get; set; }
        public List<FactPorVencer> PorVencer { get; set; }
        public List<FactCobranza> Cobranza { get; set; }
        public List<FactSales> Ventas { get; set; }

        public void Load(int idEmpresa, NpgsqlConnection conn)
        {
            LoadDues(conn);
            LoadAboutToDue(conn);
            LoadSales(conn);
            LoadCollection(conn);
        }

        private void LoadCollection(NpgsqlConnection conn)
        { 
            foreach(FactCobranza collection in Cobranza)
            {
                AddCollection(collection, conn);
            }
        }

        private void LoadSales(NpgsqlConnection conn)
        {
            foreach (FactSales sale in Ventas)
            {
                AddSale(sale, conn);
            }
        }

        private void LoadDues(NpgsqlConnection conn)
        {
            foreach(FactVencido due in Vencidos)
            {
                AddFactVencido(due, conn);
            }
        }

        private void LoadAboutToDue(NpgsqlConnection conn)
        {
            foreach (FactPorVencer a2due in PorVencer)
            {
                AddFactPorVencer(a2due, conn);
            }
        }

        private void AddSale(FactSales fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_sales(seller_id, sold_today, sold_week, sold_month) " +
                "VALUES(@seller, @hoy, @semana, @mes);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@seller", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@hoy", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@semana", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@seller"].Value = fact.Seller.IdSeller;
            cmd.Parameters["@hoy"].Value = fact.SoldToday;
            cmd.Parameters["@semana"].Value = fact.SoldWeek;
            cmd.Parameters["@mes"].Value = fact.SoldMonth;

            cmd.ExecuteNonQuery();
        }

        private void AddCollection(FactCobranza fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_collection(id_mes, vendido, cobrado, incobrable)" +
                "VALUES(@mes, @vendido, @cobrado, @incobrable);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@vendido", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@cobrado", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@incobrable", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@mes"].Value = fact.Month.IdMes;
            cmd.Parameters["@vendido"].Value = fact.Sold;
            cmd.Parameters["@cobrado"].Value = fact.Collected;
            cmd.Parameters["@incobrable"].Value = fact.Uncollectable;

            cmd.ExecuteNonQuery();
        }

        private void AddFactPorVencer(FactVencimiento fact, NpgsqlConnection conn)
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

        private void AddFactVencido(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_vencido(id_cliente, id_grupo_vencimiento, saldo_vencido)" +
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
