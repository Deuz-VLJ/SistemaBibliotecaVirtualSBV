// Formulario FormLector.cs actualizado para usar la base de datos Biblioteca en SQL Server estándar
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
    public partial class FormLector : Form
    {
        public FormLector()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg,
        int wParam, int lParam);

        private void Mover()
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void FormLector_Load(object sender, EventArgs e)
        {
            MostrarLectores(dataGridView1);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validar que los campos estén llenos antes de guardar
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos antes de guardar el lector.",
                                "Campos vacíos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Ejecutar funciones si los datos están completos
            IngresarNuevoLector();
            MostrarLectores(dataGridView1);
            LimpiarTxt();
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Validar que los campos estén llenos antes de guardar
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos antes de eliminar el lector.",
                                "Campos vacíos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            EliminarLector();
            MostrarLectores(dataGridView1);
            LimpiarTxt();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Validar que los campos estén llenos antes de guardar
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos antes de Actulaizar el lector.",
                                "Campos vacíos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            ModificarLector();
            MostrarLectores(dataGridView1);
            LimpiarTxt();
        }

        private string Conexion()
        {
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
        }

        private void SeleccionarLector()
        {
            if (dataGridView1.CurrentRow != null)
            {
                txtId.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                txtCedula.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtNombre.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                txtTelefono.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                txtDireccion.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            }
        }

        private void IngresarNuevoLector()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("NuevoLector", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CedulaLector", txtCedula.Text);
                    cmd.Parameters.AddWithValue("@NombreLector", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@TelefonoLector", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@DireccionLector", txtDireccion.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Registro añadido exitosamente", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ha ocurrido un error: " + e.Message);
                }
            }
        }

        private void EliminarLector()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("EliminarLector", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdLector", txtId.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Registro eliminado", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ha ocurrido un error: " + e.Message);
                }
            }
        }

        private void ModificarLector()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("ModificarLector", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdLector", txtId.Text);
                    cmd.Parameters.AddWithValue("@CedulaLector", txtCedula.Text);
                    cmd.Parameters.AddWithValue("@NombreLector", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@TelefonoLector", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@DireccionLector", txtDireccion.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("REGISTRO ACTUALIZADO", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ha ocurrido un error: " + e.Message);
                }
            }
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

        private void BuscarLector(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("BuscarLector", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreLector" +
                    "", txtBuscar.Text);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void LimpiarTxt()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtCedula.Clear();
            txtDireccion.Clear();
            txtBuscar.Clear();
            txtTelefono.Clear();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                MostrarLectores(dataGridView1);
            }
            else
            {
                BuscarLector(dataGridView1);
            }
        }

        private void FormLector_FormClosing(object sender, FormClosingEventArgs e)
        {
            MenuPrincipal MP = new MenuPrincipal();
            MP.Show();
            this.Hide();
        }

        private void FormLector_FormClosed(object sender, FormClosedEventArgs e)
        {
            MenuPrincipal MP = new MenuPrincipal();
            MP.Show();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarLector();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }
    }
}
