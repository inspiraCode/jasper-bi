using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSAdminPaqWrapper.Loader;
using Npgsql;

namespace WSAdminPaqWrapper.Process
{
    public abstract class FactVencimiento
    {
        public DimClientes Cliente { get; set; }
        public DimGrupoVencimiento GrupoVencimiento { get; set; }
        public double Saldo { get; set; }

        internal abstract void Execute(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn);
        
        protected abstract void DeleteFacts(int idEmpresa, NpgsqlConnection conn);
        protected abstract void AddFact(FactVencimiento fact, NpgsqlConnection conn);


    }
}
