// Formulario FormLibro.cs adaptado a SQL Server estándar sin uso de SQLEXPRESS
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
using System.Xml.Linq;

namespace SistemaBibliotecaVirtualSBV
{
    public partial class FormLibro : Form
    {
        public FormLibro()
        {
            InitializeComponent();
        }

        private string Conexion()
        {
            return "Data Source=localhost;Initial Catalog=BibliotecaBurrion;Integrated Security=True";
        }

        private void IngresarNuevoLibro()
        {
            SqlConnection con = new SqlConnection(Conexion());
            try
            {
                SqlCommand cmd = new SqlCommand("InsertarLibroDesdeFormulario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Titulo", txtNombreLibro.Text);
                cmd.Parameters.AddWithValue("@Autor", txtAutor.Text);
                cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text);

                int numeroPaginas = 0;
                int.TryParse(txtNumPagina.Text, out numeroPaginas);
                cmd.Parameters.AddWithValue("@NumeroPaginas", numeroPaginas);

                cmd.Parameters.AddWithValue("@Editorial", txtCodigoEditorial.Text);

                DateTime fechaEdicion;
                if (!DateTime.TryParse(txtFechaEdicion.Text, out fechaEdicion))
                    fechaEdicion = DateTime.Now;
                cmd.Parameters.AddWithValue("@FechaEdicion", fechaEdicion);

                DateTime fechaPublicacion;
                if (!DateTime.TryParse(txtFechaPublicacion.Text, out fechaPublicacion))
                    fechaPublicacion = DateTime.Now;
                cmd.Parameters.AddWithValue("@FechaPublicacion", fechaPublicacion);

                cmd.Parameters.AddWithValue("@Edicion", txtEdicion.Text);
                cmd.Parameters.AddWithValue("@Idioma", txtIdioma.Text);

                int numeroEjemplares = 0;
                int.TryParse(txtEjemplares.Text, out numeroEjemplares);
                cmd.Parameters.AddWithValue("@NumeroEjemplares", numeroEjemplares);

                // Agregado: parámetro requerido VecesPrestadas
                cmd.Parameters.AddWithValue("@VecesPrestadas", 0); // Puedes ajustar si el formulario lo incluye

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Registro añadido exitosamente", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ha ocurrido un error: " + e.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void ModificarLibro()
        {
            SqlConnection con = new SqlConnection(Conexion());
            try
            {
                SqlCommand cmd = new SqlCommand("ModificarLibroDesdeFormulario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdLibro", txtIdLibro.Text);
                cmd.Parameters.AddWithValue("@Titulo", txtNombreLibro.Text);
                cmd.Parameters.AddWithValue("@Autor", txtAutor.Text);
                cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text);
                cmd.Parameters.AddWithValue("@NumeroPaginas", txtNumPagina.Text);
                cmd.Parameters.AddWithValue("@Editorial", txtCodigoEditorial.Text);
                cmd.Parameters.AddWithValue("@FechaEdicion", txtFechaEdicion.Value);
                cmd.Parameters.AddWithValue("@FechaPublicacion", txtFechaPublicacion.Value);
                cmd.Parameters.AddWithValue("@Edicion", txtEdicion.Text);
                cmd.Parameters.AddWithValue("@Idioma", txtIdioma.Text);
                cmd.Parameters.AddWithValue("@NumeroEjemplares", txtEjemplares.Text);

                int vecesPrestadas = 0;
                int.TryParse(txtEjemplares?.Text, out vecesPrestadas);
                cmd.Parameters.AddWithValue("@VecesPrestadas", vecesPrestadas);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Registro modificado exitosamente", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ha ocurrido un error: " + e.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }


        private void EliminarLibro()
        {
            SqlConnection con = new SqlConnection(Conexion());
            try
            {
                SqlCommand cmd = new SqlCommand("EliminarLibroDesdeFormulario", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLibro", txtIdLibro.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Registro eliminado exitosamente", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ha ocurrido un error" + e.Message);
                con.Close();
            }
        }

        private void MostrarLibros(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT 
                L.IdLibro,
                L.Titulo,
                A.Autor,
                C.Categoria,
                L.NumeroPaginas,
                E.NombreEditorial AS Editorial,
                L.AñoEdicion AS FechaEdicion,
                L.FechaPublicacion,
                L.Edicion,
                L.Idioma,
                L.NumeroEjemplares
            FROM Libros L
            INNER JOIN Autor A ON L.IdAutor = A.IdAutor
            INNER JOIN Categorias C ON L.IdCategoria = C.IdCategoria
            INNER JOIN Editorial E ON L.IdEditorial = E.IdEditorial
        ", con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;
            }
        }


        private void LimpiarTxt()
        {
            txtAutor.Clear();
            txtCategoria.Text = "";
            txtCodigoEditorial.Clear();
            txtEdicion.Text = "";
            txtEjemplares.Clear();
            txtFechaEdicion.Text = "";
            txtFechaPublicacion.Text = "";
            txtIdioma.Clear();
            txtIdLibro.Clear();
            txtNombreLibro.Clear();
            txtNumPagina.Clear();
        }

        private void BuscarLibro(DataGridView dg)
        {
            using (SqlConnection con = new SqlConnection(Conexion()))
            {
                SqlCommand cmd = new SqlCommand("BuscarLibro", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Titulo", txtBuscar.Text.Trim());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg.DataSource = dt;

                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    MostrarLibros(dg);
                }
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns.Count > 10)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                txtIdLibro.Text = fila.Cells[0]?.Value?.ToString() ?? "";
                txtNombreLibro.Text = fila.Cells[1]?.Value?.ToString() ?? "";
                txtAutor.Text = fila.Cells[2]?.Value?.ToString() ?? "";
                txtCategoria.Text = fila.Cells[3]?.Value?.ToString() ?? "";
                txtNumPagina.Text = fila.Cells[4]?.Value?.ToString() ?? "";
                txtCodigoEditorial.Text = fila.Cells[5]?.Value?.ToString() ?? "";

                if (DateTime.TryParse(fila.Cells[6]?.Value?.ToString(), out DateTime fechaEdicion))
                    txtFechaEdicion.Value = fechaEdicion;
                else
                    txtFechaEdicion.Value = DateTime.Now;

                if (DateTime.TryParse(fila.Cells[7]?.Value?.ToString(), out DateTime fechaPublicacion))
                    txtFechaPublicacion.Value = fechaPublicacion;
                else
                    txtFechaPublicacion.Value = DateTime.Now;

                txtEdicion.Text = fila.Cells[8]?.Value?.ToString() ?? "";
                txtIdioma.Text = fila.Cells[9]?.Value?.ToString() ?? "";
                txtEjemplares.Text = fila.Cells[10]?.Value?.ToString() ?? "";
            }
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            IngresarNuevoLibro();
            LimpiarTxt();
            MostrarLibros(dataGridView1);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            EliminarLibro();
            LimpiarTxt();
            MostrarLibros(dataGridView1);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ModificarLibro();
            LimpiarTxt();
            MostrarLibros(dataGridView1);
        }

        private void FormLibro_Load(object sender, EventArgs e)
        {
            MostrarLibros(dataGridView1);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarLibro(dataGridView1);
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠ Por favor selecciona una fila completa del DataGridView.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow fila = dataGridView1.SelectedRows[0];

            txtIdLibro.Text = fila.Cells["IdLibro"].Value?.ToString() ?? "";
            txtNombreLibro.Text = fila.Cells["Titulo"].Value?.ToString() ?? "";
            txtAutor.Text = fila.Cells["Autor"].Value?.ToString() ?? "";
            txtCategoria.Text = fila.Cells["Categoria"].Value?.ToString() ?? "";
            txtNumPagina.Text = fila.Cells["NumeroPaginas"].Value?.ToString() ?? "";
            txtCodigoEditorial.Text = fila.Cells["Editorial"].Value?.ToString() ?? "";

            if (DateTime.TryParse(fila.Cells["FechaEdicion"].Value?.ToString(), out DateTime fechaEdicion))
                txtFechaEdicion.Value = fechaEdicion;
            else
                txtFechaEdicion.Value = DateTime.Now;

            if (DateTime.TryParse(fila.Cells["FechaPublicacion"].Value?.ToString(), out DateTime fechaPublicacion))
                txtFechaPublicacion.Value = fechaPublicacion;
            else
                txtFechaPublicacion.Value = DateTime.Now;

            txtEdicion.Text = fila.Cells["Edicion"].Value?.ToString() ?? "";
            txtIdioma.Text = fila.Cells["Idioma"].Value?.ToString() ?? "";
            txtEjemplares.Text = fila.Cells["NumeroEjemplares"].Value?.ToString() ?? "";
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }

        private void FormLibro_FormClosing(object sender, FormClosingEventArgs e)
        {
            MenuPrincipal MenPri = new MenuPrincipal();
            this.Hide();
            MenPri.Show();
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            // Validar campos vacíos antes de guardar
            if (string.IsNullOrWhiteSpace(txtNombreLibro.Text) ||
                string.IsNullOrWhiteSpace(txtAutor.Text) ||
                string.IsNullOrWhiteSpace(txtCodigoEditorial.Text) )
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos obligatorios antes de guardar.",
                                "Campos incompletos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Si todos los campos están llenos
            IngresarNuevoLibro();
            LimpiarTxt();
            MostrarLibros(dataGridView1);
        }

        private void btnSeleccionar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("⚠ Por favor selecciona una fila del DataGridView.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow fila = dataGridView1.CurrentRow;

            if (fila.Cells.Count < 11)
            {
                MessageBox.Show("⚠ La fila seleccionada no tiene suficientes columnas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtIdLibro.Text = fila.Cells[0]?.Value?.ToString() ?? "";
            txtNombreLibro.Text = fila.Cells[1]?.Value?.ToString() ?? "";
            txtAutor.Text = fila.Cells[2]?.Value?.ToString() ?? "";
            txtCategoria.Text = fila.Cells[3]?.Value?.ToString() ?? "";
            txtNumPagina.Text = fila.Cells[4]?.Value?.ToString() ?? "";
            txtCodigoEditorial.Text = fila.Cells[5]?.Value?.ToString() ?? "";

            if (DateTime.TryParse(fila.Cells[6]?.Value?.ToString(), out DateTime fechaEdicion))
                txtFechaEdicion.Value = fechaEdicion;
            else
                txtFechaEdicion.Value = DateTime.Now;

            if (DateTime.TryParse(fila.Cells[7]?.Value?.ToString(), out DateTime fechaPublicacion))
                txtFechaPublicacion.Value = fechaPublicacion;
            else
                txtFechaPublicacion.Value = DateTime.Now;

            txtEdicion.Text = fila.Cells[8]?.Value?.ToString() ?? "";
            txtIdioma.Text = fila.Cells[9]?.Value?.ToString() ?? "";
            txtEjemplares.Text = fila.Cells[10]?.Value?.ToString() ?? "";
        }

        private void btnActualizar_Click_1(object sender, EventArgs e)
        {
            // Validar campos vacíos antes de guardar
            if (string.IsNullOrWhiteSpace(txtNombreLibro.Text) ||
                string.IsNullOrWhiteSpace(txtAutor.Text) ||
                string.IsNullOrWhiteSpace(txtCodigoEditorial.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos obligatorios antes de guardar.",
                                "Campos incompletos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            ModificarLibro();
            LimpiarTxt();
            MostrarLibros(dataGridView1);
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            // Validar campos vacíos antes de guardar
            if (string.IsNullOrWhiteSpace(txtNombreLibro.Text) ||
                string.IsNullOrWhiteSpace(txtAutor.Text) ||
                string.IsNullOrWhiteSpace(txtCodigoEditorial.Text))
            {
                MessageBox.Show("⚠️ Por favor complete todos los campos obligatorios antes de guardar.",
                                "Campos incompletos",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            EliminarLibro();
            MostrarLibros(dataGridView1);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           // SeleccionarDatosDg();
        }
    }
}
