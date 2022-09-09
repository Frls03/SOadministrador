﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;



namespace Simulacion_Procesos
{
    public partial class Form1 : Form 
    {
        //Declaración de Variable String para obtener el nombre del proceso en la tabla para su eliminacion
        string Str_Obt_Proc;
        public ContextMenuStrip menu;
        public Process selected_process;
        public Form1()
        {
            InitializeComponent();
            //Activacion del timer que actualizara la tabla
            ActualizarTabla();
            timer1.Enabled = true;
            
        }
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint ="SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private void ActualizarTabla()
        {
            //limpieza del datagrid
            dgv_Proceso.Rows.Clear();
            //creacion columnas con sus respectivos nombres
            dgv_Proceso.Columns[0].Name = "Num. Procesos";
            dgv_Proceso.Columns[1].Name = "Procesos";
            dgv_Proceso.Columns[2].Name = "Prioridad Proceso";
            dgv_Proceso.Columns[3].Name = "ID";
            dgv_Proceso.Columns[4].Name = "Memoria Fisica";
            dgv_Proceso.Columns[5].Name = "Memoria Virtual";
           

            //Propiedad para autoajustar el tamaño de las celdas segun su contenido
            dgv_Proceso.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //Propiedad para que el usuario seleccione solamente filas en la tabla y no celdas sueltas
            dgv_Proceso.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //Propiedad para que el usuario no pueda seleccionar mas de una fila
            dgv_Proceso.MultiSelect = false;

            //Declaracion de la variable que sera un contador para el total de procesos
            int Int_Cant_Proc = 1;


            foreach (Process Proc_Proceso in Process.GetProcesses())
            {
                //Ingreso de los datos en el datagrid
                dgv_Proceso.Rows.Add(Int_Cant_Proc, Proc_Proceso.ProcessName, Proc_Proceso.BasePriority, Proc_Proceso.Id, Proc_Proceso.WorkingSet64,
                    Proc_Proceso.VirtualMemorySize64);
                //aumento en 1 de la variable
                Int_Cant_Proc += 1;
            }
            //El label muestra la cantidad de procesos actuales
            lbl_Contador.Text = "Procesos Actuales: " + (Int_Cant_Proc - 1);    //  cant de procesos   


        } //fin metodo ActualizarTabla


        //Boton Actualizar
        private void BtnActualizar_Click(object sender, EventArgs e){
            //Ocultamos todos los objetos para los graficos y mostramos solo el Dgv de Procesos
            LblNombreCPU.Visible = false;
            LblNombreRam.Visible = false;
            ProgressBarCPU.Visible = false;
            ProgressBarRAM.Visible = false;
            LblPorCPU.Visible = false;
            LblPorRAM.Visible = false;
            Grafico.Visible = false;
            dgv_Proceso.Visible = true;

            //Llamado al proceso para actualizar la tabla
            ActualizarTabla(); 
        }


        public Process getProcess(String ProcessName)
        {
            try 
            {
                //Por cada proceso que haya se comparara su nombre con el nombre del proceso que se desea eliminar.
                foreach(Process proceso in Process.GetProcesses())
                {
                    if (proceso.ProcessName == ProcessName)
                    {
                        return proceso;
                    }
                }

            }
                //En caso no se seleccione un proceso mostrara el siguiente mensaje.
            catch (Exception x) 
            {
                MessageBox.Show("No se ha seleccionado ningun proceso para detener." + x, "Error al Detener Proceso", MessageBoxButtons.OK);
            }
            return new Process();
        }

   

        //Boton Detener Proceso
        private void Btn_Detener_Click(object sender, EventArgs e){
            //Ocultamos todos los objetos para los graficos y mostramos solo el Dgv de Procesos
            LblNombreCPU.Visible = false;
            LblNombreRam.Visible = false;
            ProgressBarCPU.Visible = false;
            ProgressBarRAM.Visible = false;
            LblPorCPU.Visible = false;
            LblPorRAM.Visible = false;
            Grafico.Visible = false;
            dgv_Proceso.Visible = true;

            try 
            {
                //Por cada proceso que haya se comparara su nombre con el nombre del proceso que se desea eliminar.
                foreach(Process proceso in Process.GetProcesses())
                {
                    if (proceso.ProcessName == Str_Obt_Proc)
                    {
                        proceso.Kill();//Se elimina el proceso
                        ActualizarTabla();//Se llama al proceso para actualizar la tabla automaticamente
                    }
                }

            }
                //En caso no se seleccione un proceso mostrara el siguiente mensaje.
            catch (Exception x) 
            {
                MessageBox.Show("No se ha seleccionado ningun proceso para detener." + x, "Error al Detener Proceso", MessageBoxButtons.OK);
            }
        }


        private void dgv_Proceso_MouseClick_1(object sender, MouseEventArgs e)
        {
            //La variable obtiene el Nombre del Proceso de la Tabla al hacerle clic
            Str_Obt_Proc = dgv_Proceso.SelectedRows[0].Cells[1].Value.ToString();
        }

        private void Form1_Load(object sender, EventArgs e){
            //Evento que maneja el contador de rendimiento de la RAM y del CPU
            timer.Start();
        }
        

        private void Timer_Tick(object sender, EventArgs e){
            //Asignamos un float a un PerformanceCounter que ya tiene asignado el contador de rendimiento de la RAM y del CPU
            float fCPU = pCPU.NextValue();
            float fRAM = pRAM.NextValue();
            //Agregamos los Valores a la Barra de progeso respectivamente
            ProgressBarCPU.Value = (int)fCPU;
            ProgressBarRAM.Value = (int)fRAM;
            //Damos el formato de porcentaje correspondiente al label de porcentaje con lo float
            LblPorCPU.Text = string.Format("{0:0.00}%", fCPU);
            LblPorRAM.Text = string.Format("{0:0.00}%", fRAM);
            //Agregamos los valores de Y que se usaran para mostrarlos en grafica
            Grafico.Series["CPU"].Points.AddY(fCPU);
            Grafico.Series["RAM"].Points.AddY(fRAM);
        }

        private void Button1_Click(object sender, EventArgs e){
            //Ocultamos el Dgv de Procesos y mostramos todos los objetos para los gráficos
            LblNombreCPU.Visible = true;
            LblNombreRam.Visible = true;
            ProgressBarCPU.Visible = true;
            ProgressBarRAM.Visible = true;
            LblPorCPU.Visible = true;
            LblPorRAM.Visible = true;
            Grafico.Visible = true;
            dgv_Proceso.Visible = false;
        }

        private void Grafico_Click(object sender, EventArgs e)
        {

        }

        private void dgv_Proceso_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Detener_MouseEnter(object sender, EventArgs e)
        {
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_Proceso_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
            }
            else
            {
                this.menu = new System.Windows.Forms.ContextMenuStrip();
                int mousePosition = dgv_Proceso.HitTest(e.X, e.Y).RowIndex;
                var hti = dgv_Proceso.HitTest(e.X, e.Y);
                dgv_Proceso.ClearSelection();
                dgv_Proceso.Rows[hti.RowIndex].Selected = true;
                if(mousePosition >= 0)
                {
                    this.menu.Items.Add("Finalizar Proceso").Name = "Delete";
                    this.menu.Items.Add("Ver Detalles").Name = "Details";
                    this.menu.Items.Add("Abrir Ubicacion").Name = "Location";
                }
                this.menu.Show(dgv_Proceso, new Point(e.X, e.Y));
                this.menu.ItemClicked += Menu_ItemClicked;

                String nameProcess = dgv_Proceso.SelectedRows[0].Cells[1].Value.ToString();
                this.selected_process = this.getProcess(nameProcess);
            }

        }


        public void killProcess(Process proc)
        {
            proc.Kill();
            this.ActualizarTabla();
            MessageBox.Show("Proceso: " + proc.ProcessName + " finalizado");
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.menu.Visible = false;
            switch (e.ClickedItem.Name.ToString())
            {
                case "Delete":
                    this.killProcess(this.selected_process);
                    break;
                case "Location":
                    MessageBox.Show("Mostraremos la ubicacion");
                    break;
                case "Details":
                    MessageBox.Show("Detalles de la aplicacion");
                    break;
                default:
                    MessageBox.Show("No haremos ni vrg");
                    break;
            }
        }
    }
}
