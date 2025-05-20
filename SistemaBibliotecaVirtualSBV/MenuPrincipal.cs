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
    public partial class MenuPrincipal : Form
    {
        // 📌 1️⃣ Cadena de conexión a SQL Server (Asegúrate de cambiar el servidor y las credenciales si es necesario)
        private string connectionString = "Server=localhost;Database=BibliotecaBurrion;User Id=sa;Password=J17u20a04n7;";

        public MenuPrincipal()
        {
            InitializeComponent();
        }

        // 📌 2️⃣ Permitir mover el formulario con el mouse
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Mover()
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            Mover();
        }

        // 📌 3️⃣ Método para mostrar los libros en el DataGridView
        private void MostrarLibros()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT IdLibro, Titulo, AñoEdicion, VecesPrestadas, IdEstado FROM Libros";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error al cargar libros: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 📌 4️⃣ Cargar libros al iniciar el formulario
        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            MostrarLibros();

            if (sesion.TipoUsuario == "Bibliotecario")
            {
                btnLibros.Enabled = false;
                btnUsuarios.Enabled = false;
                btnLector.Enabled = false;
            }
        }

        // 📌 5️⃣ Abrir formularios de la biblioteca
        private void btnLector_Click(object sender, EventArgs e)
        {
            FormLector FL = new FormLector();
            this.Hide();
            FL.Show();
        }

        private void btnLibros_Click(object sender, EventArgs e)
        {
            FormLibro FL = new FormLibro();
            this.Hide();
            FL.Show();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            FormUsuario FormUsu = new FormUsuario();
            this.Hide();
            FormUsu.Show();
        }

        private void btnPrestar_Click(object sender, EventArgs e)
        {
            FormPrestamo FP = new FormPrestamo();
            this.Hide();
            FP.Show();
        }

        private void btnDevolverLibro_Click(object sender, EventArgs e)
        {
            FormDevolucion FormDev = new FormDevolucion();
            this.Hide();
            FormDev.Show();
        }

        // 📌 6️⃣ Manejo del botón de registrar (abre/cierra menú)
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            panelSubMenuRegistrar.Visible = !panelSubMenuRegistrar.Visible;
        }

        // 📌 7️⃣ Cerrar sesión (volver al login)
        private void CerrarSesion_Click(object sender, EventArgs e)
        {
            FormLogin FL = new FormLogin();
            FL.Show();
            this.Hide();
        }

        // 📌 8️⃣ Manejo de cierre del formulario
        private void MenuPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // 📌 9️⃣ Botones de cerrar y minimizar
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
