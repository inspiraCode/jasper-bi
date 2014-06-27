using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using Npgsql;

namespace WSAdminPaqWrapper.Process
{
    public abstract class FactVencimiento : Factoreable
    {
        public DimClientes Cliente { get; set; }
        public DimGrupoVencimiento GrupoVencimiento { get; set; }
        public double Saldo { get; set; }

        public abstract void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn);
        protected abstract void DeleteFacts(int idEmpresa, NpgsqlConnection conn);
    }
}
