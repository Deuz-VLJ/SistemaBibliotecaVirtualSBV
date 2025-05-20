// Formulario FormUsuario.cs adaptado a la base de datos Biblioteca en SQL Server estándar
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaBibliotecaVirtualSBV
{
    public partial class FormUsuario : Form
    {
        public FormUsuario()
        {
            InitializeComponent();
        }

        private string Conexion()
        {
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
        }

        private void SeleccionarUsuario()
        {
            txtCodigo.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtNombre.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtCuenta.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtContraseña.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();

            string tipo = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            if (!comboTipoUsuario.Items.Contains(tipo))
            {
                comboTipoUsuario.Items.Clear();
                comboTipoUsuario.Items.Add("Admin");
                comboTipoUsuario.Items.Add("Bibliotecario");
            }

            comboTipoUsuario.SelectedItem = tipo;
        }


        private void MostrarUsuarios(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT IdUsuario, NombreCompleto, Cuenta, Contraseña, TipoUsuario FROM Usuarios", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void InsertarUsuario()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("InsertarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Cuenta", txtCuenta.Text);
                    cmd.Parameters.AddWithValue("@Contraseña", txtContraseña.Text);
                    cmd.Parameters.AddWithValue("@NombreCompleto", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@TipoUsuario", comboTipoUsuario.SelectedItem?.ToString() ?? "Bibliotecario");
                   

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario registrado exitosamente.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al registrar usuario: " + e.Message);
                }
            }
        }

        private void ActualizarUsuario()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("ActualizarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", txtCodigo.Text);
                    cmd.Parameters.AddWithValue("@Cuenta", txtCuenta.Text);
                    cmd.Parameters.AddWithValue("@Contraseña", txtContraseña.Text);
                    cmd.Parameters.AddWithValue("@NombreCompleto", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@TipoUsuario", comboTipoUsuario.SelectedItem?.ToString() ?? "Bibliotecario");

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario actualizado exitosamente.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al actualizar usuario: " + e.Message);
                }
            }
        }

        private void EliminarUsuario()
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("EliminarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", txtCodigo.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario eliminado exitosamente.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al eliminar usuario: " + e.Message);
                }
            }
        }

        private void BuscarUsuario(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("BuscarUsuario", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreUsuario", txtBuscar.Text.Trim());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void LimpiarTxt()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtCuenta.Clear();
            txtContraseña.Clear();
            comboTipoUsuario.SelectedIndex = -1;
        }

        private void FormUsuario_Load(object sender, EventArgs e)
        {
            MostrarUsuarios(dataGridView1);
            txtContraseña.UseSystemPasswordChar = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validar que los campos estén llenos antes de guardar
            if (string.IsNullOrWhiteSpace(txtCuenta.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Text) ||
                
                string.IsNullOrWhiteSpace(comboTipoUsuario.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos antes de guardar el usuario.",
                                "Campos vacíos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Si todo está correcto, guardar
            InsertarUsuario();
            MostrarUsuarios(dataGridView1);
            LimpiarTxt();
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Validar que los campos estén llenos antes de guardar
            if (string.IsNullOrWhiteSpace(txtCuenta.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(comboTipoUsuario.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos antes de eliminar el usuario.",
                                "Campos vacíos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            EliminarUsuario();
            MostrarUsuarios(dataGridView1);
            LimpiarTxt();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Validar que los campos estén llenos antes de guardar
            if (string.IsNullOrWhiteSpace(txtCuenta.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Text) ||
               
                string.IsNullOrWhiteSpace(comboTipoUsuario.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos antes de eliminar el usuario.",
                                "Campos vacíos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            ActualizarUsuario();
            MostrarUsuarios(dataGridView1);
            LimpiarTxt();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtContraseña.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarUsuario();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                MostrarUsuarios(dataGridView1);
            }
            else
            {
                BuscarUsuario(dataGridView1);
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }

        private void FormUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Close();
            MenPri.Show();
        }
    }
}
