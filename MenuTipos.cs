using PdfSharp.Pdf.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_restaurante
{
    public partial class MenuTipos : Form
    {
        public MenuTipos()
        {
            InitializeComponent();
        }

        string conexionString = ConexionBD.ConexionSQL();
        private string CategoriaId = "";
        private string TipoDocID = "";
        private string ProductoTipoId = "";
        private string MetodoPagoId = "";
        private string DepaID = "";
        private string PuestoID = "";
        private string MotivoId = "";
        private string UnidadID = "";

        private void button4_Click(object sender, EventArgs e)
        {
            categpanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            categpanel.BringToFront();
            categpanel.Visible = true;

            CP_CargarGrid();
            CP_Limpiar();
        }

        private void CP_CargarGrid()
        {
            string textoFiltro = categbuscar?.Text?.Trim() ?? "";
            bool soloActivos = (categfiltrochk != null && categfiltrochk.Checked);


            string sql = @"
                            SELECT IdCategoria, Nombre, Activo
                            FROM dbo.CategoriaProducto
                            WHERE (@f = '' OR Nombre LIKE '%' + @f + '%')"
                                        + (soloActivos ? " AND Activo = 1" : "") +
                                    @"
                            ORDER BY Activo DESC, Nombre;";

            using (var cn = new SqlConnection(conexionString))
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@f", textoFiltro);

                var dt = new DataTable();
                da.Fill(dt);

                categdt.AutoGenerateColumns = true;
                categdt.DataSource = dt;


                if (categdt.Columns.Contains("IdCategoria"))
                {
                    var c = categdt.Columns["IdCategoria"];
                    c.HeaderText = "ID";
                    c.Width = 70;
                    c.ReadOnly = true;
                }
                if (categdt.Columns.Contains("Nombre"))
                    categdt.Columns["Nombre"].HeaderText = "Categoría";
                if (categdt.Columns.Contains("Activo"))
                    categdt.Columns["Activo"].HeaderText = "Activo";
            }
        }

        private void CP_Limpiar()
        {
            CategoriaId = "";
            if (idcateg != null) idcateg.Text = "";
            if (categtxt != null) categtxt.Text = "";
            if (estadocateg != null) estadocateg.Checked = true;
            categtxt?.Focus();
        }

        private bool CP_Validar()
        {
            if (categtxt == null || string.IsNullOrWhiteSpace(categtxt.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                categtxt?.Focus();
                return false;
            }

            if (CP_ExisteNombre(categtxt.Text, CategoriaId))
            {
                MessageBox.Show("Ya existe una categoría con ese nombre.");
                categtxt?.Focus();
                return false;
            }
            return true;
        }

        private bool CP_ExisteNombre(string nombre, string idActual)
        {
            string sql = @"SELECT COUNT(1) FROM dbo.CategoriaProducto
                   WHERE Nombre = @n AND (@id = '' OR IdCategoria <> @idint);";

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@n", (nombre ?? "").Trim());
                int idint = 0; int.TryParse(idActual, out idint);
                cmd.Parameters.AddWithValue("@id", idActual ?? "");
                cmd.Parameters.AddWithValue("@idint", idint);

                cn.Open();
                int n = Convert.ToInt32(cmd.ExecuteScalar());
                return n > 0;
            }
        }

        private void CP_Guardar()
        {
            if (!CP_Validar()) return;

            using (var cn = new SqlConnection(conexionString))
            {
                cn.Open();

                if (string.IsNullOrEmpty(CategoriaId))
                {
                    string sql = "INSERT INTO dbo.CategoriaProducto (Nombre, Activo) VALUES (@Nombre, @Activo);";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", categtxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", (estadocateg != null && estadocateg.Checked) ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Categoría registrada con éxito."
                            : "No se pudo guardar.");
                    }
                }
                else
                {
                    string sql = "UPDATE dbo.CategoriaProducto SET Nombre=@Nombre, Activo=@Activo WHERE IdCategoria=@Id;";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", int.Parse(CategoriaId));
                        cmd.Parameters.AddWithValue("@Nombre", categtxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", (estadocateg != null && estadocateg.Checked) ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Categoría actualizada con éxito."
                            : "No se pudo actualizar.");
                    }
                }
            }

            CP_Limpiar();
            CP_CargarGrid();
        }

        private void CP_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var row = categdt.Rows[rowIndex];
            if (row == null) return;

            var idObj = row.Cells["IdCategoria"]?.Value;
            CategoriaId = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            if (idcateg != null) idcateg.Text = CategoriaId;

            if (categtxt != null)
                categtxt.Text = row.Cells["Nombre"]?.Value?.ToString() ?? "";

            if (estadocateg != null && categdt.Columns.Contains("Activo"))
            {
                var actObj = row.Cells["Activo"]?.Value;
                bool activo = false;
                if (actObj != null && actObj != DBNull.Value) activo = Convert.ToBoolean(actObj);
                estadocateg.Checked = activo;
            }
            else if (estadocateg != null)
            {
                estadocateg.Checked = true;
            }
        }

        private void TipoDoc_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var fila = identdt.Rows[rowIndex];
            if (fila == null) return;

            object idObj = fila.Cells["IdTipoDocumento"].Value;
            if (idObj != null && idObj != DBNull.Value)
                TipoDocID = idObj.ToString();
            else
                TipoDocID = "";

            iddocid.Text = TipoDocID;

            object nombreObj = fila.Cells["Nombre"].Value;
            identtxt.Text = (nombreObj != null && nombreObj != DBNull.Value)
                            ? nombreObj.ToString()
                            : "";

            bool activo = false;
            object actObj = fila.Cells["Activo"].Value;
            if (actObj != null && actObj != DBNull.Value)
                activo = Convert.ToBoolean(actObj);

            if (estadoiden != null) estadoiden.Checked = activo;
        }

        private void TipoDoc_Limpiar()
        {
            TipoDocID = "";
            iddocid.Text = "";
            identtxt.Text = "";
            if (estadoiden != null) estadoiden.Checked = true;
            identtxt.Focus();
        }

        private void TipoDoc_CargarGrid()
        {
            string filtro = identbuscar.Text.Trim();
            bool soloActivos = identfiltrochk.Checked;

            string consulta = "SELECT IdTipoDocumento, Nombre, Activo FROM TipoDocumento WHERE 1=1";

            if (filtro != "")
            {
                consulta += " AND Nombre LIKE '%' + @filtro + '%'";
            }

            if (soloActivos)
            {
                consulta += " AND Activo = 1";
            }

            consulta += " ORDER BY Activo DESC, Nombre";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            using (SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion))
            {
                if (filtro != "")
                {
                    adaptador.SelectCommand.Parameters.AddWithValue("@filtro", filtro);
                }

                DataTable dt = new DataTable();
                adaptador.Fill(dt);

                identdt.AutoGenerateColumns = true;
                identdt.DataSource = dt;

                if (identdt.Columns.Contains("IdTipoDocumento"))
                {
                    var c = identdt.Columns["IdTipoDocumento"];
                    c.HeaderText = "ID";
                    c.Width = 60;
                    c.ReadOnly = true;
                }

                if (identdt.Columns.Contains("Nombre"))
                    identdt.Columns["Nombre"].HeaderText = "Tipo documento";

                if (identdt.Columns.Contains("Activo"))
                    identdt.Columns["Activo"].HeaderText = "Activo";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            idenpanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            idenpanel.BringToFront();
            idenpanel.Visible = true;

            TipoDoc_CargarGrid();
            TipoDoc_Limpiar();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            prodpanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            prodpanel.BringToFront();
            prodpanel.Visible = true;

            PT_CargarGrid();
            PT_Limpiar();
        }

        private void PT_CargarGrid()
        {
            string texto = prodbuscar?.Text?.Trim() ?? "";
            bool soloActivos = prodfiltrochk != null && prodfiltrochk.Checked;

            string sql = @"
                            SELECT IdProductoTipo, Nombre, Activo, Ingrediente, Bebida, Adicon
                            FROM dbo.ProductoTipo
                            WHERE (@f = '' OR Nombre LIKE '%' + @f + '%')"
                                        + (soloActivos ? " AND Activo = 1" : "") +
                                    @"
                            ORDER BY Activo DESC, Nombre;";

            using (var cn = new SqlConnection(conexionString))
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@f", texto);

                var dt = new DataTable();
                da.Fill(dt);

                prodtidt.AutoGenerateColumns = true;
                prodtidt.DataSource = dt;

                if (prodtidt.Columns.Contains("IdProductoTipo"))
                {
                    var c = prodtidt.Columns["IdProductoTipo"];
                    c.HeaderText = "ID";
                    c.Width = 70;
                    c.ReadOnly = true;
                }
                if (prodtidt.Columns.Contains("Nombre"))
                    prodtidt.Columns["Nombre"].HeaderText = "Tipo";
                if (prodtidt.Columns.Contains("Activo"))
                    prodtidt.Columns["Activo"].HeaderText = "Activo";
                if (prodtidt.Columns.Contains("Ingrediente"))
                    prodtidt.Columns["Ingrediente"].HeaderText = "¿Ingrediente?";
                if (prodtidt.Columns.Contains("Bebida"))
                    prodtidt.Columns["Bebida"].HeaderText = "¿Bebida?";
                if (prodtidt.Columns.Contains("Adicion"))
                    prodtidt.Columns["Adicion"].HeaderText = "¿Adicion?";
            }
        }

        private void PT_Limpiar()
        {
            ProductoTipoId = "";
            if (idprod != null) idprod.Text = "";
            if (prodtxt != null) prodtxt.Text = "";
            if (estadoprod != null) estadoprod.Checked = true;
            if (ingredientechk != null) ingredientechk.Checked = false;
            if (bebidachk != null) bebidachk.Checked = false; // por tu DEFAULT (1)
            prodtxt?.Focus();
        }

        private bool PT_Validar()
        {
            if (prodtxt == null || string.IsNullOrWhiteSpace(prodtxt.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                prodtxt?.Focus();
                return false;
            }
            if (PT_ExisteNombre(prodtxt.Text, ProductoTipoId))
            {
                MessageBox.Show("Ya existe un tipo con ese nombre.");
                prodtxt?.Focus();
                return false;
            }
            return true;
        }

        private bool PT_ExisteNombre(string nombre, string idActual)
        {
            string sql = @"SELECT COUNT(1) FROM dbo.ProductoTipo
                   WHERE Nombre = @n AND (@id = '' OR IdProductoTipo <> @idint);";
            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@n", (nombre ?? "").Trim());
                int idint = 0; int.TryParse(idActual, out idint);
                cmd.Parameters.AddWithValue("@id", idActual ?? "");
                cmd.Parameters.AddWithValue("@idint", idint);

                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void PT_Guardar()
        {
            if (!PT_Validar()) return;

            bool activo = estadoprod != null && estadoprod.Checked;
            bool ingrediente = ingredientechk != null && ingredientechk.Checked;
            bool bebida = bebidachk != null && bebidachk.Checked;
            bool adicion = adicionchk != null && adicionchk.Checked;

            using (var cn = new SqlConnection(conexionString))
            {
                cn.Open();

                if (string.IsNullOrEmpty(ProductoTipoId))
                {
                    string sql = "INSERT INTO dbo.ProductoTipo (Nombre, Activo, Ingrediente, Bebida, Adicion) VALUES (@Nombre, @Activo, @Ingrediente, @Bebida, @Adicion);";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", prodtxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", activo ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Ingrediente", ingrediente ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Bebida", bebida ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Adicion", adicion ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Tipo registrado con éxito."
                            : "No se pudo guardar.");
                    }
                }
                else
                {
                    string sql = "UPDATE dbo.ProductoTipo SET Nombre=@Nombre, Activo=@Activo, Ingrediente=@Ingrediente, Bebida=@Bebida, Adicion=@Adicion WHERE IdProductoTipo=@Id;";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", int.Parse(ProductoTipoId));
                        cmd.Parameters.AddWithValue("@Nombre", prodtxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", activo ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Ingrediente", ingrediente ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Bebida", bebida ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Adicion", adicion ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Tipo actualizado con éxito."
                            : "No se pudo actualizar.");
                    }
                }
            }

            PT_Limpiar();
            PT_CargarGrid();
        }

        private void PT_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var row = prodtidt.Rows[rowIndex];
            if (row == null) return;

            var idObj = row.Cells["IdProductoTipo"]?.Value;
            ProductoTipoId = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            if (idprod != null) idprod.Text = ProductoTipoId;

            if (prodtxt != null)
                prodtxt.Text = row.Cells["Nombre"]?.Value?.ToString() ?? "";

            if (estadoprod != null)
            {
                bool act = false;
                var actObj = row.Cells["Activo"]?.Value;
                if (actObj != null && actObj != DBNull.Value) act = Convert.ToBoolean(actObj);
                estadoprod.Checked = act;
            }

            if (ingredientechk != null)
            {
                bool ing = true;
                var ingObj = row.Cells["Ingrediente"]?.Value;
                if (ingObj != null && ingObj != DBNull.Value) ing = Convert.ToBoolean(ingObj);
                ingredientechk.Checked = ing;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            metodopanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            metodopanel.BringToFront();
            metodopanel.Visible = true;

            MP_CargarGrid();
            MP_Limpiar();
        }

        private void MP_CargarGrid()
        {
            string textoFiltro = metbuscar?.Text?.Trim() ?? "";
            bool soloActivos = (metfiltrochk != null && metfiltrochk.Checked);


            string sql = @"
                        SELECT IdMetodoPago, Nombre, Activo
                        FROM dbo.MetodoPago
                        WHERE (@f = '' OR Nombre LIKE '%' + @f + '%')"
                                    + (soloActivos ? " AND Activo = 1" : "") +
                                @"
                        ORDER BY Activo DESC, Nombre;";

            using (var cn = new SqlConnection(conexionString))
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@f", textoFiltro);

                var dt = new DataTable();
                da.Fill(dt);

                metododt.AutoGenerateColumns = true;
                metododt.DataSource = dt;

                if (metododt.Columns.Contains("IdMetodoPago"))
                {
                    var c = metododt.Columns["IdMetodoPago"];
                    c.HeaderText = "ID";
                    c.Width = 70;
                    c.ReadOnly = true;
                }
                if (metododt.Columns.Contains("Nombre"))
                    metododt.Columns["Nombre"].HeaderText = "Método";
                if (metododt.Columns.Contains("Activo"))
                    metododt.Columns["Activo"].HeaderText = "Activo";
            }
        }

        private void MP_Limpiar()
        {
            MetodoPagoId = "";
            idmetpago.Text = "";
            metodotxt.Text = "";
            if (estadometodo != null) estadometodo.Checked = true;
            metodotxt.Focus();
        }

        private bool MP_Validar()
        {
            if (string.IsNullOrWhiteSpace(metodotxt.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                metodotxt.Focus();
                return false;
            }

            if (MP_ExisteNombre(metodotxt.Text, MetodoPagoId))
            {
                MessageBox.Show("Ya existe un método con ese nombre.");
                metodotxt.Focus();
                return false;
            }
            return true;
        }

        private bool MP_ExisteNombre(string nombre, string idActual)
        {
            string sql = @"SELECT COUNT(1) FROM dbo.MetodoPago
                   WHERE Nombre = @n AND (@id = '' OR IdMetodoPago <> @idint);";

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@n", (nombre ?? "").Trim());
                int idint = 0; int.TryParse(idActual, out idint);
                cmd.Parameters.AddWithValue("@id", idActual ?? "");
                cmd.Parameters.AddWithValue("@idint", idint);

                cn.Open();
                int n = Convert.ToInt32(cmd.ExecuteScalar());
                return n > 0;
            }
        }

        private void MP_Guardar()
        {
            if (!MP_Validar()) return;

            using (var cn = new SqlConnection(conexionString))
            {
                cn.Open();

                if (string.IsNullOrEmpty(MetodoPagoId))
                {
                    string sql = "INSERT INTO dbo.MetodoPago (Nombre, Activo) VALUES (@Nombre, @Activo);";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", metodotxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", (estadometodo != null && estadometodo.Checked) ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Método registrado con éxito."
                            : "No se pudo guardar.");
                    }
                }
                else
                {
                    string sql = "UPDATE dbo.MetodoPago SET Nombre=@Nombre, Activo=@Activo WHERE IdMetodoPago=@Id;";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", int.Parse(MetodoPagoId));
                        cmd.Parameters.AddWithValue("@Nombre", metodotxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", (estadometodo != null && estadometodo.Checked) ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Método actualizado con éxito."
                            : "No se pudo actualizar.");
                    }
                }
            }

            MP_Limpiar();
            MP_CargarGrid();
        }

        private void MP_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var row = metododt.Rows[rowIndex];
            if (row == null) return;

            var idObj = row.Cells["IdMetodoPago"]?.Value;
            MetodoPagoId = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            idmetpago.Text = MetodoPagoId;

            metodotxt.Text = row.Cells["Nombre"]?.Value?.ToString() ?? "";

            if (metododt.Columns.Contains("Activo") && estadometodo != null)
            {
                var actObj = row.Cells["Activo"]?.Value;
                bool activo = false;
                if (actObj != null && actObj != DBNull.Value) activo = Convert.ToBoolean(actObj);
                estadometodo.Checked = activo;
            }
            else if (estadometodo != null)
            {
                estadometodo.Checked = true;
            }
        }

        private void Puesto_CargarGrid()
        {
            string textoFiltro = puestobuscar?.Text?.Trim() ?? "";
            bool soloActivos = (puestofiltrochk != null && puestofiltrochk.Checked);

            string sql = @"
                    SELECT  p.IdPuesto,
                            p.IdDepartamento,
                            d.Nombre AS Departamento,
                            p.Nombre   AS Puesto,
                            p.Activo
                            FROM dbo.Puesto p
                            JOIN dbo.Departamento d ON d.IdDepartamento = p.IdDepartamento
                            WHERE (@f = '' OR p.Nombre LIKE '%' + @f + '%' OR d.Nombre LIKE '%' + @f + '%')"
                                + (soloActivos ? " AND p.Activo = 1" : "") +
                            @"
                            ORDER BY p.Activo DESC, p.Nombre;";

            using (var cn = new SqlConnection(conexionString))
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@f", textoFiltro);

                var dt = new DataTable();
                da.Fill(dt);

                puestodt.AutoGenerateColumns = true;
                puestodt.DataSource = dt;

                if (puestodt.Columns.Contains("IdPuesto"))
                {
                    var c = puestodt.Columns["IdPuesto"];
                    c.HeaderText = "ID";
                    c.Width = 60;
                    c.ReadOnly = true;
                }
                if (puestodt.Columns.Contains("IdDepartamento"))
                    puestodt.Columns["IdDepartamento"].Visible = false; // escondemos el ID del depto

                if (puestodt.Columns.Contains("Departamento"))
                    puestodt.Columns["Departamento"].HeaderText = "Departamento";

                if (puestodt.Columns.Contains("Puesto"))
                    puestodt.Columns["Puesto"].HeaderText = "Puesto";

                if (puestodt.Columns.Contains("Activo"))
                    puestodt.Columns["Activo"].HeaderText = "Activo";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            puestopanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            puestopanel.BringToFront();
            puestopanel.Visible = true;

            Puesto_CargarGrid();
            Puesto_Limpiar();
        }

        private void Departamento_CargarGrid()
        {
            string filtro = depabuscar.Text.Trim();
            bool soloActivos = depafiltrochk.Checked;

            string consulta = "SELECT IdDepartamento, Nombre, Activo FROM Departamento WHERE 1=1";

            if (filtro != "")
            {
                consulta += " AND Nombre LIKE '%' + @filtro + '%'";
            }

            if (soloActivos)
            {
                consulta += " AND Activo = 1";
            }

            consulta += " ORDER BY Activo DESC, Nombre";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            using (SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion))
            {
                if (filtro != "")
                {
                    adaptador.SelectCommand.Parameters.AddWithValue("@filtro", filtro);
                }

                DataTable dt = new DataTable();
                adaptador.Fill(dt);

                depdt.AutoGenerateColumns = true;
                depdt.DataSource = dt;

                if (depdt.Columns.Contains("IdDepartamento"))
                {
                    DataGridViewColumn c = depdt.Columns["IdDepartamento"];
                    c.HeaderText = "ID";
                    c.Width = 60;
                    c.ReadOnly = true;
                }

                if (depdt.Columns.Contains("Nombre"))
                {
                    depdt.Columns["Nombre"].HeaderText = "Departamento";
                }

                if (depdt.Columns.Contains("Activo"))
                {
                    depdt.Columns["Activo"].HeaderText = "Activo";
                }
            }
        }

        private void Departamento_CargarDesdeGridFila()
        {
            if (depdt.CurrentRow == null) return;

            var fila = depdt.CurrentRow;

            var idObj = fila.Cells["IdDepartamento"].Value;
            DepaID = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            iddepa.Text = DepaID;

            depatxt.Text = fila.Cells["Nombre"].Value?.ToString() ?? "";

            bool activo = false;
            var actObj = fila.Cells["Activo"].Value;
            if (actObj != null && actObj != DBNull.Value)
                activo = Convert.ToBoolean(actObj);
            estadodepa.Checked = activo;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            depapanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            depapanel.BringToFront();
            depapanel.Visible = true;

            Departamento_CargarGrid();
            Departamento_Limpiar();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            textoinicial.Location = new Point(396, 150);
            textoinicial.BringToFront();
            textoinicial.Visible = true;

            depapanel.Location = new Point(744, 428);
            depapanel.Visible = false;

            puestopanel.Location = new Point(225, 428);
            puestopanel.Visible = false;

            metodopanel.Location = new Point(225, 12);
            metodopanel.Visible = false;

            prodpanel.Location = new Point(1263, 12);
            prodpanel.Visible = false;

            idenpanel.Location = new Point(744, 12);
            idenpanel.Visible = false;

            categpanel.Location = new Point(1263, 428);
            categpanel.Visible = false;

            motivopanel.Location = new Point(744, 428);
            motivopanel.Visible = false;

            unidadpanel.Location = new Point(744, 428);
            unidadpanel.Visible = false;
        }

        private void metfiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            MP_CargarGrid();
        }

        private void Puesto_CargarDepartamentosGrid()
        {
            string filtro = depapuestobuscar.Text.Trim();
            bool soloActivos = puestodepafiltrochk.Checked;

            string consulta = "SELECT IdDepartamento, Nombre, Activo FROM Departamento WHERE 1=1";

            if (filtro != "")
            {
                consulta += " AND Nombre LIKE '%' + @filtro + '%'";
            }

            if (soloActivos)
            {
                consulta += " AND Activo = 1";
            }

            consulta += " ORDER BY Activo DESC, Nombre";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            using (SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion))
            {
                if (filtro != "")
                {
                    adaptador.SelectCommand.Parameters.AddWithValue("@filtro", filtro);
                }

                DataTable dt = new DataTable();
                adaptador.Fill(dt);

                departdt.AutoGenerateColumns = true;
                departdt.DataSource = dt;

                if (departdt.Columns.Contains("IdDepartamento"))
                {
                    var c = departdt.Columns["IdDepartamento"];
                    c.HeaderText = "ID";
                    c.Width = 60;
                }

                if (departdt.Columns.Contains("Nombre"))
                    departdt.Columns["Nombre"].HeaderText = "Departamento";

                if (departdt.Columns.Contains("Activo"))
                    departdt.Columns["Activo"].HeaderText = "Activo";
            }
        }

        public int buscardepa = 1;

        private void button25_Click(object sender, EventArgs e)
        {
            if (buscardepa == 1)
            {
                btndepabuscar.Image = Proyecto_restaurante.Properties.Resources.cancelar1;
                toolTip1.SetToolTip(btndepabuscar, "Cancelar búsqueda");
                departdt.Location = new Point(13, 262);
                selectdepapuest.Location = new Point(363, 262);
                depapuestobuscar.Location = new Point(363, 332);
                puestodepafiltrochk.Location = new Point(363, 361);
                departdt.Visible = true;
                selectdepapuest.Visible = true;
                depapuestobuscar.Visible = true;
                puestodepafiltrochk.Visible = true;


                puestodt.Visible = false;
                selecpuest.Visible = false;
                puestobuscar.Visible = false;
                puestofiltrochk.Visible = false;

                Puesto_CargarDepartamentosGrid();

                buscardepa = 0;
            }
            else
            {
                btndepabuscar.Image = Proyecto_restaurante.Properties.Resources.busqueda1;
                toolTip1.SetToolTip(btndepabuscar, "Buscar departamento");
                puestodt.Location = new Point(13, 262);
                selecpuest.Location = new Point(363, 262);
                puestobuscar.Location = new Point(363, 332);
                puestofiltrochk.Location = new Point(363, 361);

                puestodt.Visible = true;
                selecpuest.Visible = true;
                puestobuscar.Visible = true;
                puestofiltrochk.Visible = true;

                departdt.Visible = false;
                selectdepapuest.Visible = false;
                depapuestobuscar.Visible = false;
                puestodepafiltrochk.Visible = false;

                buscardepa = 1;
            }
        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            motivopanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            motivopanel.BringToFront();
            motivopanel.Visible = true;

            MSI_CargarGrid();
            MSI_Limpiar();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            unidadpanel.Location = new Point(225, 12);
            textoinicial.Location = new Point(357, 875);
            unidadpanel.BringToFront();
            unidadpanel.Visible = true;

            UM_CargarGrid();
            UM_Limpiar();
        }

        private void guardardepartamento_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(depatxt.Text))
            {
                MessageBox.Show("Error: No deje campos vacíos.");
                return;
            }

            string nombre = depatxt.Text.Trim();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    // Validar nombre repetido
                    string sqlExiste;
                    if (string.IsNullOrEmpty(DepaID))
                    {
                        sqlExiste = "SELECT COUNT(*) FROM Departamento WHERE Nombre = @Nombre";
                    }
                    else
                    {
                        sqlExiste = "SELECT COUNT(*) FROM Departamento WHERE Nombre = @Nombre AND IdDepartamento <> @IdDepartamento";
                    }

                    using (SqlCommand cmdExiste = new SqlCommand(sqlExiste, conexion))
                    {
                        cmdExiste.Parameters.AddWithValue("@Nombre", nombre);
                        if (!string.IsNullOrEmpty(DepaID))
                        {
                            cmdExiste.Parameters.AddWithValue("@IdDepartamento", int.Parse(DepaID));
                        }

                        int cantidad = Convert.ToInt32(cmdExiste.ExecuteScalar());
                        if (cantidad > 0)
                        {
                            MessageBox.Show("Ya existe un departamento con ese nombre.");
                            return;
                        }
                    }

                    if (string.IsNullOrEmpty(DepaID))
                    {
                        string queryInsertar = "INSERT INTO Departamento (Nombre, Activo) VALUES (@Nombre, @Activo)";
                        using (SqlCommand insertarCommand = new SqlCommand(queryInsertar, conexion))
                        {
                            insertarCommand.Parameters.AddWithValue("@Nombre", nombre);
                            insertarCommand.Parameters.AddWithValue("@Activo", estadodepa.Checked ? 1 : 0);

                            int rowsAffected = insertarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Departamento registrado con éxito.");
                            }
                            else
                            {
                                MessageBox.Show("No se pudo guardar los datos.");
                            }
                        }
                    }
                    else
                    {
                        string queryActualizar = "UPDATE Departamento SET Nombre = @Nombre, Activo = @Activo WHERE IdDepartamento = @IdDepartamento";
                        using (SqlCommand actualizarCommand = new SqlCommand(queryActualizar, conexion))
                        {
                            actualizarCommand.Parameters.AddWithValue("@IdDepartamento", int.Parse(DepaID));
                            actualizarCommand.Parameters.AddWithValue("@Nombre", nombre);
                            actualizarCommand.Parameters.AddWithValue("@Activo", estadodepa.Checked ? 1 : 0);

                            int rowsAffected = actualizarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Departamento actualizado con éxito.");
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar los datos.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error: " + ex.Message);
                }
            }

            Departamento_Limpiar();
            Departamento_CargarGrid();
        }

        private void button15_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(puestotxt.Text) ||
                string.IsNullOrWhiteSpace(iddepapuestotxt.Text) ||
                string.IsNullOrWhiteSpace(depapuestotxt.Text))
            {
                MessageBox.Show("Error: No deje campos vacíos.");
                return;
            }

            if (!int.TryParse(iddepapuestotxt.Text, out int idDepa))
            {
                MessageBox.Show("El ID de departamento no es válido.");
                return;
            }

            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    if (string.IsNullOrEmpty(PuestoID))
                    {
                        string queryInsertar = @"
                                                INSERT INTO Puesto (IdDepartamento, Nombre, Activo)
                                                VALUES (@IdDepartamento, @Nombre, @Activo);";

                        using (SqlCommand insertarCommand = new SqlCommand(queryInsertar, conexion))
                        {
                            insertarCommand.Parameters.AddWithValue("@IdDepartamento", idDepa);
                            insertarCommand.Parameters.AddWithValue("@Nombre", puestotxt.Text.Trim());
                            insertarCommand.Parameters.AddWithValue("@Activo", estadopuesto.Checked ? 1 : 0);

                            int rowsAffected = insertarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                                MessageBox.Show("Puesto registrado con éxito.");
                            else
                                MessageBox.Show("No se pudo guardar los datos.");
                        }
                    }
                    else
                    {
                        string queryActualizar = @"
                                                    UPDATE Puesto
                                                    SET IdDepartamento = @IdDepartamento,
                                                    Nombre        = @Nombre,
                                                    Activo        = @Activo
                                                    WHERE IdPuesto     = @IdPuesto;";

                        using (SqlCommand actualizarCommand = new SqlCommand(queryActualizar, conexion))
                        {
                            actualizarCommand.Parameters.AddWithValue("@IdPuesto", int.Parse(PuestoID));
                            actualizarCommand.Parameters.AddWithValue("@IdDepartamento", idDepa);
                            actualizarCommand.Parameters.AddWithValue("@Nombre", puestotxt.Text.Trim());
                            actualizarCommand.Parameters.AddWithValue("@Activo", estadopuesto.Checked ? 1 : 0);

                            int rowsAffected = actualizarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                                MessageBox.Show("Puesto actualizado con éxito.");
                            else
                                MessageBox.Show("No se pudo actualizar los datos.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}");
                }
            }

            Puesto_Limpiar();
            Puesto_CargarGrid();
        }

        private void Puesto_Limpiar()
        {
            PuestoID = "";
            idpuesto.Text = "";
            puestotxt.Text = "";
            iddepapuestotxt.Text = "";
            depapuestotxt.Text = "";
            estadopuesto.Checked = true;
            puestotxt.Focus();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            Puesto_Limpiar();
        }

        private void Departamento_Limpiar()
        {
            DepaID = "";
            iddepa.Text = "";
            depatxt.Text = "";
            estadodepa.Checked = true;
            depatxt.Focus();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Departamento_Limpiar();
        }

        private void Puesto_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var row = puestodt.Rows[rowIndex];
            if (row == null) return;

            var idObj = row.Cells["IdPuesto"]?.Value;
            PuestoID = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            idpuesto.Text = PuestoID;


            puestotxt.Text = row.Cells["Puesto"]?.Value?.ToString() ?? "";


            if (row.Cells["IdDepartamento"]?.Value != null)
                iddepapuestotxt.Text = row.Cells["IdDepartamento"].Value.ToString();
            else
                iddepapuestotxt.Text = "";

            depapuestotxt.Text = row.Cells["Departamento"]?.Value?.ToString() ?? "";


            bool activo = false;
            var actObj = row.Cells["Activo"]?.Value;
            if (actObj != null && actObj != DBNull.Value) activo = Convert.ToBoolean(actObj);
            estadopuesto.Checked = activo;
        }

        private void selecpuest_Click(object sender, EventArgs e)
        {
            if (puestodt.CurrentRow != null)
                Puesto_CargarDesdeGridRow(puestodt.CurrentRow.Index);
        }

        private void puestodt_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Puesto_CargarDesdeGridRow(e.RowIndex);
        }

        private void puestodt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Puesto_CargarDesdeGridRow(e.RowIndex);
        }

        private void departdt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            iddepapuestotxt.Text = departdt.SelectedCells[0].Value.ToString();
            depapuestotxt.Text = departdt.SelectedCells[1].Value.ToString();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(identtxt.Text))
            {
                MessageBox.Show("Error: No deje campos vacíos.");
                return;
            }

            string nombre = identtxt.Text.Trim();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    // Validar nombre repetido
                    string sqlExiste;
                    if (string.IsNullOrEmpty(TipoDocID))
                    {
                        sqlExiste = "SELECT COUNT(*) FROM TipoDocumento WHERE Nombre = @Nombre";
                    }
                    else
                    {
                        sqlExiste = "SELECT COUNT(*) FROM TipoDocumento WHERE Nombre = @Nombre AND IdTipoDocumento <> @IdTipoDoc";
                    }

                    using (SqlCommand cmdExiste = new SqlCommand(sqlExiste, conexion))
                    {
                        cmdExiste.Parameters.AddWithValue("@Nombre", nombre);
                        if (!string.IsNullOrEmpty(TipoDocID))
                        {
                            cmdExiste.Parameters.AddWithValue("@IdTipoDoc", int.Parse(TipoDocID));
                        }

                        int cantidad = Convert.ToInt32(cmdExiste.ExecuteScalar());
                        if (cantidad > 0)
                        {
                            MessageBox.Show("Ya existe un tipo de documento con ese nombre.");
                            return;
                        }
                    }


                    if (string.IsNullOrEmpty(TipoDocID))
                    {
                        string queryInsertar = "INSERT INTO TipoDocumento (Nombre, Activo) VALUES (@Nombre, @Activo)";
                        using (SqlCommand insertarCommand = new SqlCommand(queryInsertar, conexion))
                        {
                            insertarCommand.Parameters.AddWithValue("@Nombre", nombre);
                            insertarCommand.Parameters.AddWithValue("@Activo", estadoiden.Checked ? 1 : 0);

                            int rowsAffected = insertarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Documento registrado con éxito.");
                            }
                            else
                            {
                                MessageBox.Show("No se pudo guardar los datos.");
                            }
                        }
                    }
                    else
                    {
                        string queryActualizar = "UPDATE TipoDocumento SET Nombre = @Nombre, Activo = @Activo WHERE IdTipoDocumento = @IdTipoDoc";
                        using (SqlCommand actualizarCommand = new SqlCommand(queryActualizar, conexion))
                        {
                            actualizarCommand.Parameters.AddWithValue("@IdTipoDoc", int.Parse(TipoDocID));
                            actualizarCommand.Parameters.AddWithValue("@Nombre", nombre);
                            actualizarCommand.Parameters.AddWithValue("@Activo", estadoiden.Checked ? 1 : 0);

                            int rowsAffected = actualizarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Tipo de documento actualizado con éxito.");
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar los datos.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error: " + ex.Message);
                }
            }

            TipoDoc_Limpiar();
            TipoDoc_CargarGrid();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MP_Guardar();
        }

        private void UM_CargarGrid()
        {

            string filtro = unidadbusqueda?.Text?.Trim() ?? "";
            bool? soloActivos = (unidadfiltrochk != null && unidadfiltrochk.Checked) ? true : (bool?)null;

            string sql = @"
                            SELECT IdUnidadMedida, Nombre, Valor, Activo
                            FROM UnidadMedida
                            WHERE (@f = '' OR Nombre LIKE '%' + @f + '%')
                            AND (@act IS NULL OR Activo = @act)
                            -- Activos primero cuando NO se filtra por activos
                            ORDER BY 
                            CASE WHEN @act IS NULL THEN CASE WHEN Activo = 1 THEN 0 ELSE 1 END ELSE 0 END, Nombre;";

            using (SqlConnection con = new SqlConnection(conexionString))
            using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
            {
                da.SelectCommand.Parameters.AddWithValue("@f", filtro);
                da.SelectCommand.Parameters.AddWithValue(
                    "@act",
                    (object?)(soloActivos.HasValue ? (soloActivos.Value ? 1 : 0) : null) ?? DBNull.Value
                );

                DataTable dt = new DataTable();
                da.Fill(dt);


                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataSource = dt;


                if (dataGridView2.Columns.Contains("IdUnidadMedida"))
                {
                    var c = dataGridView2.Columns["IdUnidadMedida"];
                    c.HeaderText = "ID";
                    c.Width = 80;
                    c.ReadOnly = true;
                }
                if (dataGridView2.Columns.Contains("Nombre"))
                {
                    var c = dataGridView2.Columns["Nombre"];
                    c.HeaderText = "Unidad";
                }
                if (dataGridView2.Columns.Contains("Valor"))
                {
                    var c = dataGridView2.Columns["Valor"];
                    c.HeaderText = "Valor";
                    c.DefaultCellStyle.Format = "0.####"; // hasta 4 decimales, sin ceros de mas
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    c.Width = 110;
                }

            }
        }

        private void UM_Limpiar()
        {
            UnidadID = "";
            idunidad.Text = "";
            nombreunidadtxt.Clear();
            valorunidadtxt.Clear();
            unidadestadochk.Checked = true;
            nombreunidadtxt.Focus();
        }

        private bool UM_Validar(out decimal valor)
        {
            valor = 0m;

            if (string.IsNullOrWhiteSpace(nombreunidadtxt.Text))
            { MessageBox.Show("El nombre es obligatorio."); nombreunidadtxt.Focus(); return false; }

            string txt = (valorunidadtxt.Text ?? "").Replace(',', '.');
            if (!decimal.TryParse(txt, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out valor) || valor <= 0)
            { MessageBox.Show("El valor debe ser > 0 (ej: 1 o 0.001)"); valorunidadtxt.Focus(); return false; }

            if (UM_ExisteNombre(nombreunidadtxt.Text, UnidadID))
            { MessageBox.Show("Ya existe una unidad con ese nombre."); nombreunidadtxt.Focus(); return false; }

            return true;
        }

        private bool UM_ExisteNombre(string nombre, string idActual)
        {
            string sql = @"SELECT COUNT(1) FROM UnidadMedida 
                   WHERE Nombre = @n AND (@id = '' OR IdUnidadMedida <> @idint);";
            using (SqlConnection con = new SqlConnection(conexionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@n", (nombre ?? "").Trim());
                int idint = 0; int.TryParse(idActual, out idint);
                cmd.Parameters.AddWithValue("@id", idActual ?? "");
                cmd.Parameters.AddWithValue("@idint", idint);
                con.Open();
                int n = Convert.ToInt32(cmd.ExecuteScalar());
                return n > 0;
            }
        }

        private void UM_Guardar()
        {
            if (!UM_Validar(out decimal valor)) return;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    if (string.IsNullOrEmpty(UnidadID))
                    {
                        string q = @"INSERT INTO UnidadMedida (Nombre, Valor, Activo)
                             VALUES (@Nombre, @Valor, @Activo)";
                        using (SqlCommand cmd = new SqlCommand(q, conexion))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", nombreunidadtxt.Text.Trim());

                            // Valor como decimal(10,4)
                            var pValor = cmd.Parameters.Add("@Valor", SqlDbType.Decimal);
                            pValor.Precision = 10; pValor.Scale = 4;
                            pValor.Value = Math.Round(valor, 4);

                            cmd.Parameters.AddWithValue("@Activo", unidadestadochk.Checked ? 1 : 0);

                            MessageBox.Show(cmd.ExecuteNonQuery() > 0
                                ? "Unidad registrada con éxito." : "No se pudo guardar.");
                        }
                    }
                    else
                    {
                        string q = @"UPDATE UnidadMedida
                             SET Nombre=@Nombre, Valor=@Valor, Activo=@Activo
                             WHERE IdUnidadMedida=@Id";
                        using (SqlCommand cmd = new SqlCommand(q, conexion))
                        {
                            cmd.Parameters.AddWithValue("@Id", int.Parse(UnidadID));
                            cmd.Parameters.AddWithValue("@Nombre", nombreunidadtxt.Text.Trim());

                            var pValor = cmd.Parameters.Add("@Valor", SqlDbType.Decimal);
                            pValor.Precision = 10; pValor.Scale = 4;
                            pValor.Value = Math.Round(valor, 4);

                            cmd.Parameters.AddWithValue("@Activo", unidadestadochk.Checked ? 1 : 0);

                            MessageBox.Show(cmd.ExecuteNonQuery() > 0
                                ? "Unidad actualizada con éxito." : "No se pudo actualizar.");
                        }
                    }

                    UM_Limpiar();
                    UM_CargarGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void UM_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var row = dataGridView2.Rows[rowIndex];
            if (row == null) return;

            var idObj = row.Cells["IdUnidadMedida"]?.Value;
            UnidadID = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            idunidad.Text = UnidadID;

            nombreunidadtxt.Text = row.Cells["Nombre"]?.Value?.ToString() ?? "";

            var valObj = row.Cells["Valor"]?.Value;
            decimal valDec = 0m;
            if (valObj != null && valObj != DBNull.Value) decimal.TryParse(valObj.ToString(), out valDec);
            valorunidadtxt.Text = valDec.ToString("0.####");

            var actObj = row.Cells["Activo"]?.Value;
            bool activo = false;
            if (actObj != null && actObj != DBNull.Value) activo = Convert.ToBoolean(actObj);
            unidadestadochk.Checked = activo;
        }

        private void MSI_CargarGrid()
        {
            string textoFiltro = movitobusqueda?.Text?.Trim() ?? "";
            bool soloActivos = (motivofiltrochk != null && motivofiltrochk.Checked);

            string sql = @"
                            SELECT IdMotivo, Nombre, Activo
                            FROM dbo.MotivoSalidaInventario
                            WHERE (@f = '' OR Nombre LIKE '%' + @f + '%')
                            AND (@soloAct = 0 OR Activo = 1)
                            ORDER BY 
                            CASE WHEN @soloAct = 0 THEN CASE WHEN Activo = 1 THEN 0 ELSE 1 END ELSE 0 END, Nombre;";

            using (var cn = new SqlConnection(conexionString))
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@f", textoFiltro);
                da.SelectCommand.Parameters.AddWithValue("@soloAct", soloActivos ? 1 : 0);

                var dt = new DataTable();
                da.Fill(dt);

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dt;

                if (dataGridView1.Columns.Contains("IdMotivo"))
                {
                    var c = dataGridView1.Columns["IdMotivo"];
                    c.HeaderText = "ID";
                    c.Width = 70;
                    c.ReadOnly = true;
                }
                if (dataGridView1.Columns.Contains("Nombre"))
                    dataGridView1.Columns["Nombre"].HeaderText = "Motivo";
                if (dataGridView1.Columns.Contains("Activo"))
                    dataGridView1.Columns["Activo"].HeaderText = "Activo";
            }
        }

        private void MSI_Limpiar()
        {
            MotivoId = "";
            if (textBox2 != null) textBox2.Text = "";
            motivotxt.Text = "";
            if (estadomotivo != null) estadomotivo.Checked = true;
            motivotxt.Focus();
        }

        private bool MSI_Validar()
        {
            if (string.IsNullOrWhiteSpace(motivotxt.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                motivotxt.Focus();
                return false;
            }

            if (MSI_ExisteNombre(motivotxt.Text, MotivoId))
            {
                MessageBox.Show("Ya existe un motivo con ese nombre.");
                motivotxt.Focus();
                return false;
            }
            return true;
        }

        private bool MSI_ExisteNombre(string nombre, string idActual)
        {
            string sql = @"SELECT COUNT(1) FROM dbo.MotivoSalidaInventario
                   WHERE Nombre = @n AND (@id = '' OR IdMotivo <> @idint);";
            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@n", (nombre ?? "").Trim());
                int idint = 0; int.TryParse(idActual, out idint);
                cmd.Parameters.AddWithValue("@id", idActual ?? "");
                cmd.Parameters.AddWithValue("@idint", idint);

                cn.Open();
                int n = Convert.ToInt32(cmd.ExecuteScalar());
                return n > 0;
            }
        }

        private void MSI_Guardar()
        {
            if (!MSI_Validar()) return;

            using (var cn = new SqlConnection(conexionString))
            {
                cn.Open();

                if (string.IsNullOrEmpty(MotivoId))
                {
                    string sql = "INSERT INTO dbo.MotivoSalidaInventario (Nombre, Activo) VALUES (@Nombre, @Activo);";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", motivotxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", (estadomotivo != null && estadomotivo.Checked) ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Motivo registrado con éxito."
                            : "No se pudo guardar.");
                    }
                }
                else
                {
                    string sql = "UPDATE dbo.MotivoSalidaInventario SET Nombre=@Nombre, Activo=@Activo WHERE IdMotivo=@Id;";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", int.Parse(MotivoId));
                        cmd.Parameters.AddWithValue("@Nombre", motivotxt.Text.Trim());
                        cmd.Parameters.AddWithValue("@Activo", (estadomotivo != null && estadomotivo.Checked) ? 1 : 0);

                        MessageBox.Show(cmd.ExecuteNonQuery() > 0
                            ? "Motivo actualizado con éxito."
                            : "No se pudo actualizar.");
                    }
                }
            }

            MSI_Limpiar();
            MSI_CargarGrid();
        }

        private void MSI_CargarDesdeGridRow(int rowIndex)
        {
            if (rowIndex < 0) return;
            var row = dataGridView1.Rows[rowIndex];
            if (row == null) return;

            var idObj = row.Cells["IdMotivo"]?.Value;
            MotivoId = (idObj == null || idObj == DBNull.Value) ? "" : idObj.ToString();
            if (textBox2 != null) textBox2.Text = MotivoId;

            motivotxt.Text = row.Cells["Nombre"]?.Value?.ToString() ?? "";

            if (estadomotivo != null && dataGridView1.Columns.Contains("Activo"))
            {
                var actObj = row.Cells["Activo"]?.Value;
                bool activo = false;
                if (actObj != null && actObj != DBNull.Value) activo = Convert.ToBoolean(actObj);
                estadomotivo.Checked = activo;
            }
            else if (estadomotivo != null)
            {
                estadomotivo.Checked = true;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Puesto_Limpiar();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TipoDoc_Limpiar();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MP_Limpiar();
        }

        private void selecdepa_Click(object sender, EventArgs e)
        {
            Departamento_CargarDesdeGridFila();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            UM_Limpiar();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            UM_Guardar();
        }

        private void unidadbusqueda_TextChanged(object sender, EventArgs e)
        {
            UM_CargarGrid();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            UM_CargarDesdeGridRow(e.RowIndex);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            UM_Limpiar();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null)
                UM_CargarDesdeGridRow(dataGridView2.CurrentRow.Index);
        }

        private void unidadfiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            UM_CargarGrid();
        }

        private void guardarmotivo_Click(object sender, EventArgs e)
        {
            MSI_Guardar();
        }

        private void motivofiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            MSI_CargarGrid();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            MSI_Limpiar();
        }

        private void selecmotivo_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
                MSI_CargarDesdeGridRow(dataGridView1.CurrentRow.Index);
        }

        private void movitobusqueda_TextChanged(object sender, EventArgs e)
        {
            MSI_CargarGrid();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MSI_CargarDesdeGridRow(e.RowIndex);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            MSI_Limpiar();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MP_Limpiar();
        }

        private void selecmetodo_Click(object sender, EventArgs e)
        {
            if (metododt.CurrentRow != null)
                MP_CargarDesdeGridRow(metododt.CurrentRow.Index);
        }

        private void metbuscar_TextChanged(object sender, EventArgs e)
        {
            MP_CargarGrid();
        }

        private void metododt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MP_CargarDesdeGridRow(e.RowIndex);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            CP_Limpiar();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            CP_Guardar();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            CP_Limpiar();
        }

        private void seleccateg_Click(object sender, EventArgs e)
        {
            if (categdt.CurrentRow != null)
                CP_CargarDesdeGridRow(categdt.CurrentRow.Index);
        }

        private void categbuscar_TextChanged(object sender, EventArgs e)
        {
            CP_CargarGrid();
        }

        private void categfiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            CP_CargarGrid();
        }

        private void categdt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CP_CargarDesdeGridRow(e.RowIndex);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            PT_Limpiar();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            PT_Guardar();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            PT_Limpiar();
        }

        private void selecprod_Click(object sender, EventArgs e)
        {
            if (prodtidt.CurrentRow != null)
                PT_CargarDesdeGridRow(prodtidt.CurrentRow.Index);
        }

        private void prodbuscar_TextChanged(object sender, EventArgs e)
        {
            PT_CargarGrid();
        }

        private void prodfiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            PT_CargarGrid();
        }

        private void prodtidt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PT_CargarDesdeGridRow(e.RowIndex);
        }

        private void departdt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            departdt.CurrentCell = departdt.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (departdt.SelectedCells.Count >= 2)
            {
                iddepapuestotxt.Text = departdt.SelectedCells[0].Value.ToString();
                depapuestotxt.Text = departdt.SelectedCells[1].Value.ToString();
            }
        }

        private void depdt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                depdt.CurrentCell = depdt.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Departamento_CargarDesdeGridFila();
            }
        }

        private void puestodepafiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            Puesto_CargarDepartamentosGrid();
        }

        private void puestobuscar_TextChanged(object sender, EventArgs e)
        {
            Puesto_CargarGrid();
        }

        private void puestofiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            Puesto_CargarGrid();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Departamento_Limpiar();
        }

        private void depabuscar_TextChanged(object sender, EventArgs e)
        {
            Departamento_CargarGrid();
        }

        private void depafiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            Departamento_CargarGrid();
        }

        private void depapuestobuscar_TextChanged(object sender, EventArgs e)
        {
            Puesto_CargarDepartamentosGrid();
        }

        private void selectdepapuest_Click(object sender, EventArgs e)
        {
            if (departdt.SelectedCells.Count >= 2)
            {
                iddepapuestotxt.Text = departdt.SelectedCells[0].Value.ToString();
                depapuestotxt.Text = departdt.SelectedCells[1].Value.ToString();
            }
        }

        private void identfiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            TipoDoc_CargarGrid();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TipoDoc_Limpiar();
        }

        private void identdt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                identdt.CurrentCell = identdt.Rows[e.RowIndex].Cells[e.ColumnIndex];
                TipoDoc_CargarDesdeGridRow(e.RowIndex);
            }
        }

        private void seleciden_Click(object sender, EventArgs e)
        {
            if (identdt.CurrentRow != null)
            {
                TipoDoc_CargarDesdeGridRow(identdt.CurrentRow.Index);
            }
        }

        private void identbuscar_TextChanged(object sender, EventArgs e)
        {
            TipoDoc_CargarGrid();
        }

        private void ingredientechk_CheckedChanged(object sender, EventArgs e)
        {
            if (ingredientechk.Checked)
            {
                bebidachk.Checked = false;
                adicionchk.Checked = false;
            }
        }

        private void bebidachk_CheckedChanged(object sender, EventArgs e)
        {
            if (bebidachk.Checked)
            {
                ingredientechk.Checked = false;
                adicionchk.Checked = false;
            }
        }

        private void adicionchk_CheckedChanged(object sender, EventArgs e)
        {
            if (adicionchk.Checked)
            {
                ingredientechk.Checked = false;
                bebidachk.Checked = false;
            }
        }
    }
}