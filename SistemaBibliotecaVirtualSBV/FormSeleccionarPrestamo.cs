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

namespace SistemaBibliotecaVirtualSBV
{
    public partial class FormSeleccionarPrestamo : Form
    {
        public FormSeleccionarPrestamo()
        {
            InitializeComponent();
        }

        private void FormSeleccionarPrestamo_Load(object sender, EventArgs e)
        {
            MostrarLibrosPrestados(dataGridPrestamoLibros);
        }

        private string Conexion()
        {
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
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

                // Aplicar colores según el estado del préstamo
                foreach (DataGridViewRow row in dg.Rows)
                {
                    string estado = row.Cells["EstadoPrestamo"]?.Value?.ToString();
                    if (estado == "Atrasado")
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    else if (estado == "Pendiente")
                        row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                    else if (estado == "Devuelto")
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    else
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }


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

                dg.AutoGenerateColumns = true;
                dg.DataSource = dt;

                // Aplicar colores según estado
                foreach (DataGridViewRow row in dg.Rows)
                {
                    string estado = row.Cells["EstadoPrestamo"]?.Value?.ToString();
                    if (estado == "Atrasado")
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    else if (estado == "Pendiente")
                        row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                    else if (estado == "Devuelto")
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    else
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }



        private void btnSelecPrestamo_Click(object sender, EventArgs e)
        {
            if (dataGridPrestamoLibros.Rows.Count == 0)
            {
                return;
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnBuscarPrestamo_Click(object sender, EventArgs e)
        {
            BuscarLibroPrestado(dataGridPrestamoLibros);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
