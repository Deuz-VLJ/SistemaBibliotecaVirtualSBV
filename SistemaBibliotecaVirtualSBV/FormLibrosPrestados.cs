using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace SistemaBibliotecaVirtualSBV
{
    public partial class FormLibrosPrestados : Form
    {
        public FormLibrosPrestados()
        {
            InitializeComponent();
        }

        private string Conexion()
        {
            // Actualizado para usar la base de datos estándar "Biblioteca"
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Mover()
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void MostrarLibrosPrestados(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("MostrarPrestamosDetallado", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;

                // Pintar filas según estado
                foreach (DataGridViewRow row in dg.Rows)
                {
                    string estado = row.Cells["EstadoPrestamo"].Value?.ToString();
                    if (estado == "Devuelto")
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    else if (estado == "Pendiente")
                        row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                    else if (estado == "Atrasado")
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }
        }




        //nuevo
        private void BuscarLibroPrestado(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("BuscarPrestamoDetallado", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPrestamo", txtBuscarPrestamo.Text);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
               da.Fill(dt);

                dg.AutoGenerateColumns = true; // Muy importante
                dg.DataSource = dt;
            }
        }

        private void FormLibrosPrestados_Load(object sender, EventArgs e)
        {
            dataGridPrestamoLibros.AutoGenerateColumns = true;
            MostrarLibrosPrestados(dataGridPrestamoLibros);
        }

        private void FormLibrosPrestados_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }

        private void btnBuscarPrestamo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarPrestamo.Text))
            {
                MessageBox.Show("El campo no puede estar vacío.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MostrarLibrosPrestados(dataGridPrestamoLibros);
            }
            else
            {
                BuscarLibroPrestado(dataGridPrestamoLibros);
            }
        }

        private void FormLibrosPrestados_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Puedes implementar lógica adicional aquí si es necesario
        }
    }
}