using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Configuration;
using System.Data;

namespace WSAdminPaqWrapper.Miner
{
    public class CatEmpresa
    {
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string RutaEmpresa { get; set; }

        public static List<CatEmpresa> GetEmpresas()
        {
            List<CatEmpresa> result = new List<CatEmpresa>();

            NpgsqlConnection conn;
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            
            string sqlString = "SELECT id_empresa, nombre_empresa, ruta " +
                "FROM cat_empresa;";

            string connectionString = ConfigurationManager.ConnectionStrings[Configuration.Common.MONFOLL].ConnectionString;
            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);

            conn.Close();

            if (ds.Tables.Count == 1)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(EmpresaFromRow(row));
                }
            }

            return result;
        }

        private static CatEmpresa EmpresaFromRow(DataRow row)
        {
            CatEmpresa result = new CatEmpresa();

            result.IdEmpresa = int.Parse(row["id_empresa"].ToString());
            result.NombreEmpresa = row["nombre_empresa"].ToString();
            result.RutaEmpresa = row["ruta"].ToString();

            return result;
        }


    }
}
