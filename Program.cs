using System.Diagnostics;
using VisanBC25;
using VisanBC25.Modelos;

Proceso p = new Proceso();
Funciones.KillAll();

m_Datos Datos = new m_Datos();
List<string> Opciones = new List<string>(); 

for (int n = 1; Environment.GetCommandLineArgs().Length>n; n++)
{
    Opciones.Add(Environment.GetCommandLineArgs()[n]);
}

if (Opciones.Count == 0) Opciones.Add(Funciones.Menu(System.Configuration.ConfigurationManager.AppSettings["VERSION"]));


await p.Bucle(Datos, Opciones);
