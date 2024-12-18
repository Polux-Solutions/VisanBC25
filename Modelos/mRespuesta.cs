using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VisanBC25.Modelos
{
    public class m_Respuesta
    {
        public bool Ok{ get; set; } = true;
        public string Respuesta { get; set; } = string.Empty;
    }
}
