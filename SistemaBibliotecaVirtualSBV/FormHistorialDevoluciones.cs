using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace SistemaBibliotecaVirtualSBV
{
    public partial class FormHistorialDevoluciones : Form
    {
        public FormHistorialDevoluciones()
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

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Mover();
        }

        private string Conexion()
        {
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
        }

        private void MostrarHistorial(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Devolucion", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }

        private void BuscarHistorialLibros(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT D.IdDevolucion, D.IdPrestamo, D.IdLibro, L.Titulo, D.FechaDevolucion, D.ObservacionDevolucion
            FROM Devolucion D
            INNER JOIN Libros L ON D.IdLibro = L.IdLibro
            WHERE L.Titulo LIKE @Titulo", con);

                cmd.Parameters.AddWithValue("@Titulo", "%" + txtBuscar.Text.Trim() + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }



        private void FormHistorialDevoluciones_Load(object sender, EventArgs e)
        {
            MostrarHistorial(dataGridView1);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                BuscarHistorialLibros(dataGridView1);
            else
                MostrarHistorial(dataGridView1);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            FormDevolucion FD = new FormDevolucion();
            this.Hide();
            FD.Show();
        }
    }
}
