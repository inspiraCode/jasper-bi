using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;
using System.Configuration;

namespace WSAdminPaqWrapper.Loader
{
    public class DimSellers
    {
        public int IdSeller { get; set; }
        public int ApId { get; set; }
        public string Code { get; set; }
        public string AgentName { get; set; }
        public string Email { get; set; }
        public double WeeklyGoal { get; set; }
        public string Empresa { get; set; }
        public int IdEmpresa { get; set; }

        public static DimSellers GetSellerByAdminPaqId(NpgsqlConnection conn, int id, int enterpriseId)
        {
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            DimSellers result = null;

            string sqlString = "SELECT seller_id, ap_id, agent_code, agent_name, email, weekly_goal, empresa, id_empresa " +
                "FROM dim_sellers " +
                "WHERE ap_id=@id AND id_empresa = @enterpriseId;";

            if (conn == null || !(conn.State == ConnectionState.Open))
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                didOpenConnection = true;
            }

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@enterpriseId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = id;
            cmd.Parameters["@enterpriseId"].Value = enterpriseId;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = new DimSellers();

                result.IdSeller = int.Parse(dr["seller_id"].ToString());
                result.ApId = int.Parse(dr["ap_id"].ToString());
                result.Code = dr["agent_code"].ToString();
                result.AgentName = dr["agent_code"].ToString();
                result.Email = dr["email"].ToString();
                result.WeeklyGoal = double.Parse(dr["weekly_goal"].ToString());
                result.Empresa = dr["empresa"].ToString();
                result.IdEmpresa = int.Parse(dr["id_empresa"].ToString());
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }

        public static DimSellers GetSellerById(NpgsqlConnection conn, int id)
        {
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            DimSellers result = null;

            string sqlString = "SELECT seller_id, ap_id, agent_code, agent_name, email, weekly_goal, empresa, id_empresa " +
                "FROM dim_sellers " +
                "WHERE seller_id=@id;";

            if (conn == null || !(conn.State == ConnectionState.Open))
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                didOpenConnection = true;
            }

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = id;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = new DimSellers();

                result.IdSeller = int.Parse(dr["seller_id"].ToString());
                result.ApId = int.Parse(dr["ap_id"].ToString());
                result.Code = dr["agent_code"].ToString();
                result.AgentName = dr["agent_name"].ToString();
                result.Email = dr["email"].ToString();
                result.WeeklyGoal = double.Parse(dr["weekly_goal"].ToString());
                result.Empresa = dr["empresa"].ToString();
                result.IdEmpresa = int.Parse(dr["id_empresa"].ToString());
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }

        public static List<DimSellers> GetSellers(NpgsqlConnection conn)
        {
            bool didOpenConnection = false;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            List<DimSellers> result = new List<DimSellers>();

            string sqlString = "SELECT seller_id, ap_id, agent_code, email, weekly_goal, empresa, id_empresa " +
                "FROM dim_sellers;";

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
                DimSellers dm = new DimSellers();

                dm.IdSeller = int.Parse(dr["seller_id"].ToString());
                dm.ApId = int.Parse(dr["ap_id"].ToString());
                dm.Code = dr["agent_code"].ToString();
                dm.Email = dr["email"].ToString();
                dm.WeeklyGoal = double.Parse(dr["weekly_goal"].ToString());
                dm.Empresa = dr["empresa"].ToString();
                dm.IdEmpresa = int.Parse(dr["id_empresa"].ToString());

                result.Add(dm);
            }

            dr.Close();

            if (didOpenConnection)
                conn.Close();

            return result;
        }
    }
}
