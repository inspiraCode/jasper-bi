using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;
using System.Configuration;

namespace WSAdminPaqWrapper.Loader
{
    public class DimGrupoVencimiento
    {
        public int IdGrupo { get; set; }
        public int Inicio { get; set; }
        public int Fin { get; set; }

        public static List<DimGrupoVencimiento> Vencimientos(NpgsqlConnection conn)
        {
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            List<DimGrupoVencimiento> result = new List<DimGrupoVencimiento>();

            string sqlString = "SELECT id_grupo_vencimiento, minimo_dias, maximo_dias " +
                "FROM dim_grupo_vencimiento;";

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
                DimGrupoVencimiento gv = new DimGrupoVencimiento();

                gv.IdGrupo = int.Parse(dr["id_grupo_vencimiento"].ToString());
                gv.Inicio = int.Parse(dr["minimo_dias"].ToString());
                gv.Fin = int.Parse(dr["maximo_dias"].ToString());

                result.Add(gv);
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }
    }
}
