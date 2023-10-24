using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Proyecto_SO
{
    public partial class Form1 : Form
    {
        private List<Proceso> procesosLlegadaCero = new List<Proceso>();
        private List<Proceso> procesosPrioridad = new List<Proceso>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnPlanificar_Click(object sender, EventArgs e)
        {
            procesosLlegadaCero.Clear();
            procesosPrioridad.Clear();
            lstResultados.Items.Clear();

            int numProcesos = int.Parse(txtNumProcesos.Text);

            for (int i = 0; i < numProcesos; i++)
            {
                //Microsoft.VisualBasic.Interaction.InputBox 
                //ESTA FUNCION NOS ABRE OTRA/S VENTANA/S PARA QUE PODAMOS INGRESAR LOS VALORES DE LOS PROCESOS

                string nombre = Microsoft.VisualBasic.Interaction.InputBox($"Ingrese el nombre del proceso {i + 1}:");
                int rafaga = int.Parse(Microsoft.VisualBasic.Interaction.InputBox($"Ingrese la Rafaga de CPU del proceso {i + 1}:"));
                int tiempoLlegada = int.Parse(Microsoft.VisualBasic.Interaction.InputBox($"Ingrese el tiempo de llegada del proceso {i + 1}:"));
                int prioridad = int.Parse(Microsoft.VisualBasic.Interaction.InputBox($"Ingrese la prioridad del proceso {i + 1} (mayor número = mayor prioridad):"));

                Proceso proceso = new Proceso(nombre, rafaga, tiempoLlegada, prioridad);

                if (tiempoLlegada == 0)
                {
                    procesosLlegadaCero.Add(proceso);
                }
                else
                {
                    procesosPrioridad.Add(proceso);
                }
            }

            procesosLlegadaCero.Sort((x, y) => x.TiempoLlegada != y.TiempoLlegada
                ? x.TiempoLlegada.CompareTo(y.TiempoLlegada)
                : x.Prioridad.CompareTo(y.Prioridad));

            int tiempoTotal = 0;
            List<int> tiemposEspera = new List<int>();
            List<int> tiemposRetorno = new List<int>();

            if (procesosLlegadaCero.Count == 1)
            {
                Proceso procesoUnico = procesosLlegadaCero[0];
                lstResultados.Items.Add($"Ejecutando {procesoUnico.Nombre} (Rafaga: {procesoUnico.Rafaga}, Tiempo de llegada: {procesoUnico.TiempoLlegada}, Prioridad: {procesoUnico.Prioridad})");
                tiempoTotal += procesoUnico.Rafaga;
                tiemposEspera.Add(0);
                tiemposRetorno.Add(tiempoTotal);
            }

            if (procesosLlegadaCero.Count > 1)
            {
                procesosLlegadaCero.Sort((x, y) => x.Prioridad.CompareTo(y.Prioridad));
                Proceso procesoElegido = procesosLlegadaCero[0];
                lstResultados.Items.Add($"Ejecutando {procesoElegido.Nombre} (Rafaga: {procesoElegido.Rafaga}, Tiempo de llegada: {procesoElegido.TiempoLlegada}, Prioridad: {procesoElegido.Prioridad})");
                tiempoTotal += procesoElegido.Rafaga;
                tiemposEspera.Add(0);
                tiemposRetorno.Add(tiempoTotal);

                foreach (Proceso proceso in procesosLlegadaCero.GetRange(1, procesosLlegadaCero.Count - 1))
                {
                    procesosPrioridad.Insert(0, proceso);
                }
            }

            procesosPrioridad.Sort((x, y) => x.Prioridad != y.Prioridad
                ? x.Prioridad.CompareTo(y.Prioridad)
                : x.TiempoLlegada.CompareTo(y.TiempoLlegada));

            foreach (Proceso proceso in procesosPrioridad)
            {
                lstResultados.Items.Add($"Ejecutando {proceso.Nombre} (Rafaga: {proceso.Rafaga}, Tiempo de llegada: {proceso.TiempoLlegada}, Prioridad: {proceso.Prioridad})");
                tiempoTotal += proceso.Rafaga;
                tiemposEspera.Add(tiempoTotal - (proceso.Rafaga + proceso.TiempoLlegada));
                tiemposRetorno.Add(tiempoTotal);
            }

            lstResultados.Items.Add($"Tiempo total de ejecución: {tiempoTotal}");
            lstResultados.Items.Add("Tiempos de espera: " + string.Join(", ", tiemposEspera));
            lstResultados.Items.Add("Tiempos de retorno: " + string.Join(", ", tiemposRetorno));

            int tiempoTotalEspera = tiemposEspera.Sum();
            int tiempoTotalRetorno = tiemposRetorno.Sum();

            double promedioEspera = (double)tiempoTotalEspera / numProcesos;
            lstResultados.Items.Add($"Tiempo total de espera: {tiempoTotalEspera}");
            lstResultados.Items.Add($"Promedio del tiempo de espera: {promedioEspera:F2}");

            double promedioRetorno = (double)tiempoTotalRetorno / numProcesos;
            lstResultados.Items.Add($"Tiempo total de retorno: {tiempoTotalRetorno}");
            lstResultados.Items.Add($"Promedio del tiempo de retorno: {promedioRetorno:F2}");

            btnReiniciar.Enabled = true;

        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            // Limpiar los resultados y controles de entrada.
            procesosLlegadaCero.Clear();
            procesosPrioridad.Clear();
            lstResultados.Items.Clear();
            txtNumProcesos.Text = "";

            // Deshabilitar el botón de reinicio hasta que se inicie otra planificación.
            btnReiniciar.Enabled = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Cerrar la aplicación cuando se hace clic en el botón "Salir".
            Application.Exit();
        }



    }

    class Proceso
    {
        public string Nombre { get; }
        public int Rafaga { get; }
        public int TiempoLlegada { get; }
        public int Prioridad { get; }

        public Proceso(string nombre, int rafaga, int tiempoLlegada, int prioridad)
        {
            Nombre = nombre;
            Rafaga = rafaga;
            TiempoLlegada = tiempoLlegada;
            Prioridad = prioridad;
        }
    }
}
