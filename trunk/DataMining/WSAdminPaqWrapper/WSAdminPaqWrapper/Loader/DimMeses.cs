using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
