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
    public partial class FormDevolucion : Form
    {
        public FormDevolucion()
        {
            InitializeComponent();
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

        private string Conexion()
        {
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
        }

        private void SeleccionarPrestamo()
        {
            FormSeleccionarPrestamo FormSelecPres = new FormSeleccionarPrestamo();
            lblNumeroPrestamo.Text = FormSelecPres.dataGridPrestamoLibros.Rows[FormSelecPres.dataGridPrestamoLibros.CurrentRow.Index].Cells[0].Value.ToString();
        }

        private void MostrarLibrosPrestados(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Prestamo", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void DevolverLibro()
        {
            if (!int.TryParse(lblNumeroPrestamo.Text, out int idPrestamo) ||
                !int.TryParse(lblIdLibro.Text, out int idLibro))
            {
                MessageBox.Show("ID de préstamo o libro no válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    con.Open();

                    // Verificar si ya existe una devolución para evitar duplicados
                    SqlCommand verificar = new SqlCommand(@"
                SELECT COUNT(*) FROM Devolucion 
                WHERE IdPrestamo = @IdPrestamo AND IdLibro = @IdLibro", con);
                    verificar.Parameters.AddWithValue("@IdPrestamo", idPrestamo);
                    verificar.Parameters.AddWithValue("@IdLibro", idLibro);

                    int existe = (int)verificar.ExecuteScalar();
                    if (existe > 0)
                    {
                        MessageBox.Show("⚠️ Este libro ya ha sido devuelto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Procedimiento almacenado para registrar devolución
                    SqlCommand cmd = new SqlCommand("DevolverLibro", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdPrestamo1", idPrestamo);
                    cmd.Parameters.AddWithValue("@IdLibro2", idLibro);
                    cmd.Parameters.AddWithValue("@FechaDevolucion", dateTimeEntrega.Value.Date);
                    cmd.Parameters.AddWithValue("@ObservacionDevolucion", txtObservacion.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("✅ Libro devuelto correctamente.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al devolver el libro: " + e.Message, "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void EliminarLibro()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("EliminarPrestamo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPrestamo1", lblNumeroPrestamo.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void FormDevolucion_Load(object sender, EventArgs e)
        {
            dateTimeEntrega.Enabled = false;
            dateTimeEntrega.Value = DateTime.Today;
            dateTimeEntrega.MinDate = DateTime.Today;
            dateTimeEntrega.MaxDate = DateTime.Today;
            dateTimeEntrega.Format = DateTimePickerFormat.Short; // Opcional: formato corto (dd/MM/yyyy)
        }

        private void btnSelecPrestamo_Click(object sender, EventArgs e)
        {
            FormSeleccionarPrestamo fsp = new FormSeleccionarPrestamo();
            fsp.ShowDialog();

            if (fsp.DialogResult == DialogResult.OK)
            {
                lblNumeroPrestamo.Text = fsp.dataGridPrestamoLibros.Rows[fsp.dataGridPrestamoLibros.CurrentRow.Index].Cells[0].Value.ToString();
                lblIdLibro.Text = fsp.dataGridPrestamoLibros.Rows[fsp.dataGridPrestamoLibros.CurrentRow.Index].Cells[4].Value.ToString();
                txtObservacion.Focus();
            }
        }

        private void LimpiarDatos()
        {
            txtObservacion.Clear();
            lblIdLibro.Text = "";
            lblNumeroPrestamo.Text = "";
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            DevolverLibro();
            LimpiarDatos();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }

        private void FormDevolucion_FormClosing(object sender, FormClosingEventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }

        private void panelCabecera_MouseDown(object sender, MouseEventArgs e)
        {
            Mover();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            FormHistorialDevoluciones FHD = new FormHistorialDevoluciones();
            FHD.Show();
            this.Hide();
        }
    }
}
