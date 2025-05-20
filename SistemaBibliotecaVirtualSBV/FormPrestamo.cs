// Formulario FormPrestamo actualizado para usar la base de datos Biblioteca en SQL Server estándar
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
    public partial class FormPrestamo : Form
    {
        public FormPrestamo()
        {
            InitializeComponent();
        }

        private string Conexion()
        {
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

        private void SeleccionarLector()
        {
            if (dataGridLectores.CurrentRow != null)
            {
                txtCodigoElector.Text = dataGridLectores.CurrentRow.Cells[0].Value.ToString();
                txtElector.Text = dataGridLectores.CurrentRow.Cells[2].Value.ToString();
            }
        }

        private void SeleccionarLibro()
        {
            if (dataGridLibros.CurrentRow != null)
            {
                txtCodigoLibro.Text = dataGridLibros.CurrentRow.Cells[0].Value.ToString();
                txtLibro.Text = dataGridLibros.CurrentRow.Cells[1].Value.ToString();
                cbbEdicion.Text = dataGridLibros.CurrentRow.Cells[3].Value.ToString();
            }
        }

        private void LimpiarTxt()
        {
            txtCodigoElector.Clear();
            txtCodigoLibro.Clear();
            txtBuscarLibro.Clear();
            txtBuscarElector.Clear();
            txtElector.Clear();
            txtLibro.Clear();
            cbbEdicion.SelectedIndex = -1;
        }

        private void MostrarLectores(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Lectores", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void MostrarLibros(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Libros", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void GuardarPrestamoLibro()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    if (!int.TryParse(txtCodigoLibro.Text, out int idLibro) ||
                        !int.TryParse(txtCodigoElector.Text, out int idLector))
                    {
                        MessageBox.Show("ID de libro o lector no válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DateTime fechaSalida = DateTime.Now.Date;
                    DateTime fechaDevolucion = dateTimeEntrega.Value.Date;

                    if (fechaDevolucion <= fechaSalida)
                    {
                        MessageBox.Show("La fecha de devolución debe ser al menos un día después de hoy.", "Error de fecha", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    con.Open();

                    // Verificar ejemplares disponibles
                    SqlCommand checkCmd = new SqlCommand("SELECT NumeroEjemplares FROM Libros WHERE IdLibro = @IdLibro", con);
                    checkCmd.Parameters.AddWithValue("@IdLibro", idLibro);
                    object result = checkCmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Libro no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int ejemplaresDisponibles = Convert.ToInt32(result);
                    if (ejemplaresDisponibles <= 0)
                    {
                        MessageBox.Show("No hay ejemplares disponibles para este libro.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Realizar el préstamo
                    SqlCommand cmd = new SqlCommand("GuardarPrestamo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdLibro", idLibro);
                    cmd.Parameters.AddWithValue("@FechaSalida", fechaSalida);
                    cmd.Parameters.AddWithValue("@FechaDevolucion", fechaDevolucion);
                    cmd.Parameters.AddWithValue("@IdLector", idLector);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Se ha generado el préstamo del libro.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar el préstamo: " + ex.Message, "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void btnSelecLibro_Click(object sender, EventArgs e)
        {
            SeleccionarLibro();
        }

        private void btnSelecElector_Click(object sender, EventArgs e)
        {
            SeleccionarLector();
        }

        private void FormPrestamo_Load(object sender, EventArgs e)
        {
            MostrarLectores(dataGridLectores);
            MostrarLibros(dataGridLibros);

            // Establecer la fecha de salida como hoy y bloquearla
            dateTimeSalida.Value = DateTime.Today;
            dateTimeSalida.MinDate = DateTime.Today;
            dateTimeSalida.MaxDate = DateTime.Today;
            dateTimeSalida.Enabled = false; // Opcional: deshabilitar para que no se edite

            // Establecer fecha de entrega mínima como mañana
            dateTimeEntrega.Value = DateTime.Today.AddDays(1);
            dateTimeEntrega.MinDate = DateTime.Today.AddDays(1);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }

        private void btnPrestar_Click(object sender, EventArgs e)
        {
            if (!LibroDisponible(txtCodigoLibro.Text))
            {
                return;
            }

            GuardarPrestamoLibro();
            ActualizarEjemplares(txtCodigoLibro.Text);
            MostrarLectores(dataGridLectores);
            MostrarLibros(dataGridLibros);
            LimpiarTxt();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarTxt();
            MostrarLectores(dataGridLectores);
            MostrarLibros(dataGridLibros);
        }

        //funcionalidad
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is FormLibrosPrestados)
                {
                    form.Close(); // Cierra el formulario si ya está abierto
                    break;
                }
            }

            FormLibrosPrestados nuevo = new FormLibrosPrestados();
            nuevo.Show();
        }


        private void panelCabecera_MouseDown(object sender, MouseEventArgs e)
        {
            Mover();
        }


        //nuevo
        private void btnBuscarLibro_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarLibro.Text))
            {
                MostrarLibros(dataGridLibros);
                return;
            }

            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Libros WHERE Titulo LIKE @Titulo", con);
                cmd.Parameters.AddWithValue("@Titulo", "%" + txtBuscarLibro.Text.Trim() + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridLibros.DataSource = dt;
            }
        }

        //nuevo
        private void btnBuscarEector_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarElector.Text))
            {
                MostrarLectores(dataGridLectores);
                return;
            }

            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Lectores WHERE NombreLector LIKE @Nombre", con);
                cmd.Parameters.AddWithValue("@Nombre", "%" + txtBuscarElector.Text.Trim() + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridLectores.DataSource = dt;
            }
        }

        //nuevo
        private bool LibroDisponible(string idLibro)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                string query = "SELECT NumeroEjemplares FROM Libros WHERE IdLibro = @IdLibro";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdLibro", idLibro);

                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result != null && int.TryParse(result.ToString(), out int ejemplares))
                {
                    if (ejemplares > 0)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("❌ El libro no está disponible. No hay ejemplares.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("⚠️ No se pudo verificar la disponibilidad del libro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        //nuevo
        private void ActualizarEjemplares(string idLibro)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                string query = "UPDATE Libros SET NumeroEjemplares = NumeroEjemplares - 1 WHERE IdLibro = @IdLibro AND NumeroEjemplares > 0";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdLibro", idLibro);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }





    }
}
