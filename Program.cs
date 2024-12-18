using System.Diagnostics;
using VisanBC25;
using VisanBC25.Modelos;

m_Datos Datos = new m_Datos();
List<string> Opciones = new List<string>(); 
int Counter = 0;

if (! Funciones.Leer_Parametros(ref Datos))
{
    Counter += 1;
    Console.WriteLine($"{Counter.ToString()} Error Lectura parámetros: {Datos.Error}");
    return;
}

for (int n = 1; Environment.GetCommandLineArgs().Length>n; n++)
{
    Opciones.Add(Environment.GetCommandLineArgs()[n]);
}


Proceso p = new Proceso();
Funciones.KillAll();

if (Opciones.Count == 0) Opciones.Add(Funciones.Menu(ref Datos));
await p.Bucle(Datos, Opciones);
