using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

namespace WSAdminPaqWrapper.Process
{
    interface Factoreable
    {
        void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn);
    }
}
