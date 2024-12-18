using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisanBC25.Modelos
{
    public class m_Datos
    {
        public string Folder { get; set; } = string.Empty;
        public string Log { get; set; } = string.Empty;
        public string LogDetalle { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string ServerWeb { get; set; } = string.Empty ;
        public int Puerto { get; set; } = 0;
        public string Instancia { get; set; } = string.Empty;
        public string CompanyWeb { get; set; } = string.Empty;
        public string Codeunit { get; set; } = string.Empty;
        public string UserBC { get; set; } = string.Empty;
        public string PasswdBC { get; set; } = string.Empty;
        public string DomainBC { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
        public string Error { get; set; } = string.Empty;
    }
}
