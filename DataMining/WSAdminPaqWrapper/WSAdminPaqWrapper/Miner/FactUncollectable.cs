using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using Npgsql;
using System.Data;
using System.Configuration;

namespace WSAdminPaqWrapper.Miner
{
    public class FactUncollectable
    {
        public DimMeses Mes { get; set; }
        public double Uncollectable { get; set; }

        public FactUncollectable(DimMeses mes)
        {
            List<CatEmpresa> result = new List<CatEmpresa>();

            NpgsqlConnection conn;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT  SUM(saldo) AS uncollectable " +
                            "FROM ctrl_seguimiento " +
                            "INNER JOIN ctrl_cuenta " +
                            "ON ctrl_seguimiento.id_doco = ctrl_cuenta.id_doco " +
                            "WHERE id_movimiento = 7 " +
                            "AND EXTRACT(MONTH FROM ts_seguimiento) = @mes " +
                            "AND EXTRACT(YEAR FROM ts_seguimiento)=@yyyy;";


            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@yyyy", NpgsqlTypes.NpgsqlDbType.Integer);
            
            cmd.Parameters["@mes"].Value = (int) mes.IndiceMes;
            cmd.Parameters["@yyyy"].Value = mes.YYYY;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bool parsed = false;
                double tempUncollectable = 0;
                parsed = double.TryParse(dr["uncollectable"].ToString(), out tempUncollectable);

                if(parsed)
                    Uncollectable = tempUncollectable;
                else
                    Uncollectable = 0;
            }   
            else
                Uncollectable = 0;
            

            dr.Close();
            conn.Close();
        }

    }
}
