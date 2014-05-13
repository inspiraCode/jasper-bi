using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;
using System.Configuration;

namespace WSAdminPaqWrapper.Loader
{
    public class DimMeses
    {
        private Meses indiceMes = Meses.ENERO;

        public int IdMes { get; set; }
        public string CodigoMes 
        {
            get 
            {
                string mes = Enum.GetName(typeof(Meses), indiceMes);
                return mes.Substring(0,3);
            }
        }
        public int YYYY { get; set; }
        public string Mes 
        {
            get 
            {
                return Enum.GetName(typeof(Meses), indiceMes);
            }
        }
        public Meses IndiceMes { get { return indiceMes; } set { indiceMes = value; } }

        public static List<DimMeses> GetMeses(NpgsqlConnection conn)
        {
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            List<DimMeses> result = new List<DimMeses>();

            string sqlString = "SELECT id_mes, yyyy, indice_mes " +
                "FROM dim_meses;";

            if (conn == null || !(conn.State == ConnectionState.Open))
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                didOpenConnection = true;
            }

            cmd = new NpgsqlCommand(sqlString, conn);

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DimMeses dm = new DimMeses();

                dm.IdMes = int.Parse(dr["id_mes"].ToString());
                dm.YYYY = int.Parse(dr["yyyy"].ToString());
                dm.IndiceMes = (Meses) int.Parse(dr["indice_mes"].ToString());

                result.Add(dm);
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }

    }

    public enum Meses : byte
    {
        ENERO = 1,
        FEBRERO,
        MARZO,
        ABRIL,
        MAYO,
        JUNIO,
        JULIO,
        AGOSTO,
        SEPTIEMBRE,
        OCTUBRE,
        NOVIEMBRE,
        DICIEMBRE
    }
}
