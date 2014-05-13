using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WSAdminPaqWrapper.Config
{
    /// <summary>
    /// Sección que define la configuración de una empresa y sus códigos de documentos
    /// </summary>
    public class EnterpriseSection : ConfigurationSection
    {
        [ConfigurationProperty("NombreEmpresa", DefaultValue = "Ramos Hermanos Internacional SPR de RL de CV")]
        public string NombreEmpresa
        {
            get { return (string)this["NombreEmpresa"]; }
            set { this["NombreEmpresa"] = value; }
        }

        [ConfigurationProperty("ConceptosVenta", DefaultValue = "2117,2118,2119,2120")]
        public string ConceptosVenta
        {
            get { return (string)this["ConceptosVenta"]; }
            set { this["ConceptosVenta"] = value; }
        }

        [ConfigurationProperty("ConceptosDevolucion", DefaultValue = "2109,2110")]
        public string ConceptosDevolucion
        {
            get { return (string)this["ConceptosDevolucion"]; }
            set { this["ConceptosDevolucion"] = value; }
        }

    }
}
