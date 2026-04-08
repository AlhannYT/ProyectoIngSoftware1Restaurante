using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Proyecto_restaurante
{
    public partial class inicio : Form
    {
        public inicio()
        {
            InitializeComponent();
        }

        private static string rutaUsuario = @"C:\SistemaArchivos\Usuarios\Usuarios.txt";
        public string rutaArchivo = @"C:\SistemaArchivos\Conexion\ConexionesSQL.txt";

        public int idUsuario = 0;
        bool esAdmin = false;
        public string UsuarioAdmin = "A";
        public string PassAdmin = "A";

        private void iniciobtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtusuario.Text) || string.IsNullOrEmpty(txtpass.Text))
            {
                MessageBox.Show("Error: Campos vacíos.");
                return;
            }

            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    string queryUsuario = @"
                    SELECT IdUsuario 
                    FROM Usuario 
                    WHERE Login = @usuario AND Contrasena = @pass AND Activo = 1";

                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conexion))
                    {
                        cmd.Parameters.AddWithValue("@usuario", txtusuario.Text);
                        cmd.Parameters.AddWithValue("@pass", txtpass.Text);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado == null)
                        {
                            if (txtusuario.Text == UsuarioAdmin && txtpass.Text == PassAdmin)
                            {
                                idUsuario = 1;
                                resultado = 1;
                                esAdmin = true;
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contraseña incorrectos / Inactivo.");
                                return;
                            }
                        }

                        idUsuario = Convert.ToInt32(resultado);
                    }

                    string queryPermiso = @"
                    SELECT Admin 
                    FROM PermisosUsuario 
                    WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand cmdPermiso = new SqlCommand(queryPermiso, conexion))
                    {
                        cmdPermiso.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        object adminResult = cmdPermiso.ExecuteScalar();

                        if (adminResult != null)
                            esAdmin = Convert.ToBoolean(adminResult);
                    }

                    string nombrePC = Environment.MachineName;

                    string queryExiste = "SELECT COUNT(*) FROM Configuracion WHERE NombrePC = @NombrePC";
                    using (SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion))
                    {
                        cmdExiste.Parameters.AddWithValue("@NombrePC", nombrePC);
                        int existe = (int)cmdExiste.ExecuteScalar();

                        if (existe == 0)
                        {
                            string insertarQuery = "INSERT INTO Configuracion (NombrePC) VALUES (@NombrePC)";
                            using (SqlCommand cmdInsertar = new SqlCommand(insertarQuery, conexion))
                            {
                                cmdInsertar.Parameters.AddWithValue("@NombrePC", nombrePC);
                                cmdInsertar.ExecuteNonQuery();
                            }
                        }
                    }

                    menu menu = new menu();

                    if (esAdmin)
                    {
                        menu.administrador = 1;
                        menu.panel5.BackColor = Color.Gold;
                    }
                    else
                    {
                        menu.administrador = 0;
                        menu.panel5.BackColor = Color.Green;
                    }

                    menu.usuariolabel.Text = $"USUARIO ACTUAL:\n{txtusuario.Text}";
                    menu.usuarioActual = txtusuario.Text;
                    menu.IdUsuarioActual = idUsuario;

                    Directory.CreateDirectory(Path.GetDirectoryName(rutaUsuario));

                    if (recordarchk.Checked)
                    {
                        try
                        {
                            File.WriteAllText(rutaUsuario, txtusuario.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("No se pudo guardar el usuario recordado: " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (File.Exists(rutaUsuario))
                                File.Delete(rutaUsuario);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("No se pudo eliminar el archivo del usuario recordado: " + ex.Message);
                        }
                    }

                    menu.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void passView_CheckedChanged(object sender, EventArgs e)
        {
            if (passView.Checked == true)
            {
                passView.Image = Proyecto_restaurante.Properties.Resources.ojos_cruzados;
                txtpass.UseSystemPasswordChar = false;
            }
            else if (passView.Checked == false)
            {
                passView.Image = Proyecto_restaurante.Properties.Resources.ojo;
                txtpass.UseSystemPasswordChar = true;
            }
        }

        private void txtusuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtpass.Focus();
                e.Handled = true;
            }
        }

        private void txtpass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                iniciobtn_Click(sender, e);
                e.Handled = true;
            }
        }

        private void txtusuario_TextChanged(object sender, EventArgs e)
        {
            int posicion = txtusuario.SelectionStart;

            txtusuario.Text = txtusuario.Text.ToUpper();

            txtusuario.SelectionStart = posicion;

            recordarchk.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sqlbtn_Click(object sender, EventArgs e)
        {
            autorizar.Location = new Point(78, 71);
            autorizar.BringToFront();
            autorizar.Visible = true;

            panelSesion.Enabled = false;
        }

        private void salirsqlbtn_Click(object sender, EventArgs e)
        {
            conexionpanel.Location = new Point(605, 45);
            conexionpanel.Visible = false;
            
        }

        private void guardarbtn_Click(object sender, EventArgs e)
        {
            string servidor = servidortxt.Text.Trim();
            string usuario = usuarioservidortxt.Text.Trim();
            string contraseña = contservidortxt.Text.Trim();
            string porDefecto = defectochk.Checked ? "1" : "0";

            if (string.IsNullOrEmpty(servidor) || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> lineas = new List<string>();
            bool reemplazoHecho = false;
            bool actualizada = false;

            if (File.Exists(rutaArchivo))
            {
                var lineasOriginales = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineasOriginales)
                {
                    var partes = linea.Split('|');
                    if (partes.Length == 4)
                    {
                        bool mismaConexion = partes[0] == servidor && partes[1] == usuario;

                        if (mismaConexion)
                        {
                            lineas.Add($"{servidor}|{usuario}|{contraseña}|{porDefecto}");
                            actualizada = true;
                        }
                        else
                        {
                            if (partes[3] == "1" && porDefecto == "1" && !reemplazoHecho)
                            {
                                partes[3] = "0";
                                reemplazoHecho = true;
                            }

                            lineas.Add(string.Join("|", partes));
                        }
                    }
                }
            }

            if (!actualizada)
                lineas.Add($"{servidor}|{usuario}|{contraseña}|{porDefecto}");

            File.WriteAllLines(rutaArchivo, lineas);

            MessageBox.Show(actualizada ? "Conexión actualizada correctamente." : "Conexión guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            iniciobtn.Enabled = true;
            button6_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conexiones.Location = new Point(0, 33);
            conexiones.BringToFront();
            conexiones.Visible = true;

            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show("El archivo de conexiones no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tabla = new DataTable();
            tabla.Columns.Add("Servidor");
            tabla.Columns.Add("Usuario");
            tabla.Columns.Add("Contraseña");
            tabla.Columns.Add("PorDefecto");

            var lineas = File.ReadAllLines(rutaArchivo);

            foreach (var linea in lineas)
            {
                var partes = linea.Split('|');
                if (partes.Length == 4)
                {
                    tabla.Rows.Add(partes[0], partes[1], partes[2], partes[3] == "1" ? "Sí" : "No");
                }
            }

            txtsql.DataSource = tabla;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            conexiones.Location = new Point(605, 45);
            conexiones.Visible = false; conexiones.Location = new Point(605, 435);
            conexiones.Visible = false;
        }

        private void txtsql_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = txtsql.Rows[e.RowIndex];

                servidortxt.Text = fila.Cells["Servidor"].Value?.ToString();
                usuarioservidortxt.Text = fila.Cells["Usuario"].Value?.ToString();
                contservidortxt.Text = fila.Cells["Contraseña"].Value?.ToString();

                string porDefecto = fila.Cells["PorDefecto"].Value?.ToString();
                defectochk.Checked = porDefecto == "Sí";
                button5_Click(sender, e);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtsql_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, 0));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            servidortxt.Text = "";
            usuarioservidortxt.Text = "";
            contservidortxt.Text = "";
            defectochk.Checked = false;
        }

        private void borrarconex_Click(object sender, EventArgs e)
        {
            if (txtsql.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una conexión para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow fila = txtsql.SelectedRows[0];
            string servidor = fila.Cells["Servidor"].Value?.ToString();
            string usuario = fila.Cells["Usuario"].Value?.ToString();
            string contraseña = fila.Cells["Contraseña"].Value?.ToString();
            string porDefecto = fila.Cells["PorDefecto"].Value?.ToString() == "Sí" ? "1" : "0";

            string lineaEliminar = $"{servidor}|{usuario}|{contraseña}|{porDefecto}";

            var lineas = File.ReadAllLines(rutaArchivo).ToList();
            bool eliminada = lineas.Remove(lineaEliminar);

            if (eliminada)
            {
                File.WriteAllLines(rutaArchivo, lineas);
                MessageBox.Show("Conexión eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button3_Click(sender, e);
            }
            else
            {
                MessageBox.Show("No se pudo eliminar la conexión. Verifica los datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inicio_Load(object sender, EventArgs e)
        {
            string rutaBase = @"C:\SistemaArchivos";

            string rutaReportes = @"C:\SistemaArchivos\Reportes";

            string rutaConexion = Path.Combine(rutaBase, "Conexion", "ConexionesSQL.txt");

            string[] carpetas = {
                "Clientes",
                "Conexion",
                "Configuracion",
                "Empleados",
                "Facturas",
                "Productos",
                "Proveedor",
                "DGIITXT",
                "Usuarios"
            };

            string[] carpetasReportes = {
                "Ventas",
                "PlatosVendidos",
                "Stock",
                "Compras",
                "Clientes",
                "Proveedores",
                "Empleados"
            };

            try
            {
                if (!Directory.Exists(rutaBase))
                    Directory.CreateDirectory(rutaBase);

                if (!Directory.Exists(rutaReportes))
                    Directory.CreateDirectory(rutaReportes);

                foreach (string carpeta in carpetas)
                {
                    string rutaCompleta = Path.Combine(rutaBase, carpeta);
                    if (!Directory.Exists(rutaCompleta))
                        Directory.CreateDirectory(rutaCompleta);
                }

                foreach (string carpetaRep in carpetasReportes)
                {
                    string rutaRepCompleta = Path.Combine(rutaReportes, carpetaRep);
                    if (!Directory.Exists(rutaRepCompleta))
                        Directory.CreateDirectory(rutaRepCompleta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear las carpetas requeridas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!File.Exists(rutaConexion))
            {
                iniciobtn.Enabled = false;

                alerta.Visible = true;

                System.Windows.Forms.Timer timerParpadeo = new System.Windows.Forms.Timer();
                timerParpadeo.Interval = 500;
                int contador = 0;

                timerParpadeo.Tick += (s, args) =>
                {
                    alerta.Visible = !alerta.Visible;
                    contador++;

                    if (contador >= 10)
                    {
                        timerParpadeo.Stop();
                        alerta.Visible = true;
                    }
                };

                timerParpadeo.Start();
            }
            else
            {
                alerta.Visible = false;
                iniciobtn.Enabled = true;
            }

            try
            {
                if (File.Exists(rutaUsuario))
                {
                    string usuarioGuardado = File.ReadAllText(rutaUsuario).Trim();

                    if (!string.IsNullOrEmpty(usuarioGuardado))
                    {
                        txtusuario.Text = usuarioGuardado;
                        recordarchk.Checked = true;
                        recordarchk.BackColor = Color.LightGreen;
                        txtpass.Focus();
                    }
                }
                else
                {
                    txtusuario.Text = string.Empty;
                    recordarchk.BackColor = SystemColors.Window;
                    recordarchk.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo leer el usuario recordado: " + ex.Message);
            }
        }

        private void inicio_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtusuario.Text))
                txtpass.Focus();
        }

        private void recordarchk_CheckedChanged(object sender, EventArgs e)
        {
            if (recordarchk.Checked)
            {
                recordarchk.BackColor = Color.LightGreen;
            }
            else
            {
                recordarchk.BackColor = SystemColors.Window;
            }
        }

        private async void CrearDBbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(servidortxt.Text) || string.IsNullOrEmpty(usuarioservidortxt.Text) || string.IsNullOrEmpty(contservidortxt.Text))
            {
                MessageBox.Show("Error: Campos vacíos.");
                return;
            }

            progressBar1.Visible = true;
            progressBar1.Value = 10;

            string servidor = servidortxt.Text;
            string usuario = usuarioservidortxt.Text;
            string contraseña = contservidortxt.Text;

            string conexion = $"Server={servidor};Database=master;User Id={usuario};Password={contraseña};TrustServerCertificate=True;";

            string nombreDB = "GloriaRestaurant";

            try
            {
                using (SqlConnection conn = new SqlConnection(conexion))
                {
                    await conn.OpenAsync();
                }

                progressBar1.Value = 30;
                MessageBox.Show("Conexión exitosa");

                bool existe = await Task.Run(() => ExisteBaseDatos(conexion, nombreDB));

                if (existe)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show("La base de datos ya existe. No se puede crear nuevamente.");
                    return;
                }

                progressBar1.Value = 50;

                await Task.Run(() => EjecutarScript(conexion, ScriptDB.CrearBaseDatos));

                progressBar1.Value = 90;

                MessageBox.Show("Base de datos creada correctamente");

                guardarbtn.PerformClick();

                progressBar1.Value = 100;
            }
            catch (Exception ex)
            {
                progressBar1.Value = 0;
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                await Task.Delay(500);
                progressBar1.Visible = false;
            }
        }

        private void EjecutarScript(string conexion, string script)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string[] comandos = script.Split(
                    new[] { "\r\nGO", "\nGO", "GO" },
                    StringSplitOptions.RemoveEmptyEntries
                );

                foreach (string comando in comandos)
                {
                    if (!string.IsNullOrWhiteSpace(comando))
                    {
                        using (SqlCommand cmd = new SqlCommand(comando, conn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private bool ExisteBaseDatos(string conexion, string nombreDB)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM sys.databases WHERE name = @nombre";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombreDB);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void autorizarBTN_Click(object sender, EventArgs e)
        {
            if (usuAdmin.Text == UsuarioAdmin && contAdmin.Text == PassAdmin)
            {
                esAdmin = true;

                //quita
                autorizar.Location = new Point(51, 436);
                autorizar.BringToFront();
                autorizar.Visible = true;

                //pone el otro
                conexionpanel.Location = new Point(0, 33);
                conexionpanel.BringToFront();
                conexionpanel.Visible = true;
                panelSesion.Enabled = true;
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos / Inactivo.");
                return;
            }
        }

        private void salirAutorBTN_Click(object sender, EventArgs e)
        {
            autorizar.Location = new Point(51, 436);
            autorizar.BringToFront();
            autorizar.Visible = true;

            panelSesion.Enabled = true;
        }
    }
}