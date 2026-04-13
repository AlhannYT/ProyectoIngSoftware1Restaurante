using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Proyecto_restaurante.menu;
using System.Globalization;

namespace Proyecto_restaurante
{
    public partial class Reservacion : Form
    {
        public Reservacion()
        {
            InitializeComponent();
        }

        private int ReservaID = 0;
        private int? ClienteIDReserva = null;
        private Button botonActivo = null;
        private int idMesaSeleccionada = -1;

        public int MesaID;
        private int SalaID = 0;
        private int EventoID = 0;
        private int Origen = 2; // 2 = reserva, 3 = evento
        private int? ClienteIDEvento = null;
        private List<int> mesasSeleccionadasEvento = new List<int>();
        private Dictionary<int, decimal> preciosMesasSeleccionadasEvento = new Dictionary<int, decimal>();
        private int estadoBuscarSalaEvento = 1;
        private bool panelSalaEventoVisible = false;

        private decimal Total;
        decimal TotalPedido = 0m;
        decimal TotalAplicado = 0m;
        decimal TotalRestante = 0;

        private decimal totalAcumulado = 0;
        private decimal subtotalAcumulado = 0;
        public string comprobanteFinal;
        bool cargandoOrden = false;
        bool cargandoGrupos = false;

        string conexionString = ConexionBD.ConexionSQL();

        private class MesaInfoReserva
        {
            public int Id { get; set; }
            public bool Ocupado { get; set; }
            public bool Reservado { get; set; }
        }
        private int? ObtenerIdReservaSeleccionada()
        {
            if (ReservacionMesasDGV.CurrentRow == null ||
                ReservacionMesasDGV.CurrentRow.IsNewRow)
                return null;

            if (ReservacionMesasDGV.CurrentRow.Cells["ID"].Value == null)
                return null;

            return Convert.ToInt32(ReservacionMesasDGV.CurrentRow.Cells["ID"].Value);
        }

        private int? ObtenerIdEventoSeleccionado()
        {
            if (ReservacionMesasDGV.CurrentRow == null ||
                ReservacionMesasDGV.CurrentRow.IsNewRow)
                return null;

            if (ReservacionMesasDGV.CurrentRow.Cells["ID"].Value == null)
                return null;

            return Convert.ToInt32(ReservacionMesasDGV.CurrentRow.Cells["ID"].Value);
        }

        private void BtnMesa_Click(object sender, EventArgs e)
        {
            Button btnSeleccionado = sender as Button;
            if (btnSeleccionado == null) return;

            if (botonActivo != null && botonActivo != btnSeleccionado)
            {
                var infoAnterior = botonActivo.Tag as MesaInfoReserva;
                if (infoAnterior != null)
                {
                    if (infoAnterior.Ocupado)
                        botonActivo.BackColor = Color.LightCoral;
                    else if (infoAnterior.Reservado)
                        botonActivo.BackColor = Color.MediumPurple;
                    else
                        botonActivo.BackColor = Color.LightGreen;
                }
            }

            botonActivo = btnSeleccionado;
            botonActivo.BackColor = Color.DodgerBlue;

            var info = botonActivo.Tag as MesaInfoReserva;
            idMesaSeleccionada = (info != null) ? info.Id : -1;
        }

        private void Reservacion_Load(object sender, EventArgs e)
        {
            tipoReservacmbx.SelectedIndex = 0;
            CargarSalaCBX();

            PanelClientes.Visible = false;

            try
            {
                LiberarReservasVencidas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar reservas vencidas: " + ex.Message);
            }

            PrepararNuevaReserva();
            CargarMesasDisponiblesReserva();

            fecini.Value = DateTime.Today;
            fecfin.Value = DateTime.Today;
            CargarReservas();

            CargarSalaCBX();
            PrepararNuevoEvento();

            CargarMesasDisponiblesEvento();

            FechaInicialDTP.Value = SistemaFecha.FechaActual;
            FechaFinDTP.Value = SistemaFecha.FechaActual;
            panelOrganizador.Visible = false;
            panelOrganizador.Parent = tabEventos;
            panelOrganizador.Anchor = AnchorStyles.None;
        }

        private void PrepararNuevaReserva()
        {
            ReservaID = 0;
            ClienteIDReserva = null;
            idMesaSeleccionada = -1;
            botonActivo = null;

            CargarProximoIdReserva();

            fechacreacionreserva.Value = SistemaFecha.FechaActual;
            fechaResv.Value = SistemaFecha.FechaActual;

            idclientetxt.Clear();
            txtnombrecompleto.Clear();
            txtnumero_cliente.Clear();

            CantidadPersonasNUD.Value = 1;

            CargarMesasDisponiblesReserva();
        }
        private void CargarProximoIdReserva()
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                string sql = "SELECT ISNULL(MAX(IdReserva), 0) + 1 FROM Reserva;";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    object res = cmd.ExecuteScalar();
                    txtidreserva.Text = Convert.ToInt32(res).ToString();
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

            idcliente2.Clear();
            NombreEventoTxt.Clear();

            cedulacliente2.Clear();
            numerocliente2.Clear();
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

                string sql2 = @"
                    SELECT 
                        IdSala,
                        Nombre,
                        Piso
                    FROM Sala
                    WHERE Activo = 1
                    ORDER BY IdSala ASC";

                using (SqlDataAdapter da2 = new SqlDataAdapter(sql2, conexion))
                {
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);

                    salacmbx2.DataSource = dt2;
                    salacmbx2.DisplayMember = "Nombre";
                    salacmbx2.ValueMember = "IdSala";
                }
            }

            if (salacmbx.Items.Count > 0)
                salacmbx.SelectedIndex = 0;

            if (salacmbx2.Items.Count > 0)
                salacmbx2.SelectedIndex = 0;
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

        private void nuevobtn_Click(object sender, EventArgs e)
        {
            try
            {
                LiberarReservasVencidas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar reservas vencidas: " + ex.Message);
            }

            PrepararNuevaReserva();
        }

        private void CargarMesasDisponiblesReserva()
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
                ISNULL(m.Reservado,0) AS Reservado
                FROM Mesa m
                INNER JOIN Sala s ON m.IdSala = s.IdSala
                ORDER BY s.Nombre, m.Numero;";

                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    flowmesa.Controls.Clear();
                    botonActivo = null;
                    idMesaSeleccionada = -1;

                    while (dr.Read())
                    {
                        int idMesa = Convert.ToInt32(dr["IdMesa"]);
                        int numero = Convert.ToInt32(dr["Numero"]);
                        string nombreSala = dr["NombreSala"].ToString();
                        int capacidad = Convert.ToInt32(dr["Capacidad"]);
                        bool ocupado = Convert.ToBoolean(dr["Ocupado"]);
                        bool reservado = Convert.ToBoolean(dr["Reservado"]);

                        Button btnMesa = new Button
                        {
                            Width = 150,
                            Height = 100,
                            Margin = new Padding(10),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Cursor = Cursors.Hand,
                            Tag = new MesaInfoReserva
                            {
                                Id = idMesa,
                                Ocupado = ocupado,
                                Reservado = reservado
                            }
                        };

                        if (ocupado)
                            btnMesa.BackColor = Color.LightCoral;
                        else if (reservado)
                            btnMesa.BackColor = Color.MediumPurple;
                        else
                            btnMesa.BackColor = Color.LightGreen;

                        btnMesa.Text =
                            $"Mesa #{numero}\n" +
                            $"Sala: {nombreSala}\n" +
                            $"Asientos: {capacidad}";

                        btnMesa.Click += BtnMesa_Click;

                        flowmesa.Controls.Add(btnMesa);
                    }
                }
            }
        }
        private void LiberarReservasVencidas()
        {
            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {

                    string sqlCancelVencidas = @"
                                    UPDATE Reserva
                                    SET Estado = 'cancelada'
                                    WHERE Estado = 'solicitada'
                                    AND FechaHora < SYSDATETIME();";

                    using (SqlCommand cmd = new SqlCommand(sqlCancelVencidas, con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string sqlLiberarMesas = @"
                        UPDATE m
                        SET m.Reservado = 0
                        FROM Mesa m
                        WHERE ISNULL(m.Reservado,0) = 1
                        AND NOT EXISTS (
                        SELECT 1
                        FROM Reserva r
                        WHERE r.IdMesa = m.IdMesa
                        AND r.Estado IN ('solicitada','confirmada')
                        AND r.FechaHora >= SYSDATETIME()
                        );";

                    using (SqlCommand cmd = new SqlCommand(sqlLiberarMesas, con, tran))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        private void guardareservabtn_Click(object sender, EventArgs e)
        {
            if (idMesaSeleccionada <= 0)
            {
                MessageBox.Show("Debe seleccionar una mesa.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtnombrecompleto.Text))
            {
                MessageBox.Show("Debe seleccionar un cliente.");
                return;
            }

            if (CantidadPersonasNUD.Value <= 0)
            {
                MessageBox.Show("La cantidad de personas debe ser mayor que 0.");
                return;
            }

            int idMesa = idMesaSeleccionada;
            DateTime fechaReserva = fechaResv.Value;
            int personas = (int)CantidadPersonasNUD.Value;
            string nombreCliente = txtnombrecompleto.Text.Trim();

            string conexionString = ConexionBD.ConexionSQL();

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sqlCheck = "SELECT Ocupado, ISNULL(Reservado,0) AS Reservado FROM Mesa WHERE IdMesa = @IdMesa;";
                using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, con))
                {
                    cmdCheck.Parameters.AddWithValue("@IdMesa", idMesa);

                    using (SqlDataReader dr = cmdCheck.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            bool ocupado = Convert.ToBoolean(dr["Ocupado"]);
                            bool reservado = Convert.ToBoolean(dr["Reservado"]);

                            if (ocupado || reservado)
                            {
                                MessageBox.Show("Esa mesa ya no está disponible. Presiona Nuevo para recargar.");
                                return;
                            }
                        }
                    }
                }

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    if (ReservaID == 0)
                    {
                        string sqlInsert = @"
                                INSERT INTO Reserva
                                (IdMesa, FechaHora, Personas, Cliente, Estado, CreadoEn)
                                VALUES
                                (@IdMesa, @FechaHora, @Personas, @Cliente, 'solicitada', SYSDATETIME());
                                SELECT CAST(SCOPE_IDENTITY() AS int);";

                        using (SqlCommand cmd = new SqlCommand(sqlInsert, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@IdMesa", idMesa);
                            cmd.Parameters.AddWithValue("@FechaHora", fechaReserva);
                            cmd.Parameters.AddWithValue("@Personas", personas);
                            cmd.Parameters.AddWithValue("@Cliente", nombreCliente);

                            ReservaID = (int)cmd.ExecuteScalar();
                            txtidreserva.Text = ReservaID.ToString();
                        }

                        string sqlUpdateMesa = @"
                            UPDATE Mesa
                            SET Reservado = 1
                            WHERE IdMesa = @IdMesa;";

                        using (SqlCommand cmdMesa = new SqlCommand(sqlUpdateMesa, con, trans))
                        {
                            cmdMesa.Parameters.AddWithValue("@IdMesa", idMesa);
                            cmdMesa.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sqlUpdate = @"
                            UPDATE Reserva
                            SET IdMesa    = @IdMesa,
                            FechaHora = @FechaHora,
                            Personas  = @Personas,
                            Cliente   = @Cliente
                            WHERE IdReserva = @IdReserva;";

                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@IdReserva", ReservaID);
                            cmd.Parameters.AddWithValue("@IdMesa", idMesa);
                            cmd.Parameters.AddWithValue("@FechaHora", fechaReserva);
                            cmd.Parameters.AddWithValue("@Personas", personas);
                            cmd.Parameters.AddWithValue("@Cliente", nombreCliente);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    MessageBox.Show("Reserva guardada correctamente.");

                    PrepararNuevaReserva();
                    CargarMesasDisponiblesReserva();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Error al guardar la reserva: " + ex.Message);
                }
            }
        }


        private void buscarclientebtn_Click(object sender, EventArgs e)
        {
            PanelClientes.Visible = !PanelClientes.Visible;

            if (PanelClientes.Visible)
            {
                PanelClientes.BringToFront();
                PanelClientes.Location = new Point(0, 0);
                txtbuscador.Text = "";
                filtrochk.Checked = true;
                CargarClientesReserva("", true);
                txtbuscador.Focus();
            }
        }

        private void CargarClientesReserva(string filtroTexto, bool soloActivos)
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
                    OR p.Email LIKE @filtro)";
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
                        tabladatoscliente.DataSource = dt;
                    }
                }
            }

            if (tabladatoscliente.Columns.Contains("IdCliente"))
                tabladatoscliente.Columns["IdCliente"].HeaderText = "ID";
            if (tabladatoscliente.Columns.Contains("NombreCompleto"))
                tabladatoscliente.Columns["NombreCompleto"].HeaderText = "Nombre";
            if (tabladatoscliente.Columns.Contains("Email"))
                tabladatoscliente.Columns["Email"].HeaderText = "Correo";
            if (tabladatoscliente.Columns.Contains("Activo"))
                tabladatoscliente.Columns["Activo"].HeaderText = "Activo";
        }

        private void CargarClientesEvento(string filtroTexto, bool soloActivos)
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
                    OR p.Email LIKE @filtro)";
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
                        tabladatosclienteEV.DataSource = dt;
                    }
                }
            }

            if (tabladatosclienteEV.Columns.Contains("IdCliente"))
                tabladatosclienteEV.Columns["IdCliente"].HeaderText = "ID";
            if (tabladatosclienteEV.Columns.Contains("NombreCompleto"))
                tabladatosclienteEV.Columns["NombreCompleto"].HeaderText = "Nombre";
            if (tabladatosclienteEV.Columns.Contains("Email"))
                tabladatosclienteEV.Columns["Email"].HeaderText = "Correo";
            if (tabladatosclienteEV.Columns.Contains("Activo"))
                tabladatosclienteEV.Columns["Activo"].HeaderText = "Activo";
        }

        private void txtbuscador_TextChanged(object sender, EventArgs e)
        {
            CargarClientesReserva(txtbuscador.Text.Trim(), filtrochk.Checked);
        }

        private void filtrochk_CheckedChanged(object sender, EventArgs e)
        {
            CargarClientesReserva(txtbuscador.Text.Trim(), filtrochk.Checked);
        }

        private void recargarbtn_Click(object sender, EventArgs e)
        {
            txtbuscador.Clear();
            filtrochk.Checked = true;
            CargarClientesReserva("", true);
        }

        private void eliminarbtn_Click(object sender, EventArgs e)
        {
            txtbuscador.Clear();
        }

        private void tabladatoscliente_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = tabladatoscliente.Rows[e.RowIndex];
            if (fila.Cells["IdCliente"].Value == null) return;

            ClienteIDReserva = Convert.ToInt32(fila.Cells["IdCliente"].Value);
            idclientetxt.Text = ClienteIDReserva.ToString();
            txtnombrecompleto.Text = fila.Cells["NombreCompleto"].Value?.ToString();

            PanelClientes.Visible = false;
        }
        private void salirclientebtn_Click(object sender, EventArgs e)
        {
            PanelClientes.Visible = false;
        }

        private void CargarReservas(string texto = "")
        {
            string conexionString = ConexionBD.ConexionSQL();

            DateTime desde = fecini.Value.Date;
            DateTime hasta = fecfin.Value.Date;

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = "";
                string filtro = string.IsNullOrWhiteSpace(texto) ? "" : texto.Trim();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Desde", desde);
                    cmd.Parameters.AddWithValue("@Hasta", hasta);
                    cmd.Parameters.AddWithValue("@Texto", filtro);
                    cmd.Parameters.AddWithValue("@Filtro", "%" + filtro + "%");

                    if (tipoReservacmbx.SelectedIndex == 0)
                    {
                        sql = @"
                        SELECT 
                            r.IdReserva AS ID,
                            r.Cliente,
                            r.FechaHora,
                            r.Estado,
                            ISNULL(r.TotalRes, 0) AS Total
                        FROM Reserva r
                        WHERE 
                            r.FechaHora >= @Desde
                            AND r.FechaHora < DATEADD(day, 1, @Hasta)
                            AND (
                                @Texto = '' OR
                                CAST(r.IdReserva AS varchar(10)) LIKE @Filtro OR
                                r.Cliente LIKE @Filtro OR
                                r.Estado LIKE @Filtro
                            )
                        ORDER BY r.FechaHora DESC;";
                    }
                    else if (tipoReservacmbx.SelectedIndex == 1)
                    {
                        sql = @"
                        SELECT 
                            e.IdEvento AS ID,
                            e.Organizador,
                            e.Estado,
                            e.FechaInicio,
                            e.TotalRes
                        FROM Evento e
                        WHERE 
                            e.FechaInicio < DATEADD(day, 1, @Hasta)
                            AND e.FechaFin >= @Desde
                            AND (
                                @Texto = '' OR
                                CAST(e.IdEvento AS varchar(10)) LIKE @Filtro OR
                                e.Organizador LIKE @Filtro OR
                                e.Estado LIKE @Filtro OR
                                e.NombreEvento LIKE @Filtro
                            )
                        ORDER BY e.FechaInicio DESC;";
                    }
                    else
                    {
                        ReservacionMesasDGV.DataSource = null;
                        return;
                    }

                    cmd.CommandText = sql;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        ReservacionMesasDGV.DataSource = dt;
                    }
                }
            }

            if (tipoReservacmbx.SelectedIndex == 0)
            {
                if (ReservacionMesasDGV.Columns.Contains("ID"))
                    ReservacionMesasDGV.Columns["ID"].HeaderText = "ID";
                if (ReservacionMesasDGV.Columns.Contains("Cliente"))
                    ReservacionMesasDGV.Columns["Cliente"].HeaderText = "Cliente";
                if (ReservacionMesasDGV.Columns.Contains("FechaHora"))
                    ReservacionMesasDGV.Columns["FechaHora"].HeaderText = "Fecha / Hora";
                if (ReservacionMesasDGV.Columns.Contains("Estado"))
                    ReservacionMesasDGV.Columns["Estado"].HeaderText = "Estado";
                if (ReservacionMesasDGV.Columns.Contains("Total"))
                    ReservacionMesasDGV.Columns["Total"].HeaderText = "Total";
            }
            else if (tipoReservacmbx.SelectedIndex == 1)
            {
                if (ReservacionMesasDGV.Columns.Contains("ID"))
                    ReservacionMesasDGV.Columns["ID"].HeaderText = "ID";
                if (ReservacionMesasDGV.Columns.Contains("Organizador"))
                    ReservacionMesasDGV.Columns["Organizador"].HeaderText = "Organizador";
                if (ReservacionMesasDGV.Columns.Contains("Estado"))
                    ReservacionMesasDGV.Columns["Estado"].HeaderText = "Estado";
                if (ReservacionMesasDGV.Columns.Contains("FechaInicio"))
                    ReservacionMesasDGV.Columns["FechaInicio"].HeaderText = "Fecha Inicio";
                if (ReservacionMesasDGV.Columns.Contains("FechaFin"))
                    ReservacionMesasDGV.Columns["FechaFin"].HeaderText = "Fecha Fin";
            }
        }

        private void txtbusquedareserva_TextChanged(object sender, EventArgs e)
        {
            CargarReservas(txtbusquedareserva.Text);
        }

        private void ordenbtn_Click(object sender, EventArgs e)
        {
            Total = 0;

            if (ReservacionMesasDGV.CurrentRow == null || ReservacionMesasDGV.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Seleccione una registro para su orden.");
                return;
            }

            int idReserva = Convert.ToInt32(ReservacionMesasDGV.CurrentRow.Cells["ID"].Value);

            if (tipoReservacmbx.SelectedIndex == 0)
            {
                Origen = 2;

                if (Total <= 0)
                {
                    using (SqlConnection cn = new SqlConnection(conexionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(
                            "SELECT TotalRes FROM Reserva WHERE IdReserva = @id",
                            cn
                        );
                        cmd.Parameters.AddWithValue("@id", idReserva);
                        object vTotal = cmd.ExecuteScalar();

                        if (vTotal == null || vTotal == DBNull.Value)
                        {
                            MessageBox.Show("No se pudo obtener el total de la reserva.");
                            return;
                        }

                        Total = Convert.ToDecimal(vTotal);
                    }
                }
            }
            else if (tipoReservacmbx.SelectedIndex == 1)
            {
                Origen = 3;

                if (Total <= 0)
                {
                    using (SqlConnection cn = new SqlConnection(conexionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(
                            "SELECT TotalRes FROM Evento WHERE IdEvento = @id",
                            cn
                        );
                        cmd.Parameters.AddWithValue("@id", idReserva);
                        object vTotal = cmd.ExecuteScalar();

                        if (vTotal == null || vTotal == DBNull.Value)
                        {
                            MessageBox.Show("No se pudo obtener el total del evento.");
                            return;
                        }

                        Total = Convert.ToDecimal(vTotal);
                    }
                }
            }

            detallepanelcompleto.Visible = true;
            detallepanelcompleto.BringToFront();
            detallepanelcompleto.Location = new Point(0, 0);
            detallepagopanel.Visible = true;

            if (detallePagoDT.ColumnCount == 0)
            {
                detallePagoDT.Columns.Add("tipodetalle", "Tipo");
                detallePagoDT.Columns.Add("referencia", "Referencia");
                detallePagoDT.Columns.Add("origen", "Origen");
                detallePagoDT.Columns.Add("monto", "Aplicado");

                TotalPedido = Convert.ToDecimal(Total);
                TotalRestante = TotalPedido;
                TotalAPagar.Text = TotalPedido.ToString("N2");
                restante1txt.Text = TotalRestante.ToString("N2");
                restante2txt.Text = TotalRestante.ToString("N2");
                restante3txt.Text = TotalRestante.ToString("N2");
                efectivotxt.Text = TotalPedido.ToString("N2");
                tarjetaMonto.Text = TotalPedido.ToString("N2");
                transfMonto.Text = TotalPedido.ToString("N2");
            }
        }


        private void RegresarBtn_Click(object sender, EventArgs e)
        {
            PanelClientes.Visible = false;
            panelOrganizador.Visible = false;

            if (tabladatoscliente.CurrentRow != null)
                tabladatoscliente.ClearSelection();

            if (tabladatosclienteEV.CurrentRow != null)
                tabladatosclienteEV.ClearSelection();
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
            decimal precioMesa = datos.PrecioMesa;

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
                if (preciosMesasSeleccionadasEvento.ContainsKey(idMesa))
                    preciosMesasSeleccionadasEvento.Remove(idMesa);

                btn.BackColor = Color.LightGreen;
            }
            else
            {
                mesasSeleccionadasEvento.Add(idMesa);
                preciosMesasSeleccionadasEvento[idMesa] = precioMesa;

                btn.BackColor = Color.DodgerBlue;
            }

            ActualizarResumenMesasEvento();
            cantMesasLista.Text = mesasSeleccionadasEvento.Count.ToString();
        }

        private void CargarMesasDisponiblesEvento(string filtro = "")
        {
            string conexionString = ConexionBD.ConexionSQL();

            DateTime fechaIni = FechaInicialDTP.Value;
            DateTime fechaFin = FechaFinDTP.Value;

            int idEventoActual = (EventoID > 0) ? EventoID : 0;

            int? idSalaFiltro = null;

            if (salacmbx.SelectedValue != null && int.TryParse(salacmbx.SelectedValue.ToString(), out int tmpSala) && tmpSala > 0)
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
                    ISNULL(m.Reservado, 0) AS Reservado,
                    ISNULL(m.PrecReserva, 0) AS PrecReserva,

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
                            decimal precioMesa = Convert.ToDecimal(dr["PrecReserva"]);

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
                                ReservadoEvento = reservadaEvento,
                                PrecioMesa = precioMesa
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

        private void ActualizarResumenMesasEvento()
        {
            cantMesasLista.Text = mesasSeleccionadasEvento.Count.ToString();

            decimal subtotal = preciosMesasSeleccionadasEvento.Values.Sum();
            decimal itbis = subtotal * 0.18m;
            decimal total = subtotal + itbis;

            labelSubtotalEV.Text = subtotal.ToString("N2");
            labelTotalEV.Text = total.ToString("N2");
        }

        private void BuscarMesaTxtB_TextChanged(object sender, EventArgs e)
        {
            string filtro = BuscarMesaTxtB.Text.Trim();
            CargarMesasDisponiblesEvento(filtro);
        }

        private void salacmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
            mesasSeleccionadasEvento.Clear();
        }

        private void GuardarEventoBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreEventoTxt.Text))
            {
                MessageBox.Show("Debe escribir el nombre del evento.");
                NombreEventoTxt.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(nombrecliente2.Text))
            {
                MessageBox.Show("Debe indicar el organizador.");
                ClienteEVbtn.Focus();
                return;
            }

            if (ClienteIDEvento == null)
            {
                MessageBox.Show("Debe seleccionar un organizador válido (cliente).", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClienteEVbtn.Focus();
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

            string organizador = nombrecliente2.Text.Trim();
            string nombreEvento = NombreEventoTxt.Text.Trim();
            DateTime fechaIni = FechaInicialDTP.Value;
            DateTime fechaFin = FechaFinDTP.Value;
            decimal totalEvento = decimal.Parse(labelTotalEV.Text);

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
                        (Organizador, FechaInicio, FechaFin, IdSala, MontajeMin, DesmontajeMin, Estado, CreadoEn, NombreEvento, IdCliente, Nota, TotalRes)
                        VALUES
                        (@Organizador, @FechaInicio, @FechaFin, @IdSala, @MontajeMin, @DesmontajeMin, @Estado, SYSDATETIME(), @NombreEvento, @IdCliente, @Nota, @Total);
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

                        using (SqlCommand cmd = new SqlCommand(sqlInsert, conexion, trans))
                        {
                            cmd.Parameters.AddWithValue("@Organizador", organizador);
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaIni);
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                            cmd.Parameters.AddWithValue("@IdSala", (object)idSalaEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@MontajeMin", 0);
                            cmd.Parameters.AddWithValue("@DesmontajeMin", 0);
                            cmd.Parameters.AddWithValue("@Estado", "planeado");
                            cmd.Parameters.AddWithValue("@NombreEvento", nombreEvento);
                            cmd.Parameters.AddWithValue("@IdCliente", (object)ClienteIDEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Nota", (object)nota ?? DBNull.Value);

                            cmd.Parameters.Add("@Total", SqlDbType.Decimal).Value = totalEvento;
                            cmd.Parameters["@Total"].Precision = 10;
                            cmd.Parameters["@Total"].Scale = 2;

                            EventoID = (int)cmd.ExecuteScalar();
                            IdEventoTxtB.Text = EventoID.ToString();
                        }
                    }
                    else
                    {
                        string sqlUpdate = @"
                        UPDATE Evento
                        SET Organizador  = @Organizador,
                            FechaInicio  = @FechaInicio,
                            FechaFin     = @FechaFin,
                            IdSala       = @IdSala,
                            NombreEvento = @NombreEvento,
                            IdCliente    = @IdCliente,
                            Nota         = @Nota,
                            TotalRes     = @Total
                        WHERE IdEvento = @IdEvento;";

                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, conexion, trans))
                        {
                            cmd.Parameters.AddWithValue("@IdEvento", EventoID);
                            cmd.Parameters.AddWithValue("@Organizador", organizador);
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaIni);
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                            cmd.Parameters.AddWithValue("@IdSala", (object)idSalaEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NombreEvento", nombreEvento);
                            cmd.Parameters.AddWithValue("@IdCliente", (object)ClienteIDEvento ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Nota", (object)nota ?? DBNull.Value);

                            cmd.Parameters.Add("@Total", SqlDbType.Decimal).Value = totalEvento;
                            cmd.Parameters["@Total"].Precision = 10;
                            cmd.Parameters["@Total"].Scale = 2;

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

        private void ClienteEVbtn_Click(object sender, EventArgs e)
        {
            panelOrganizador.Visible = !panelOrganizador.Visible;

            if (panelOrganizador.Visible)
            {
                panelOrganizador.BringToFront();
                panelOrganizador.Location = new Point(0, 0);
                txtbuscador.Text = "";
                filtrochk.Checked = true;
                CargarClientesEvento("", true);
                txtbuscadorEV.Focus();
            }
        }

        private void tabladatosclienteEV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = tabladatosclienteEV.Rows[e.RowIndex];
            if (fila.Cells["IdCliente"].Value == null) return;

            ClienteIDEvento = Convert.ToInt32(fila.Cells["IdCliente"].Value);
            idcliente2.Text = ClienteIDReserva.ToString();
            nombrecliente2.Text = fila.Cells["NombreCompleto"].Value?.ToString();

            panelOrganizador.Visible = false;
        }

        private void txtbuscadorEV_TextChanged(object sender, EventArgs e)
        {
            CargarClientesEvento(txtbuscadorEV.Text.Trim(), filtrochk2.Checked);
        }

        private void NuevoEventoBtn_Click(object sender, EventArgs e)
        {
            PrepararNuevoEvento();
            CargarMesasDisponiblesEvento();
            mesasSeleccionadasEvento.Clear();
            preciosMesasSeleccionadasEvento.Clear();
            ActualizarResumenMesasEvento();
        }

        private void salacmbx2_SelectedIndexChanged(object sender, EventArgs e)
        {
            mesasSeleccionadasEvento.Clear();
            preciosMesasSeleccionadasEvento.Clear();

            ActualizarResumenMesasEvento();

            CargarMesasDisponiblesEvento(BuscarMesaTxtB.Text.Trim());
        }

        private void buscarBTN_Click(object sender, EventArgs e)
        {
            CargarReservas(txtbusquedareserva.Text.Trim());
        }

        private void tipoReservacmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReservas(txtbusquedareserva.Text.Trim());
        }

        class PagoEfectivoInfo
        {
            public decimal MontoDado { get; set; }
            public decimal MontoAplicado { get; set; }
            public decimal Devuelta { get; set; }
        }

        private List<PagoEfectivoInfo> pagosEfectivo = new List<PagoEfectivoInfo>();

        private void aplicarefectivo_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(efectivotxt.Text, out decimal montoDado))
            {
                MessageBox.Show("Monto inválido.");
                return;
            }

            if (montoDado <= 0)
            {
                MessageBox.Show("Debe ingresar un monto válido.");
                return;
            }

            decimal aplicado = montoDado;
            decimal devueltaCalc = 0;

            if (montoDado > TotalRestante)
            {
                aplicado = TotalRestante;
                devueltaCalc = montoDado - TotalRestante;
            }

            pagosEfectivo.Add(new PagoEfectivoInfo
            {
                MontoDado = montoDado,
                MontoAplicado = aplicado,
                Devuelta = devueltaCalc
            });

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(detallePagoDT);
            row.Cells[0].Value = "EF";
            row.Cells[1].Value = "";
            row.Cells[2].Value = "Efectivo";
            row.Cells[3].Value = montoDado;

            detallePagoDT.Rows.Add(row);

            devueltatxt.Text = devueltaCalc.ToString("N2");

            efectivotxt.Clear();
            button1.Enabled = true;
            RecalcularTotalesPago();
            RecargarRestante();
        }

        private void RecalcularTotalAplicado()
        {
            TotalAplicado = 0;

            foreach (DataGridViewRow fila in detallePagoDT.Rows)
            {
                if (fila.IsNewRow) continue;
                TotalAplicado += Convert.ToDecimal(fila.Cells[3].Value);
            }

            pagadotxt.Text = TotalAplicado.ToString("N2");

            MostrarDevuelta();
        }

        private void RecalcularTotalesPago()
        {
            TotalAplicado = 0;

            foreach (DataGridViewRow fila in detallePagoDT.Rows)
            {
                if (fila.IsNewRow) continue;
                TotalAplicado += Convert.ToDecimal(fila.Cells[3].Value);
            }

            TotalRestante = TotalPedido - TotalAplicado;

            if (TotalRestante < 0)
                TotalRestante = 0;

            pagadotxt.Text = TotalAplicado.ToString("N2");
            restante1txt.Text = TotalRestante.ToString("N2");
            restante2txt.Text = TotalRestante.ToString("N2");
            restante3txt.Text = TotalRestante.ToString("N2");

            MostrarDevuelta();
        }

        private void RecargarRestante()
        {
            if (restante1txt.Text == "0.00" || restante2txt.Text == "0.00" || restante3txt.Text == "0.00")
            {
                efectivotxt.Clear();
                tarjetaMonto.Clear();
                transfMonto.Clear();

                aplicarefectivo.Enabled = false;
                aplicartarjeta.Enabled = false;
                aplicartransf.Enabled = false;
            }
            else
            {
                aplicarefectivo.Enabled = true;
                aplicartarjeta.Enabled = true;
                aplicartransf.Enabled = true;
            }

            if (restante1txt.Text == TotalAPagar.Text || restante2txt.Text == TotalAPagar.Text || restante2txt.Text == TotalAPagar.Text)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }

            efectivotxt.Text = restante1txt.Text;
            tarjetaMonto.Text = restante2txt.Text;
            transfMonto.Text = restante3txt.Text;
        }

        private void MostrarDevuelta()
        {
            if (TotalAplicado >= TotalPedido)
            {
                devueltatxt.Text = (TotalAplicado - TotalPedido).ToString("N2");
                totalpagar.Text = TotalAPagar.Text;
            }
            else
            {
                devueltatxt.Text = "0.00";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            detallepagopanel.Visible = false;
            detallepagopanel.Location = new Point(1617, 6);

            EfectuarReserva();
            MostrarDevuelta();

            devueltapanel.Visible = true;
            devueltapanel.Location = new Point(466, 0);
        }

        private void EfectuarReserva()
        {
            int? id = ObtenerIdReservaSeleccionada();
            if (id == null)
            {
                MessageBox.Show("Seleccione una reservación en la lista.");
                return;
            }

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    if (Origen == 2) // Reserva normal
                    {
                        int idMesa = 0;

                        string sqlGetMesa = "SELECT IdMesa FROM Reserva WHERE IdReserva = @Id;";
                        using (SqlCommand cmdGet = new SqlCommand(sqlGetMesa, con, tran))
                        {
                            cmdGet.Parameters.AddWithValue("@Id", id.Value);
                            object res = cmdGet.ExecuteScalar();

                            if (res != null && res != DBNull.Value)
                                idMesa = Convert.ToInt32(res);
                        }

                        string sqlReserva = "UPDATE Reserva SET Estado = 'confirmada' WHERE IdReserva = @Id;";
                        using (SqlCommand cmd = new SqlCommand(sqlReserva, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", id.Value);
                            cmd.ExecuteNonQuery();
                        }

                        if (idMesa > 0)
                        {
                            string sqlMesa = "UPDATE Mesa SET Reservado = 1 WHERE IdMesa = @IdMesa;";
                            using (SqlCommand cmdMesa = new SqlCommand(sqlMesa, con, tran))
                            {
                                cmdMesa.Parameters.AddWithValue("@IdMesa", idMesa);
                                cmdMesa.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        MessageBox.Show("Reservación confirmada.");
                    }
                    else if (Origen == 3) // Evento
                    {
                        string sqlEvento = "UPDATE Evento SET Estado = 'confirmado' WHERE IdEvento = @Id;";
                        using (SqlCommand cmd = new SqlCommand(sqlEvento, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", id.Value);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlMesasEvento = @"
                                UPDATE Mesa
                                SET Reservado = 1
                                WHERE IdMesa IN (
                                    SELECT IdMesa
                                    FROM EventoMesa
                                    WHERE IdEvento = @Id
                                );";

                        using (SqlCommand cmdMesa = new SqlCommand(sqlMesasEvento, con, tran))
                        {
                            cmdMesa.Parameters.AddWithValue("@Id", id.Value);
                            cmdMesa.ExecuteNonQuery();
                        }

                        tran.Commit();
                        MessageBox.Show("Evento confirmado.");
                    }
                    else
                    {
                        tran.Rollback();
                        MessageBox.Show("Origen no válido.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error al confirmar: " + ex.Message);
                    return;
                }

                RegistrarPago(con, tran);
            }

            CargarReservas(txtbusquedareserva.Text);
            CargarMesasDisponiblesReserva();

        }


        private void RegistrarPago(SqlConnection conexion, SqlTransaction trans)
        {
            int indexEfectivo = 0;

            foreach (DataGridViewRow fila in detallePagoDT.Rows)
            {
                if (fila.IsNewRow) continue;

                string tipo = fila.Cells[0].Value.ToString();
                string referencia = fila.Cells[1].Value?.ToString() ?? "";
                string origen = fila.Cells[2].Value?.ToString() ?? "";

                decimal totalAplicadoBD = 0;
                decimal efectivoDado = 0;
                decimal devuelta = 0;
                decimal tarjeta = 0;
                decimal transferencia = 0;

                string tarjetaNombre = DBNull.Value.ToString();
                string banco = DBNull.Value.ToString();

                string tipoSQL = "";
                if (tipo == "EF") tipoSQL = "Efectivo";
                else if (tipo == "TJ") tipoSQL = "Tarjeta";
                else if (tipo == "TR") tipoSQL = "Transferencia";
                else tipoSQL = tipo;

                if (tipo == "EF")
                {
                    var pago = pagosEfectivo[indexEfectivo];

                    efectivoDado = pago.MontoDado;
                    totalAplicadoBD = pago.MontoAplicado;
                    devuelta = pago.Devuelta;

                    indexEfectivo++;
                }
                else if (tipo == "TJ")
                {
                    tarjeta = Convert.ToDecimal(fila.Cells[3].Value);
                    totalAplicadoBD = tarjeta;
                    tarjetaNombre = origen;
                }
                else if (tipo == "TR")
                {
                    transferencia = Convert.ToDecimal(fila.Cells[3].Value);
                    totalAplicadoBD = transferencia;
                    banco = origen;
                }

                SqlCommand cmd = new SqlCommand(@"
                INSERT INTO DetallePago (IdPedido, TipoDetalle, Efectivo, Devuelta, Tarjeta, TarjetaNombre, Transferencia, Banco, Total, Estado, Referencia, Origen)
                VALUES (NULL, @TipoDetalle, @Efectivo, @Devuelta, @Tarjeta, @TarjetaNombre, @Transferencia, @Banco, @Total, @Estado, @Referencia, 1)",
                conexion, trans);

                cmd.Parameters.AddWithValue("@TipoDetalle", tipoSQL);
                cmd.Parameters.AddWithValue("@Efectivo", efectivoDado);
                cmd.Parameters.AddWithValue("@Devuelta", devuelta);
                cmd.Parameters.AddWithValue("@Tarjeta", tarjeta);
                cmd.Parameters.AddWithValue("@TarjetaNombre", string.IsNullOrWhiteSpace(tarjetaNombre) ? DBNull.Value : (object)tarjetaNombre);
                cmd.Parameters.AddWithValue("@Transferencia", transferencia);
                cmd.Parameters.AddWithValue("@Banco", string.IsNullOrWhiteSpace(banco) ? DBNull.Value : (object)banco);

                cmd.Parameters.AddWithValue("@Total", totalAplicadoBD);

                cmd.Parameters.AddWithValue("@Estado", 1);
                cmd.Parameters.AddWithValue("@Referencia", referencia);
                cmd.Parameters.AddWithValue("@Origen", Origen);

                cmd.ExecuteNonQuery();
            }
        }

        private void aplicartarjeta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tarjetaref.Text) || tarjetacmbx.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar tarjeta y referencia.");
                return;
            }

            if (!decimal.TryParse(tarjetaMonto.Text, out decimal monto))
            {
                MessageBox.Show("Monto inválido.");
                return;
            }

            if (monto > TotalRestante)
            {
                MessageBox.Show($"El monto de tarjeta excede el restante ({TotalRestante:N2}).");
                return;
            }

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(detallePagoDT);
            row.Cells[0].Value = "TJ";
            row.Cells[1].Value = tarjetaref.Text;
            row.Cells[2].Value = tarjetacmbx.Text;
            row.Cells[3].Value = monto;

            detallePagoDT.Rows.Add(row);

            tarjetaref.Clear();
            tarjetaMonto.Clear();

            RecalcularTotalesPago();
        }

        private void aplicartransf_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bancoref.Text) || bancocmbx.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar banco y referencia.");
                return;
            }

            if (!decimal.TryParse(transfMonto.Text, out decimal monto))
            {
                MessageBox.Show("Monto inválido.");
                return;
            }

            if (monto > TotalRestante)
            {
                MessageBox.Show($"El monto excede el restante ({TotalRestante:N2}).");
                return;
            }

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(detallePagoDT);
            row.Cells[0].Value = "TR";
            row.Cells[1].Value = bancoref.Text;
            row.Cells[2].Value = bancocmbx.Text;
            row.Cells[3].Value = monto;

            detallePagoDT.Rows.Add(row);

            bancoref.Clear();
            transfMonto.Clear();

            RecalcularTotalesPago();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            detallepanelcompleto.Visible = false;
            detallepanelcompleto.Location = new Point(1617, 6);
            efectivotxt.Clear();
            tarjetaref.Clear();
            bancoref.Clear();
        }

        private void eliminarDetalle_Click(object sender, EventArgs e)
        {
            if (detallePagoDT.Rows.Count == 0)
            {
                MessageBox.Show("No hay detalles para eliminar.");
                button1.Enabled = false;
                return;
            }

            foreach (DataGridViewRow fila in detallePagoDT.SelectedRows)
            {
                if (!fila.IsNewRow)
                    detallePagoDT.Rows.Remove(fila);
            }

            RecalcularTotalAplicado();
            RecalcularTotalesPago();
            RecargarRestante();
        }

        private void cancelarreservabtn_Click(object sender, EventArgs e)
        {
            //int? id = ObtenerIdReservaSeleccionada();
            //if (id == null)
            //{
            //    MessageBox.Show("Seleccione una reservación en la lista.");
            //    return;
            //}

            //DialogResult r = MessageBox.Show("¿Cancelar esta reservación?",
            //                                 "Cancelar", MessageBoxButtons.YesNo,
            //                                 MessageBoxIcon.Warning);
            //if (r != DialogResult.Yes) return;

            //string conexionString = ConexionBD.ConexionSQL();

            //using (SqlConnection con = new SqlConnection(conexionString))
            //{
            //    con.Open();
            //    SqlTransaction tran = con.BeginTransaction();

            //    try
            //    {
            //        int idMesa = 0;
            //        string estadoActual = "";

            //        string sqlGet = "SELECT IdMesa, Estado FROM Reserva WHERE IdReserva = @Id;";
            //        using (SqlCommand cmdGet = new SqlCommand(sqlGet, con, tran))
            //        {
            //            cmdGet.Parameters.AddWithValue("@Id", id.Value);

            //            using (SqlDataReader dr = cmdGet.ExecuteReader())
            //            {
            //                if (dr.Read())
            //                {
            //                    idMesa = Convert.ToInt32(dr["IdMesa"]);
            //                    estadoActual = dr["Estado"]?.ToString() ?? "";
            //                }
            //                else
            //                {
            //                    tran.Rollback();
            //                    MessageBox.Show("No se encontró la reservación.");
            //                    return;
            //                }
            //            }
            //        }

            //        if (estadoActual == "cancelada")
            //        {
            //            tran.Rollback();
            //            MessageBox.Show("Esa reservación ya estaba cancelada.");
            //            return;
            //        }

            //        string sqlCancel = "UPDATE Reserva SET Estado = 'cancelada' WHERE IdReserva = @Id;";
            //        using (SqlCommand cmd = new SqlCommand(sqlCancel, con, tran))
            //        {
            //            cmd.Parameters.AddWithValue("@Id", id.Value);
            //            cmd.ExecuteNonQuery();
            //        }

            //        if (idMesa > 0)
            //        {
            //            string sqlHayOtras = @"
            //                SELECT COUNT(*) 
            //                FROM Reserva
            //                WHERE IdMesa = @IdMesa
            //                AND Estado IN ('solicitada','confirmada');";

            //            int activas = 0;
            //            using (SqlCommand cmdCount = new SqlCommand(sqlHayOtras, con, tran))
            //            {
            //                cmdCount.Parameters.AddWithValue("@IdMesa", idMesa);
            //                activas = Convert.ToInt32(cmdCount.ExecuteScalar());
            //            }

            //            if (activas == 0)
            //            {
            //                string sqlFreeMesa = "UPDATE Mesa SET Reservado = 0 WHERE IdMesa = @IdMesa;";
            //                using (SqlCommand cmdFree = new SqlCommand(sqlFreeMesa, con, tran))
            //                {
            //                    cmdFree.Parameters.AddWithValue("@IdMesa", idMesa);
            //                    cmdFree.ExecuteNonQuery();
            //                }
            //            }
            //        }

            //        tran.Commit();
            //        MessageBox.Show("Reservación cancelada y mesa actualizada.");

            //        CargarReservas(txtbusquedareserva.Text);
            //        CargarMesasDisponiblesReserva();
            //    }
            //    catch (Exception ex)
            //    {
            //        tran.Rollback();
            //        MessageBox.Show("Error al cancelar: " + ex.Message);
            //    }
            //}
        }

        private void restante1txt_TextChanged(object sender, EventArgs e)
        {
            RecargarRestante();
        }

        private void restante2txt_TextChanged(object sender, EventArgs e)
        {
            RecargarRestante();
        }

        private void restante3txt_TextChanged(object sender, EventArgs e)
        {
            RecargarRestante();
        }

        private void finalizarbtn_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
            detallepagopanel.Visible = true;
            detallepagopanel.Location = new Point(476, 0);

            devueltapanel.Visible = false;
            devueltapanel.Location = new Point(0, 0);
        }
    }
}