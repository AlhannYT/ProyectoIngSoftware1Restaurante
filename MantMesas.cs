using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Proyecto_restaurante.menu;

namespace Proyecto_restaurante
{
    public partial class MantMesas : Form
    {
        public MantMesas()
        {
            InitializeComponent();

            ocupadochk.Cursor = Cursors.Hand;
            estadomesa.Cursor = Cursors.Hand;
            estadochk.Cursor = Cursors.Hand;
            buscarsala.Cursor = Cursors.Hand;
            button2.Cursor = Cursors.Hand;
            button5.Cursor = Cursors.Hand;
            agregar.Cursor = Cursors.Hand;
            Editar.Cursor = Cursors.Hand;


        }

        public int MesaID;
        private int SalaID = 0;
        private int EventoID = 0;
        private int? ClienteIDEvento = null;
        private List<int> mesasSeleccionadasEvento = new List<int>();
        private int estadoBuscarSalaEvento = 1;
        private bool panelSalaEventoVisible = false;

        private void ConfigurarDateTimePickersEvento()
        {
            FechaInicialDTP.Format = DateTimePickerFormat.Custom;
            FechaInicialDTP.CustomFormat = "dd/MM/yyyy HH:mm";
            FechaInicialDTP.ShowUpDown = true;

            FechaFinDTP.Format = DateTimePickerFormat.Custom;
            FechaFinDTP.CustomFormat = "dd/MM/yyyy HH:mm";
            FechaFinDTP.ShowUpDown = true;
        }

        private void guardarbtn_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtnumeroMesa.Text) ||
                string.IsNullOrWhiteSpace(txtcapacidad.Text) ||
                string.IsNullOrWhiteSpace(idsalaconsulta.Text))
            {
                MessageBox.Show("No debe dejar campos vacíos.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtnumeroMesa.Text, out int numeroMesa) || numeroMesa <= 0)
            {
                MessageBox.Show("El número de mesa solo admite números > 0.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtnumeroMesa.Focus();
                return;
            }

            if (!int.TryParse(txtcapacidad.Text, out int capacidad) || capacidad <= 0)
            {
                MessageBox.Show("La capacidad solo admite números > 0.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtcapacidad.Focus();
                return;
            }

            if (!int.TryParse(idsalaconsulta.Text, out int idSala) || idSala <= 0)
            {
                MessageBox.Show("Debe seleccionar una sala válida.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(precioReservatxt.Text, out int precioRes) || precioRes <= 0)
            {
                MessageBox.Show("El precio solo admite números > 0.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                precioReservatxt.Focus();
                return;
            }

            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                try
                {
                    if (MesaID == 0)
                    {
                        string queryInsertar = @"
                        INSERT INTO Mesa (IdSala, Numero, Capacidad, Estado, Ocupado, Reservado, IdGrupo, EsPrincipal, ParaReserva, PrecReserva)
                        VALUES (@IdSala, @Numero, @Capacidad, @Estado, @Ocupado, 0, 0, 0, @EsReserva, @PrecRes);";

                        using (SqlCommand cmd = new SqlCommand(queryInsertar, conexion))
                        {
                            cmd.Parameters.AddWithValue("@IdSala", idSala);
                            cmd.Parameters.AddWithValue("@Numero", numeroMesa);
                            cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                            cmd.Parameters.AddWithValue("@Estado", estadochk.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@Ocupado", ocupadochk.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@EsReserva", esResvchk.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@PrecRes", precioRes);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows > 0)
                            {
                                MessageBox.Show("Mesa registrada con éxito.", "Éxito",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se pudo guardar la mesa.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        string queryActualizar = @"
                        UPDATE Mesa
                        SET IdSala   = @IdSala,
                        Numero   = @Numero,
                        Capacidad= @Capacidad,
                        Estado   = @Estado,
                        Ocupado  = @Ocupado,
                        ParaReserva = @EsReserva,
                        PrecReserva = @PrecRes
                        WHERE IdMesa = @IdMesa;";

                        using (SqlCommand cmd = new SqlCommand(queryActualizar, conexion))
                        {
                            cmd.Parameters.AddWithValue("@IdMesa", MesaID);
                            cmd.Parameters.AddWithValue("@IdSala", idSala);
                            cmd.Parameters.AddWithValue("@Numero", numeroMesa);
                            cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                            cmd.Parameters.AddWithValue("@Estado", estadochk.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@Ocupado", ocupadochk.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@EsReserva", esResvchk.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@PrecRes", precioRes);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows > 0)
                            {
                                MessageBox.Show("Mesa actualizada con éxito.", "Éxito",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar la mesa.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            recargarbtn_Click(sender, e);
            limpiarbtn_Click(sender, e);
        }

        private void limpiarbtn_Click(object sender, EventArgs e)
        {
            MesaID = 0;
            idmesatxt.Clear();
            txtnumeroMesa.Clear();
            txtcapacidad.Clear();
            idsalaconsulta.Clear();
            salatxt.Clear();
            precioReservatxt.Clear();

            ocupadochk.Checked = false;
            estadomesa.Checked = true;
            esResvchk.Checked = false;

            string conexionString = ConexionBD.ConexionSQL();
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                string consultaIdMesa = "SELECT ISNULL(MAX(IdMesa), 0) + 1 FROM Mesa;";
                using (SqlCommand cmd = new SqlCommand(consultaIdMesa, conexion))
                {
                    object resultado = cmd.ExecuteScalar();
                    idmesatxt.Text = Convert.ToInt32(resultado).ToString();
                }
            }
            txtnumeroMesa.Focus();
        }

        private void MantMesas_Load(object sender, EventArgs e)
        {
            string conexionString = ConexionBD.ConexionSQL();

            string consultaIdMesa = "SELECT ISNULL(MAX(IdMesa), 0) + 1 FROM Mesa";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand(consultaIdMesa, conexion))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        idmesatxt.Text = resultado.ToString();
                    }
                    else
                    {
                        idmesatxt.Text = "?";
                    }
                }

                CargarSalaCBX();
                PrepararNuevaSala();
                CargarSalasEnGrid();
                PrepararNuevoEvento();

                CargarMesasDisponiblesEvento();

                ConfigurarDateTimePickersEvento();

                FechaInicialDTP.Value = SistemaFecha.FechaActual;
                FechaFinDTP.Value = SistemaFecha.FechaActual;
                panelOrganizador.Visible = false;
                panelOrganizador.Parent = tabEventos;
                panelOrganizador.Anchor = AnchorStyles.None;
            }

            string consultaIdSala = "SELECT ISNULL(MAX(IdSala), 0) + 1 FROM Sala";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand(consultaIdSala, conexion))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        idsalatxt.Text = resultado.ToString();
                    }
                    else
                    {
                        idsalatxt.Text = "?";
                    }
                }
            }

            txtbuscador.Text = "";
            ActivoChk.Checked = false;
            SeleccionarSalaPanel.Visible = false;
            AplicarFiltrosConsultaMesas();

            CargarSalas("", false);
            PrepararNuevaSala();
        }

        private void CargarPanelMesas(

              string texto = "",
              bool? soloOcupadas = null,
              bool? soloReservadas = null,
              bool? soloLibres = null)
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"
                        SELECT 
                        m.IdMesa,
                        m.IdSala,
                        s.Nombre AS NombreSala,
                        m.Numero,
                        m.Capacidad,
                        m.Ocupado,
                        ISNULL(m.Reservado, 0) AS Reservado,
                        m.Estado
                        FROM Mesa m
                        INNER JOIN Sala s ON m.IdSala = s.IdSala
                        WHERE 1 = 1";

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    sql += @"
                    AND (
                    CAST(m.IdMesa  AS varchar(10)) LIKE @filtro
                    OR CAST(m.Numero AS varchar(10)) LIKE @filtro
                    OR s.Nombre LIKE @filtro)";
                }

                if (soloOcupadas.HasValue && soloOcupadas.Value)
                {
                    sql += " AND m.Ocupado = 1";
                }

                if (soloReservadas.HasValue && soloReservadas.Value)
                {
                    sql += " AND ISNULL(m.Reservado,0) = 1";
                }

                if (soloLibres.HasValue && soloLibres.Value)
                {
                    sql += " AND m.Ocupado = 0";
                }

                sql += " ORDER BY m.IdSala, m.Numero;";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    if (!string.IsNullOrWhiteSpace(texto))
                        comando.Parameters.AddWithValue("@filtro", "%" + texto + "%");

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        panelMesas.Controls.Clear();
                        botonActivo = null;
                        idMesaSeleccionada = -1;

                        while (lector.Read())
                        {
                            Button btnMesa = new Button
                            {
                                Width = 150,
                                Height = 100,
                                Margin = new Padding(10),
                                TextAlign = ContentAlignment.MiddleCenter,
                                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                Cursor = Cursors.Hand
                            };

                            bool ocupada = Convert.ToBoolean(lector["Ocupado"]);
                            bool reservada = Convert.ToBoolean(lector["Reservado"]);

                            if (ocupada)
                                btnMesa.BackColor = Color.LightCoral;
                            else if (reservada)
                                btnMesa.BackColor = Color.MediumPurple;
                            else
                                btnMesa.BackColor = Color.LightGreen;

                            int idMesa = Convert.ToInt32(lector["IdMesa"]);
                            int numero = Convert.ToInt32(lector["Numero"]);
                            string nombreSala = lector["NombreSala"].ToString();
                            int capacidad = Convert.ToInt32(lector["Capacidad"]);

                            btnMesa.Text =
                                $"Mesa #{numero}\nSala: {nombreSala}\nAsientos: {capacidad}";

                            btnMesa.Tag = new MesaInfo
                            {
                                Id = idMesa,
                                Ocupado = ocupada,
                                Reservado = reservada
                            };

                            btnMesa.Click += BtnMesa_Click;

                            panelMesas.Controls.Add(btnMesa);
                        }
                    }
                }
            }
        }

        private void AplicarFiltrosConsultaMesas()
        {
            string texto = txtbuscador.Text.Trim();

            if (texto == "Buscar por numero de Mesa o Sala")
            {
                texto = "";
            }

            bool? soloOcupadas = ConsulOcupadoChk.Checked ? (bool?)true : null;
            bool? soloReservadas = ReservadoChk.Checked ? (bool?)true : null;
            bool? soloLibres = ActivoChk.Checked ? (bool?)true : null;

            CargarPanelMesas(texto, soloOcupadas, soloReservadas, soloLibres);
        }

        private class MesaInfo
        {
            public int Id { get; set; }
            public bool Ocupado { get; set; }
            public bool Reservado { get; set; }
        }

        private Button botonActivo = null;
        private int idMesaSeleccionada = -1;

        private void BtnMesa_Click(object sender, EventArgs e)
        {
            Button btnSeleccionado = sender as Button;
            if (btnSeleccionado == null) return;

            if (botonActivo != null && botonActivo != btnSeleccionado)
            {
                MesaInfo anterior = botonActivo.Tag as MesaInfo;
                if (anterior != null)
                {
                    if (anterior.Ocupado)
                        botonActivo.BackColor = Color.LightCoral;
                    else if (anterior.Reservado)
                        botonActivo.BackColor = Color.MediumPurple;
                    else
                        botonActivo.BackColor = Color.LightGreen;
                }
            }

            botonActivo = btnSeleccionado;
            botonActivo.BackColor = Color.DodgerBlue;

            MesaInfo mesa = botonActivo.Tag as MesaInfo;
            idMesaSeleccionada = (mesa != null) ? mesa.Id : -1;
        }

        private void CargarMesaParaEdicion(int idMesa)
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"
                SELECT 
                m.IdMesa,
                m.IdSala,
                m.Numero,
                m.Capacidad,
                m.Ocupado,
                m.Reservado,
                m.Estado,
                m.ParaReserva,
                m.PrecReserva,
                s.Nombre AS NombreSala
                FROM Mesa m
                LEFT JOIN Sala s ON m.IdSala = s.IdSala
                WHERE m.IdMesa = @IdMesa;";

                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdMesa", idMesa);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {

                            MesaID = Convert.ToInt32(dr["IdMesa"]);
                            idmesatxt.Text = MesaID.ToString();

                            idsalaconsulta.Text = dr["IdSala"].ToString();
                            salatxt.Text = dr["NombreSala"].ToString();

                            txtnumeroMesa.Text = dr["Numero"].ToString();
                            txtcapacidad.Text = dr["Capacidad"].ToString();
                            precioReservatxt.Text = dr["PrecReserva"].ToString();

                            bool ocupado = dr["Ocupado"] != DBNull.Value && Convert.ToBoolean(dr["Ocupado"]);
                            ocupadochk.Checked = ocupado;

                            bool esreserva = dr["ParaReserva"] != DBNull.Value && Convert.ToBoolean(dr["ParaReserva"]);
                            esResvchk.Checked = esreserva;

                            bool estadoMesa = dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"]);
                            estadochk.Checked = estadoMesa;
                        }
                    }
                }
            }

            tabControl2.SelectedTab = tabEventos;
            txtnumeroMesa.Focus();
        }

        private void recargarbtn_Click(object sender, EventArgs e)
        {
            MantMesas_Load(sender, e);
        }

        private void txtbuscador_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltrosConsultaMesas();
        }

        private void eliminarbtn_Click(object sender, EventArgs e)
        {
            txtbuscador.Clear();
            ConsulOcupadoChk.Checked = false;
            ReservadoChk.Checked = false;
            ActivoChk.Checked = false;

            AplicarFiltrosConsultaMesas();
        }
        private void agregar_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 1;
            txtnumeroMesa.Focus();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 2;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnombresala.Text) ||
         string.IsNullOrWhiteSpace(capacidadtxt.Text) ||
         string.IsNullOrWhiteSpace(pisotxt.Text))
            {
                MessageBox.Show("Error: No deje campos vacíos.");
                return;
            }

            if (!int.TryParse(capacidadtxt.Text, out int capacidad) || capacidad < 0)
            {
                MessageBox.Show("La capacidad solo admite números.");
                capacidadtxt.Focus();
                return;
            }

            if (!int.TryParse(pisotxt.Text, out int piso) || piso < 0)
            {
                MessageBox.Show("Piso solo admite números.");
                pisotxt.Focus();
                return;
            }

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                if (SalaID == 0)
                {
                    string sql = @"
                            INSERT INTO Sala (Nombre, Capacidad, Piso, Activo)
                            VALUES (@Nombre, @Capacidad, @Piso, @Activo);
                            SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtnombresala.Text.Trim());
                        cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                        cmd.Parameters.AddWithValue("@Piso", piso);
                        cmd.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);

                        object result = cmd.ExecuteScalar();
                        SalaID = Convert.ToInt32(result);
                    }

                    MessageBox.Show("Sala registrada con éxito.");
                }
                else
                {
                    string sql = @"
                            UPDATE Sala
                            SET Nombre   = @Nombre,
                            Capacidad = @Capacidad,
                            Piso      = @Piso,
                            Activo    = @Activo
                            WHERE IdSala = @IdSala;";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@IdSala", SalaID);
                        cmd.Parameters.AddWithValue("@Nombre", txtnombresala.Text.Trim());
                        cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                        cmd.Parameters.AddWithValue("@Piso", piso);
                        cmd.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Sala actualizada con éxito.");
                }
            }

            idsalatxt.Text = SalaID.ToString();
            CargarSalas(metbuscar.Text.Trim(), metfiltrochk.Checked);
        }

        private void CargarProximoIdSala()
        {
            string conexionString = ConexionBD.ConexionSQL();
            string consultaIdSala = "SELECT ISNULL(MAX(IdSala), 0) + 1 FROM Sala";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand(consultaIdSala, conexion))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                        idsalatxt.Text = resultado.ToString();
                    else
                        idsalatxt.Text = "?";
                }
            }
        }

        private void CargarSalasEnGrid()
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
                SELECT 
                IdSala      AS ID,
                Nombre      AS Nombre,
                Piso        AS Piso,
                Capacidad   AS Capacidad,
                Activo
                FROM Sala
                ORDER BY Nombre;";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    tabladatos.DataSource = dt;
                }
            }

            if (tabladatos.Columns.Contains("ID"))
                tabladatos.Columns["ID"].HeaderText = "ID";

            if (tabladatos.Columns.Contains("Nombre"))
                tabladatos.Columns["Nombre"].HeaderText = "Nombre";

            if (tabladatos.Columns.Contains("Piso"))
                tabladatos.Columns["Piso"].HeaderText = "Piso";

            if (tabladatos.Columns.Contains("Capacidad"))
                tabladatos.Columns["Capacidad"].HeaderText = "Capacidad";

            if (tabladatos.Columns.Contains("Activo"))
                tabladatos.Columns["Activo"].HeaderText = "Activo";
        }

        private void ocupadochk_CheckedChanged(object sender, EventArgs e)
        {
            if (ocupadochk.Checked == true)
            {
                ocupadochk.Text = "Si";
                ocupadochk.ForeColor = Color.Red;
            }
            else
            {
                ocupadochk.Text = "No";
                ocupadochk.ForeColor = Color.Lime;
            }
        }

        private void estadomesa_CheckedChanged(object sender, EventArgs e)
        {
            if (estadomesa.Checked == true)
            {
                estadomesa.Text = "Activo";
                estadomesa.ForeColor = Color.Lime;
            }
            else
            {
                estadomesa.Text = "Inactivo";
                estadomesa.ForeColor = Color.Red;
            }
        }

        private void estadochk_CheckedChanged_1(object sender, EventArgs e)
        {
            if (estadochk.Checked == true)
            {
                estadochk.Text = "Activo";
                estadochk.ForeColor = Color.Lime;
            }
            else
            {
                estadochk.Text = "Inactivo";
                estadochk.ForeColor = Color.Red;
            }
        }

        public int Estadobuscarsala = 1;

        private void buscarsala_Click(object sender, EventArgs e)
        {
            string conexionString = ConexionBD.ConexionSQL();
            string puesto = "SELECT IdSala, Nombre FROM Sala WHERE Activo = 1 ORDER BY Nombre;";

            SqlDataAdapter adaptador = new SqlDataAdapter(puesto, conexionString);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);

            salaconsultadt.DataSource = dt;

            if (Estadobuscarsala == 1)
            {
                buscarsala.Image = Proyecto_restaurante.Properties.Resources.cancelar1;
                toolTip1.SetToolTip(buscarsala, "Cancelar búsqueda");
                salapanel.Visible = true;
                Estadobuscarsala = 0;
            }
            else
            {
                buscarsala.Image = Proyecto_restaurante.Properties.Resources.busqueda1;
                toolTip1.SetToolTip(buscarsala, "Buscar sala");
                salapanel.Visible = false;
                Estadobuscarsala = 1;
            }
        }

        private void salaconsulta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            idconsultatxt.Text = salaconsultadt.Rows[e.RowIndex].Cells["IdSala"].Value.ToString();
            salaconsultatxt.Text = salaconsultadt.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            idsalaconsulta.Text = idconsultatxt.Text;
            salatxt.Text = salaconsultatxt.Text;
            buscarsala_Click(sender, e);
            guardarbtn.Focus();
        }

        private readonly string conexionString = ConexionBD.ConexionSQL();

        private void CargarSalas(string filtro = "", bool soloActivas = false)
        {
            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
                    SELECT 
                    IdSala,
                    Nombre,
                    Piso,
                    Capacidad,
                    Activo
                    FROM Sala
                    WHERE (@f = '' 
                    OR Nombre LIKE '%' + @f + '%' 
                    OR CAST(IdSala AS varchar(10)) LIKE '%' + @f + '%')
                    AND (@soloActivas = 0 OR Activo = 1)
                    ORDER BY Nombre;";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@f", filtro ?? "");
                    da.SelectCommand.Parameters.AddWithValue("@soloActivas", soloActivas ? 1 : 0);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    tabladatos.DataSource = dt;
                }
            }

            if (tabladatos.Columns.Contains("IdSala"))
                tabladatos.Columns["IdSala"].HeaderText = "ID";
            if (tabladatos.Columns.Contains("Nombre"))
                tabladatos.Columns["Nombre"].HeaderText = "Nombre";
            if (tabladatos.Columns.Contains("Piso"))
                tabladatos.Columns["Piso"].HeaderText = "Piso";
            if (tabladatos.Columns.Contains("Capacidad"))
                tabladatos.Columns["Capacidad"].HeaderText = "Capacidad";
            if (tabladatos.Columns.Contains("Activo"))
                tabladatos.Columns["Activo"].HeaderText = "Activo";
        }

        private void PrepararNuevaSala()
        {
            SalaID = 0;
            CargarProximoIdSala();

            txtnombresala.Clear();
            pisotxt.Clear();
            capacidadtxt.Clear();
            estadochk.Checked = true;
            txtnombresala.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PrepararNuevaSala();
        }

        private void metbuscar_TextChanged(object sender, EventArgs e)
        {
            CargarSalas(metbuscar.Text.Trim(), metfiltrochk.Checked);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            metbuscar.Clear();
            metfiltrochk.Checked = false;
            CargarSalas("", false);
            CargarSalasEnGrid();
        }

        private void tabladatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = tabladatos.Rows[e.RowIndex];
            if (row.Cells["IdSala"].Value == null) return;

            SalaID = Convert.ToInt32(row.Cells["IdSala"].Value);
            idsalatxt.Text = SalaID.ToString();

            txtnombresala.Text = row.Cells["Nombre"].Value?.ToString();
            pisotxt.Text = row.Cells["Piso"].Value?.ToString();
            capacidadtxt.Text = row.Cells["Capacidad"].Value?.ToString();

            bool activo = false;
            if (row.Cells["Activo"].Value != DBNull.Value)
                activo = Convert.ToBoolean(row.Cells["Activo"].Value);

            estadochk.Checked = activo;
        }

        private void metfiltrochk_CheckedChanged(object sender, EventArgs e)
        {
            CargarSalas(metbuscar.Text.Trim(), metfiltrochk.Checked);
        }

        private void selecmetodo_Click(object sender, EventArgs e)
        {
            if (tabladatos.CurrentRow == null || tabladatos.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Seleccione una sala en la tabla.");
                return;
            }

            DataGridViewRow row = tabladatos.CurrentRow;

            if (row.Cells["IdSala"].Value == null) return;

            SalaID = Convert.ToInt32(row.Cells["IdSala"].Value);
            idsalatxt.Text = SalaID.ToString();
            txtnombresala.Text = row.Cells["Nombre"].Value?.ToString();
            pisotxt.Text = row.Cells["Piso"].Value?.ToString();
            capacidadtxt.Text = row.Cells["Capacidad"].Value?.ToString();

            bool activo = false;
            if (row.Cells["Activo"].Value != DBNull.Value)
                activo = Convert.ToBoolean(row.Cells["Activo"].Value);

            estadochk.Checked = activo;
        }
        private void Editar_Click(object sender, EventArgs e)
        {
            if (idMesaSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una mesa en la lista.");
                return;
            }

            CargarMesaParaEdicion(idMesaSeleccionada);

            tabControl2.SelectedIndex = 1;
            txtnumeroMesa.Focus();
        }

        private void ConsulOcupadoChk_CheckedChanged(object sender, EventArgs e)
        {
            AplicarFiltrosConsultaMesas();
        }

        private void ReservadoChk_CheckedChanged(object sender, EventArgs e)
        {
            AplicarFiltrosConsultaMesas();
        }

        private void ActivoChk_CheckedChanged(object sender, EventArgs e)
        {
            AplicarFiltrosConsultaMesas();
        }

        private void txtbuscador_Enter(object sender, EventArgs e)
        {
            if (txtbuscador.Text == "Buscar por numero de Mesa o Sala")
            {
                txtbuscador.Text = "";
            }
        }
        private void txtbuscador_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbuscador.Text))
            {
                txtbuscador.Text = "Buscar por numero de Mesa o Sala";
            }
        }

        private void CargarProximoIdEvento()
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                string sql = "SELECT ISNULL(MAX(IdEvento), 0) + 1 FROM Evento;";
                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    object resultado = cmd.ExecuteScalar();
                    IdEventoTxtB.Text = Convert.ToInt32(resultado).ToString();
                }
            }
        }

        private void PrepararNuevoEvento()
        {
            EventoID = 0;
            ClienteIDEvento = null;
            mesasSeleccionadasEvento.Clear();

            CargarProximoIdEvento();

            FechaCreacionDTP.Value = SistemaFecha.FechaActual;
            FechaInicialDTP.Value = SistemaFecha.FechaActual;
            FechaFinDTP.Value = SistemaFecha.FechaActual;

            NomCompletoOrgTxtB.Clear();
            NombreEventoTxt.Clear();
            CantPersonaNUD.Value = 1;

            IdSalaSelecionadaTxtB.Clear();
            NomSalaSelecTxtB.Clear();
            notatxt.Clear();

            EventoMesasP.Controls.Clear();
        }

        private void CargarSalaCBX()
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"
                    SELECT 
                        IdSala,
                        Nombre,
                        Piso,
                        Nombre + ' P' + CAST(Piso AS varchar(10)) AS TextoSala
                    FROM Sala
                    WHERE Activo = 1
                    ORDER BY IdSala ASC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conexion))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    salacmbx.DataSource = dt;
                    salacmbx.DisplayMember = "TextoSala";
                    salacmbx.ValueMember = "IdSala";
                }
            }

            if (salacmbx.Items.Count > 0)
                salacmbx.SelectedIndex = 0;
        }

        private void CargarMesasDisponiblesEvento(string filtro = "")
        {
            string conexionString = ConexionBD.ConexionSQL();

            DateTime fechaIni = FechaInicialDTP.Value;
            DateTime fechaFin = FechaFinDTP.Value;

            int idEventoActual = (EventoID > 0) ? EventoID : 0;

            int? idSalaFiltro = null;

            if (salacmbx.SelectedValue != null &&
                int.TryParse(salacmbx.SelectedValue.ToString(), out int tmpSala) &&
                tmpSala > 0)
            {
                idSalaFiltro = tmpSala;
            }

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"
                SELECT 
                    m.IdMesa,
                    m.IdSala,
                    s.Nombre AS NombreSala,
                    m.Numero,
                    m.Capacidad,
                    m.Ocupado,
                    ISNULL(m.Reservado, 0) AS Reservado, -- (Reserva normal)

                    CASE 
                        WHEN EXISTS (
                            SELECT 1
                            FROM EventoMesa em
                            INNER JOIN Evento e ON e.IdEvento = em.IdEvento
                            WHERE em.IdMesa = m.IdMesa
                              AND e.IdEvento <> @IdEventoActual
                              AND e.Estado <> 'cancelado'
                              AND @FechaIni <= e.FechaFin
                              AND @FechaFin >= e.FechaInicio
                        )
                        THEN 1 ELSE 0
                    END AS ReservadaEvento

                    FROM Mesa m
                    INNER JOIN Sala s ON m.IdSala = s.IdSala
                    WHERE 1 = 1 AND (@IdSala IS NULL OR m.IdSala = @IdSala)";

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    sql += @"
                    AND (
                        CAST(m.IdMesa AS varchar(10)) LIKE @filtro
                        OR CAST(m.Numero AS varchar(10)) LIKE @filtro
                        OR s.Nombre LIKE @filtro
                    )";
                }

                sql += " ORDER BY s.Nombre, m.Numero;";

                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdEventoActual", idEventoActual);
                    cmd.Parameters.AddWithValue("@FechaIni", fechaIni);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@IdSala", (object)idSalaFiltro ?? DBNull.Value);

                    if (!string.IsNullOrWhiteSpace(filtro))
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        EventoMesasP.Controls.Clear();

                        while (dr.Read())
                        {
                            int idMesa = Convert.ToInt32(dr["IdMesa"]);
                            int numero = Convert.ToInt32(dr["Numero"]);
                            string nombreSala = dr["NombreSala"].ToString();
                            int capacidad = Convert.ToInt32(dr["Capacidad"]);

                            bool ocupada = Convert.ToBoolean(dr["Ocupado"]);
                            bool reservadaNormal = Convert.ToInt32(dr["Reservado"]) == 1;
                            bool reservadaEvento = Convert.ToInt32(dr["ReservadaEvento"]) == 1;

                            Button btn = new Button
                            {
                                Width = 130,
                                Height = 90,
                                Margin = new Padding(6),
                                TextAlign = ContentAlignment.MiddleCenter,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                                Cursor = Cursors.Hand
                            };

                            btn.Tag = new
                            {
                                IdMesa = idMesa,
                                Ocupado = ocupada,
                                ReservadoNormal = reservadaNormal,
                                ReservadoEvento = reservadaEvento
                            };

                            bool yaSeleccionada = mesasSeleccionadasEvento.Contains(idMesa);

                            if (ocupada)
                                btn.BackColor = Color.LightCoral;          // Ocupada por comanda
                            else if (reservadaNormal)
                                btn.BackColor = Color.MediumPurple;        // Reservación normal (tu sistema)
                            else if (reservadaEvento)
                                btn.BackColor = Color.LightGray;           // Reservada por evento (diferente a morado)
                            else
                                btn.BackColor = yaSeleccionada ? Color.DodgerBlue : Color.LightGreen;

                            btn.Text =
                                $"Mesa #{numero}\n" +
                                $"Sala: {nombreSala}\n" +
                                $"Asientos: {capacidad}";

                            btn.Click += BtnMesaEvento_Click;
                            EventoMesasP.Controls.Add(btn);
                        }
                    }
                }
            }
        }

        private void BtnMesaEvento_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            dynamic datos = btn.Tag;

            int idMesa = datos.IdMesa;
            bool ocupada = datos.Ocupado;
            bool reservadaNormal = datos.ReservadoNormal;
            bool reservadaEvento = datos.ReservadoEvento;

            if (ocupada || reservadaNormal || reservadaEvento)
            {
                string motivo =
                    ocupada ? "Está ocupada por una orden." :
                    reservadaNormal ? "Está reservada (reservación normal)." :
                    "Está reservada por otro evento en esas fechas.";

                MessageBox.Show(
                    "Esta mesa no está disponible para asignar al evento.\nMotivo: " + motivo,
                    "Mesa no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (mesasSeleccionadasEvento.Contains(idMesa))
            {
                mesasSeleccionadasEvento.Remove(idMesa);
                btn.BackColor = Color.LightGreen;
            }
            else
            {
                mesasSeleccionadasEvento.Add(idMesa);
                btn.BackColor = Color.DodgerBlue;
            }
        }

        private void BuscarMesaTxtB_TextChanged(object sender, EventArgs e)
        {
            string filtro = BuscarMesaTxtB.Text.Trim();
            CargarMesasDisponiblesEvento(filtro);
        }

        private void ActualizarFormEventoBtn_Click(object sender, EventArgs e)
        {
            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
        }

        private void NuevoEventoBtn_Click(object sender, EventArgs e)
        {
            PrepararNuevoEvento();
            CargarMesasDisponiblesEvento();
            NombreEventoTxt.Focus();
        }
        private void GuardarEventoBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreEventoTxt.Text))
            {
                MessageBox.Show("Debe escribir el nombre del evento.");
                NombreEventoTxt.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(NomCompletoOrgTxtB.Text))
            {
                MessageBox.Show("Debe indicar el organizador.");
                NomCompletoOrgTxtB.Focus();
                return;
            }

            if (ClienteIDEvento == null)
            {
                MessageBox.Show("Debe seleccionar un organizador válido (cliente).", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                BuscarOrganizadorBtn.Focus();
                return;
            }

            if (CantPersonaNUD.Value <= 0)
            {
                MessageBox.Show("La cantidad de personas debe ser mayor que 0.");
                CantPersonaNUD.Focus();
                return;
            }

            if (FechaFinDTP.Value < FechaInicialDTP.Value)
            {
                MessageBox.Show("La fecha de fin no puede ser menor que la fecha de inicio.");
                return;
            }

            if (mesasSeleccionadasEvento.Count == 0)
            {
                MessageBox.Show("Debe seleccionar al menos una mesa para el evento.");
                return;
            }

            int? idSalaEvento = null;

            if (salacmbx.SelectedValue != null &&
                int.TryParse(salacmbx.SelectedValue.ToString(), out int tmpSala))
            {
                idSalaEvento = tmpSala;
            }

            string organizador = NomCompletoOrgTxtB.Text.Trim();
            string nombreEvento = NombreEventoTxt.Text.Trim();
            int personas = (int)CantPersonaNUD.Value;
            DateTime fechaIni = FechaInicialDTP.Value;
            DateTime fechaFin = FechaFinDTP.Value;

            string nota = null;

            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                SqlTransaction trans = conexion.BeginTransaction();

                try
                {
                    // 1) VALIDAR mesas (no ocupadas ni reservadas)
                    foreach (int idMesa in mesasSeleccionadasEvento)
                    {
                        string sqlCheck = "SELECT Ocupado, ISNULL(Reservado,0) AS Reservado FROM Mesa WHERE IdMesa = @IdMesa;";
                        using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conexion, trans))
                        {
                            cmdCheck.Parameters.AddWithValue("@IdMesa", idMesa);

                            using (SqlDataReader dr = cmdCheck.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    bool ocupada = Convert.ToBoolean(dr["Ocupado"]);
                                    bool reservada = Convert.ToBoolean(dr["Reservado"]);

                                    if (ocupada || reservada)
                                    {
                                        trans.Rollback();
                                        MessageBox.Show("La mesa seleccionada ya está reservada. Elegir otra");
                                        CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    foreach (int idMesa in mesasSeleccionadasEvento)
                    {
                        string sqlSalaMesa = "SELECT IdSala FROM Mesa WHERE IdMesa = @IdMesa;";
                        using (SqlCommand cmdSala = new SqlCommand(sqlSalaMesa, conexion, trans))
                        {
                            cmdSala.Parameters.AddWithValue("@IdMesa", idMesa);

                            object result = cmdSala.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                trans.Rollback();
                                MessageBox.Show("No se pudo determinar la sala de una de las mesas seleccionadas.");
                                return;
                            }

                            int idSalaMesa = Convert.ToInt32(result);

                            if (idSalaEvento == null)
                            {
                                idSalaEvento = idSalaMesa;
                            }
                            else if (idSalaEvento != idSalaMesa)
                            {
                                trans.Rollback();
                                MessageBox.Show("Las mesas seleccionadas pertenecen a salas distintas. Seleccione mesas de una sola sala.");
                                return;
                            }
                        }
                    }

                    // 2) INSERT / UPDATE Evento
                    if (EventoID == 0)
                    {
                        string sqlInsert = @"
                        INSERT INTO Evento
                        (Organizador, FechaInicio, FechaFin, PersonasEstimadas, IdSala, MontajeMin, DesmontajeMin, Estado, CreadoEn, NombreEvento, IdCliente, Nota)
                        VALUES
                        (@Organizador, @FechaInicio, @FechaFin, @Personas, @IdSala, @MontajeMin, @DesmontajeMin, @Estado, SYSDATETIME(), @NombreEvento, @IdCliente, @Nota);
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

                        using (SqlCommand cmd = new SqlCommand(sqlInsert, conexion, trans))
                        {
                            cmd.Parameters.AddWithValue("@Organizador", organizador);
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaIni);
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                            cmd.Parameters.AddWithValue("@Personas", personas);
                            cmd.Parameters.AddWithValue("@IdSala", (object)idSalaEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@MontajeMin", 0);
                            cmd.Parameters.AddWithValue("@DesmontajeMin", 0);
                            cmd.Parameters.AddWithValue("@Estado", "planeado");
                            cmd.Parameters.AddWithValue("@NombreEvento", nombreEvento);
                            cmd.Parameters.AddWithValue("@IdCliente", (object)ClienteIDEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Nota", (object)nota ?? DBNull.Value);

                            EventoID = (int)cmd.ExecuteScalar();
                            IdEventoTxtB.Text = EventoID.ToString();
                        }
                    }
                    else
                    {
                        string sqlUpdate = @"
                            UPDATE Evento
                            SET Organizador        = @Organizador,
                            FechaInicio        = @FechaInicio,
                            FechaFin           = @FechaFin,
                            PersonasEstimadas  = @Personas,
                            IdSala             = @IdSala,
                            NombreEvento       = @NombreEvento,
                            IdCliente          = @IdCliente,
                            Nota               = @Nota
                            WHERE IdEvento = @IdEvento;";

                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, conexion, trans))
                        {
                            cmd.Parameters.AddWithValue("@IdEvento", EventoID);
                            cmd.Parameters.AddWithValue("@Organizador", organizador);
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaIni);
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                            cmd.Parameters.AddWithValue("@Personas", personas);
                            cmd.Parameters.AddWithValue("@IdSala", (object)idSalaEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NombreEvento", nombreEvento);
                            cmd.Parameters.AddWithValue("@IdCliente", (object)ClienteIDEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Nota", (object)nota ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }

                        string sqlDeleteMesas = "DELETE FROM EventoMesa WHERE IdEvento = @IdEvento;";
                        using (SqlCommand cmdDel = new SqlCommand(sqlDeleteMesas, conexion, trans))
                        {
                            cmdDel.Parameters.AddWithValue("@IdEvento", EventoID);
                            cmdDel.ExecuteNonQuery();
                        }
                    }

                    string sqlInsertMesa = "INSERT INTO EventoMesa (IdEvento, IdMesa) VALUES (@IdEvento, @IdMesa);";
                    foreach (int idMesa in mesasSeleccionadasEvento)
                    {
                        using (SqlCommand cmdMesa = new SqlCommand(sqlInsertMesa, conexion, trans))
                        {
                            cmdMesa.Parameters.AddWithValue("@IdEvento", EventoID);
                            cmdMesa.Parameters.AddWithValue("@IdMesa", idMesa);
                            cmdMesa.ExecuteNonQuery();
                        }
                    }

                    foreach (int idMesa in mesasSeleccionadasEvento)
                    {
                        string sqlRes = "UPDATE Mesa SET Reservado = 1 WHERE IdMesa = @IdMesa;";
                        using (SqlCommand cmdRes = new SqlCommand(sqlRes, conexion, trans))
                        {
                            cmdRes.Parameters.AddWithValue("@IdMesa", idMesa);
                            cmdRes.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();

                    MessageBox.Show("Evento guardado correctamente.");

                    PrepararNuevoEvento();
                    CargarMesasDisponiblesEvento();
                    NombreEventoTxt.Focus();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Error al guardar el evento: " + ex.Message);
                }
            }
        }

        private void BuscarOrganizadorBtn_Click(object sender, EventArgs e)
        {
            panelOrganizador.Visible = !panelOrganizador.Visible;

            if (panelOrganizador.Visible)
            {

                int x = (tabEventos.ClientSize.Width - panelOrganizador.Width) / 2;
                int y = (tabEventos.ClientSize.Height - panelOrganizador.Height) / 2;

                panelOrganizador.Location = new Point(x, y);
                panelOrganizador.BringToFront();

                BuscarOrganizadorTxtB.Text = "";
                FiltroActivoChk.Checked = true;
                CargarOrganizadores("", true);

                BuscarOrganizadorTxtB.Focus();
            }
        }
        private void BuscarOrganizadorTxtB_TextChanged(object sender, EventArgs e)
        {
            string texto = BuscarOrganizadorTxtB.Text.Trim();
            bool soloActivos = FiltroActivoChk.Checked;
            CargarOrganizadores(texto, soloActivos);
        }

        private void FiltroActivoChk_CheckedChanged(object sender, EventArgs e)
        {
            string texto = BuscarOrganizadorTxtB.Text.Trim();
            bool soloActivos = FiltroActivoChk.Checked;
            CargarOrganizadores(texto, soloActivos);
        }

        private void PersonaDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = PersonaDGV.Rows[e.RowIndex];

            if (fila.Cells["IdCliente"].Value == null) return;

            ClienteIDEvento = Convert.ToInt32(fila.Cells["IdCliente"].Value);

            NomCompletoOrgTxtB.Text = fila.Cells["NombreCompleto"].Value?.ToString();

            panelOrganizador.Visible = false;

            NombreEventoTxt.Focus();
        }

        private void CargarOrganizadores(string filtroTexto, bool soloActivos)
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"
                SELECT 
                c.IdCliente,
                p.NombreCompleto,
                p.Email,
                c.Activo
                FROM Cliente c
                INNER JOIN Persona p ON c.IdPersona = p.IdPersona
                WHERE 1 = 1 
                  AND c.IdCliente > 1";

                if (!string.IsNullOrWhiteSpace(filtroTexto))
                {
                    sql += @"
                AND (
                    p.NombreCompleto LIKE @filtro
                    OR p.Email LIKE @filtro
                )";
                }

                if (soloActivos)
                {
                    sql += " AND c.Activo = 1 AND p.Activo = 1";
                }

                sql += " ORDER BY p.NombreCompleto;";

                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    if (!string.IsNullOrWhiteSpace(filtroTexto))
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtroTexto + "%");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        PersonaDGV.DataSource = dt;
                    }
                }
            }

            if (PersonaDGV.Columns.Contains("IdCliente"))
                PersonaDGV.Columns["IdCliente"].HeaderText = "ID";
            if (PersonaDGV.Columns.Contains("NombreCompleto"))
                PersonaDGV.Columns["NombreCompleto"].HeaderText = "Nombre";
            if (PersonaDGV.Columns.Contains("Email"))
                PersonaDGV.Columns["Email"].HeaderText = "Correo";
            if (PersonaDGV.Columns.Contains("Activo"))
                PersonaDGV.Columns["Activo"].HeaderText = "Activo";
        }
        private void ActualizarCCBtn_Click(object sender, EventArgs e)
        {
            string texto = BuscarOrganizadorTxtB.Text.Trim();
            bool soloActivos = FiltroActivoChk.Checked;
            CargarOrganizadores(texto, soloActivos);
        }

        private void LimpiarCCBtn_Click(object sender, EventArgs e)
        {
            BuscarOrganizadorTxtB.Text = "";
            FiltroActivoChk.Checked = true;
            CargarOrganizadores("", true);
        }

        private void BuscarSalaBtn_Click(object sender, EventArgs e)
        {
            string filtro = NombreSalaTxtB.Text.Trim();

            if (estadoBuscarSalaEvento == 1)
            {
                CargarSalasParaEvento(filtro);
                SeleccionarSalaPanel.Visible = true;

                BuscarSalaBtn.Image = Proyecto_restaurante.Properties.Resources.cancelar1;
                toolTip1.SetToolTip(BuscarSalaBtn, "Cerrar selección de sala");

                estadoBuscarSalaEvento = 0;
                NombreSalaTxtB.Focus();
            }
            else
            {
                SeleccionarSalaPanel.Visible = false;

                BuscarSalaBtn.Image = Proyecto_restaurante.Properties.Resources.busqueda1;
                toolTip1.SetToolTip(BuscarSalaBtn, "Buscar sala");

                estadoBuscarSalaEvento = 1;
            }
        }

        private void CargarSalasParaEvento(string filtro = "")
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
                SELECT 
                IdSala,
                Nombre,
                Piso,
                Capacidad,
                Activo
                FROM Sala
                WHERE
                (@filtro = '' 
                 OR Nombre LIKE '%' + @filtro + '%' 
                 OR CAST(IdSala AS varchar(10)) LIKE '%' + @filtro + '%')
                AND Activo = 1
                ORDER BY Nombre;";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@filtro", filtro ?? "");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    SalaConEventoDGV.DataSource = dt;
                }
            }

            if (SalaConEventoDGV.Columns.Contains("IdSala"))
                SalaConEventoDGV.Columns["IdSala"].HeaderText = "ID";
            if (SalaConEventoDGV.Columns.Contains("Nombre"))
                SalaConEventoDGV.Columns["Nombre"].HeaderText = "Sala";
            if (SalaConEventoDGV.Columns.Contains("Piso"))
                SalaConEventoDGV.Columns["Piso"].HeaderText = "Piso";
            if (SalaConEventoDGV.Columns.Contains("Capacidad"))
                SalaConEventoDGV.Columns["Capacidad"].HeaderText = "Capacidad";
            if (SalaConEventoDGV.Columns.Contains("Activo"))
                SalaConEventoDGV.Columns["Activo"].HeaderText = "Activo";

        }
        private void DesplegarBtn_Click(object sender, EventArgs e)
        {
            if (SalaConEventoDGV.CurrentRow == null || SalaConEventoDGV.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Seleccione una sala en la lista.");
                return;
            }

            DataGridViewRow row = SalaConEventoDGV.CurrentRow;

            if (!SalaConEventoDGV.Columns.Contains("IdSala") ||
                row.Cells["IdSala"].Value == null ||
                row.Cells["IdSala"].Value == DBNull.Value)
                return;

            int idSala = Convert.ToInt32(row.Cells["IdSala"].Value);
            string nombreSala = null;

            if (SalaConEventoDGV.Columns.Contains("Nombre"))
                nombreSala = row.Cells["Nombre"].Value?.ToString();

            IdSalaSelecionadaTxtB.Text = idSala.ToString();
            NomSalaSelecTxtB.Text = nombreSala;

            int colCap = -1;
            if (SalaConEventoDGV.Columns.Contains("Capacidad"))
                colCap = SalaConEventoDGV.Columns["Capacidad"].Index;

            if (colCap >= 0)
            {
                object valorCap = row.Cells[colCap].Value;

                if (valorCap != null && valorCap != DBNull.Value)
                {
                    if (int.TryParse(valorCap.ToString(), out int capacidad))
                    {
                        if (capacidad < CantPersonaNUD.Minimum)
                            capacidad = (int)CantPersonaNUD.Minimum;

                        if (capacidad > CantPersonaNUD.Maximum)
                            capacidad = (int)CantPersonaNUD.Maximum;

                        CantPersonaNUD.Value = capacidad;
                    }
                }
            }

            SeleccionarSalaPanel.Visible = false;
            estadoBuscarSalaEvento = 1;
            BuscarSalaBtn.Image = Proyecto_restaurante.Properties.Resources.busqueda1;
            toolTip1.SetToolTip(BuscarSalaBtn, "Buscar sala");
            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());

        }
        private void NombreSalaTxtB_TextChanged(object sender, EventArgs e)
        {
            string filtro = NombreSalaTxtB.Text.Trim();
            CargarSalasParaEvento(filtro);
        }
        private void SalaConEventoDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            SalaConEventoDGV.CurrentCell = SalaConEventoDGV.Rows[e.RowIndex].Cells[0];
            DesplegarBtn_Click(sender, EventArgs.Empty);
        }

        private void notatxt_Enter(object sender, EventArgs e)
        {
            if (notatxt.Text == "Escribir nota aquí...")
            {
                notatxt.Text = "";
                notatxt.ForeColor = Color.Black;

            }
        }
        private void notatxt_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(notatxt.Text))
            {
                notatxt.Text = "Escribir nota aquí...";
                notatxt.ForeColor = Color.Gray;
            }
        }
        private void LimpiarFormEventoBtn_Click(object sender, EventArgs e)
        {
            PrepararNuevoEvento();
            CargarMesasDisponiblesEvento();
            NombreEventoTxt.Focus();
        }
        private void MostrarOcultarPanelOrganizador()
        {
            if (!panelOrganizador.Visible)
            {
                int x = (tabEventos.ClientSize.Width - panelOrganizador.Width) / 2;
                int y = (tabEventos.ClientSize.Height - panelOrganizador.Height) / 2;

                panelOrganizador.Location = new Point(x, y);
                panelOrganizador.BringToFront();
                panelOrganizador.Visible = true;

                BuscarOrganizadorTxtB.Text = "";
                FiltroActivoChk.Checked = true;
                CargarOrganizadores("", true);
                BuscarOrganizadorTxtB.Focus();
            }
            else
            {
                panelOrganizador.Visible = false;
            }
        }

        private void RegresarBtn_Click(object sender, EventArgs e)
        {
            panelOrganizador.Visible = false;

            if (PersonaDGV.CurrentRow != null)
                PersonaDGV.ClearSelection();

            NomCompletoOrgTxtB.Focus();
        }

        private void FechaInicialDTP_ValueChanged(object sender, EventArgs e)
        {
            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
        }

        private void FechaFinDTP_ValueChanged(object sender, EventArgs e)
        {
            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
        }

        private void esResvchk_CheckedChanged(object sender, EventArgs e)
        {
            if (esResvchk.Checked == true)
            {
                esResvchk.Text = "Si";
                esResvchk.ForeColor = Color.Lime;
            }
            else
            {
                esResvchk.Text = "No";
                esResvchk.ForeColor = Color.Red;
            }
        }

        private void salacmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
            mesasSeleccionadasEvento.Clear();
        }
    }
}