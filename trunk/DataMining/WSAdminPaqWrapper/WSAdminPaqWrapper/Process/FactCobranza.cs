using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using Npgsql;
using WSAdminPaqWrapper.Miner;
using System.Data;
using System.Configuration;
using System.Diagnostics;

namespace WSAdminPaqWrapper.Process
{
    public class FactCobranza : Factoreable
    {
        public DimMeses Month { get; set; }
        public double Sold { get; set; }
        public double Collected { get; set; }
        public double Uncollectable { get; set; }

        private List<FactCobranza> gruposCobranza = new List<FactCobranza>();
        public List<FactCobranza> GruposCobranza { get { return gruposCobranza; } }

        public void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn)
        {
            if (gruposCobranza.Count > 0)
                return;

            List<DimMeses> meses = DimMeses.GetMeses(conn);

            foreach (DimMeses mes in meses)
            {
                FactCobranza cobranza = new FactCobranza();
                cobranza.Month = mes;
                cobranza.Sold = 0;
                cobranza.Collected = 0;

                FactUncollectable uncollectable = new FactUncollectable(mes);
                cobranza.Uncollectable = uncollectable.Uncollectable;

                gruposCobranza.Add(cobranza);
            }
        }
    }
}
