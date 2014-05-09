using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using WSAdminPaqWrapper.Loader;

namespace WSAdminPaqWrapper.Process
{
    
    class ETLMeses
    {
        internal static void Execute(NpgsqlConnection conn)
        {

            int currentMonth = DateTime.Today.Month;
            int maxMonth = currentMonth + 3;
            int currentYear = DateTime.Today.Year;
            int lastYear = currentYear - 1;

            // Remove last year old months
            RemoveOldMonths(conn);

            // Check and cerate All months content
            CreateAllMonths(conn);

        }

        private static void RemoveOldMonths(NpgsqlConnection conn)
        {
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year;
            int lastYear = currentYear - 1;

            NpgsqlCommand cmd;

            // Remove last year old months
            string sqlString = "DELETE FROM dim_meses " +
                "WHERE yyyy=@year AND indice_mes < " + currentMonth.ToString();


            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@year", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            cmd.Parameters["@year"].Value = lastYear.ToString();
            
            
            cmd.ExecuteNonQuery();
        }

        private static void CreateAllMonths(NpgsqlConnection conn)
        {
            DateTime today = DateTime.Today;
            DateTime lastYear = today.AddYears(-1);
            DateTime cycler = lastYear;

            for (int i = 0; i <= 15; i++)
            {
                if (!MonthExists(cycler.Year, cycler.Month, conn))
                    CreateMonth(cycler.Year, cycler.Month, conn);

                cycler = cycler.AddMonths(1);
            }

        }

        private static bool MonthExists(int year, int month, NpgsqlConnection conn)
        {
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            bool result = false;

            string sqlString = "SELECT id_mes " +
                "FROM dim_meses " +
                "WHERE yyyy=@year AND indice_mes=@month;";


            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@year", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            cmd.Parameters.Add("@month", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@year"].Value = year.ToString();
            cmd.Parameters["@month"].Value = month;

            dr = cmd.ExecuteReader();
            result = dr.Read();
            dr.Close();

            return result;
        }

        private static void CreateMonth(int year, int month, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            // Remove last year old months
            string sqlString = "INSERT INTO dim_meses (codigo_mes, yyyy, nombre_mes, indice_mes) " +
                "VALUES(@codigo, @year, @mes, @indice)";

            DimMeses dim = new DimMeses();
            dim.YYYY = year;
            dim.IndiceMes = (Loader.Meses) month;


            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            cmd.Parameters.Add("@year", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Varchar, 40);
            cmd.Parameters.Add("@indice", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@codigo"].Value = dim.CodigoMes;
            cmd.Parameters["@year"].Value = dim.YYYY;
            cmd.Parameters["@mes"].Value = dim.Mes;
            cmd.Parameters["@indice"].Value = dim.IndiceMes;

            cmd.ExecuteNonQuery();
        }

    }
}
