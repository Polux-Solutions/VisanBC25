using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VisanBC25.Modelos
{
    public class m_SQL
    {
        public SqlConnection SqlConn { get; set; } = null;
        public SqlDataReader Reader { get; set; } = null;
        public DataTable TableData { get; set; } = null;
        public DataSet ds { get; set; } = null;
        public bool Estado { get; set; } = true;
        public string Error { get; set; } = string.Empty;
    }

    
}
