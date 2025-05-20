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
using static System.Collections.Specialized.BitVector32;

namespace SistemaBibliotecaVirtualSBV
{
    public partial class FormLogin : Form
    {
        // 📌 1️⃣ Cadena de conexión (ajusta según tu servidor)
        private string connectionString = "Server=localhost;Database=BibliotecaBurrion;User Id=sa;Password=J17u20a04n7";

        // Permite mover el formulario con el mouse
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public FormLogin()
        {
            InitializeComponent();
        }

        // 📌 2️⃣ Método para mover el formulario
        private void Mover()
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void PanelCabecera_MouseDown(object sender, MouseEventArgs e)
        {
            Mover();
        }

        // 📌 3️⃣ Botón salir
        private void Salir()
        {
            Application.Exit();
        }

        private void btsalir_Click(object sender, EventArgs e)
        {
            Salir();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // 📌 4️⃣ Evento al cargar el formulario (verifica la conexión)
        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    MessageBox.Show("✅ Conexión exitosa con SQL Server!", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error de conexión: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 📌 5️⃣ Método para validar usuario en SQL Server
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Consulta para validar usuario y obtener su rol
                    string query = "SELECT Cuenta, Contraseña, NombreCompleto, TipoUsuario FROM Usuarios WHERE Cuenta = @usuario AND Contraseña = @contraseña";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Parámetros seguros para evitar inyección SQL
                        cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text.Trim());
                        cmd.Parameters.AddWithValue("@contraseña", txtContraseña.Text.Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Usuario encontrado
                            {
                                // Guardar los datos del usuario en la sesión
                                sesion.Cuenta = reader["Cuenta"].ToString();
                                sesion.NombreCompleto = reader["NombreCompleto"].ToString();
                                sesion.TipoUsuario = reader["TipoUsuario"].ToString();

                                MessageBox.Show($"✅ Bienvenido, {sesion.NombreCompleto}!", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Redirigir al menú principal
                                MenuPrincipal menu = new MenuPrincipal();
                                this.Hide();
                                menu.Show();
                            }
                            else
                            {
                                MessageBox.Show("❌ Usuario o contraseña incorrectos.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtContraseña.Clear();
                                txtUsuario.Focus();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error al iniciar sesión:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            Salir();
        }
    }
}
