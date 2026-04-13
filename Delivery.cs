using PdfSharp.Drawing;
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

namespace Proyecto_restaurante
{
    public partial class Delivery : Form
    {
        public Delivery()
        {
            InitializeComponent();
        }

        private int cantidadProd = 0;
        private string pedidoActual;
        public string NombrePC;
        private int PedidoID;
        private int EditarEstado = 0;
        private decimal Total;
        private int TipoPago = 0;
        private decimal TotalPedido = 0m;
        private decimal TotalAplicado = 0m;
        private decimal totalAcumulado = 0;
        private decimal subtotalAcumulado = 0;
        private string IdClientePersonaST = "1"; //Al contado por defecto
        private string vehiculoAsignado = "";
        private string motivoAsignacion = "";
        bool modoEntregar = false;

        private List<Panel> ordenesSeleccionadas = new List<Panel>();

        decimal TotalRestante = 0;

        bool cargandoOrden = false;

        string conexionString = ConexionBD.ConexionSQL();

        private void Delivery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.D1)
            {
                tabControl1.SelectedIndex = 0;
            }

            if (e.Alt && e.KeyCode == Keys.D2)
            {
                tabControl1.SelectedIndex = 1;
            }

            if (e.Alt && e.KeyCode == Keys.D3)
            {
                tabControl1.SelectedIndex = 2;
            }
        }

        private void Delivery_Load(object sender, EventArgs e)
        {
            fechapedido.Value = SistemaFecha.FechaActual;

            if (!cargandoOrden)
                tipoComp.SelectedIndex = 1;

            tipodoccmbx.SelectedIndex = 1;

            string consulta = "SELECT TOP 1 IdPedido FROM Pedido ORDER BY IdPedido DESC";
            string busquedaCaja = @"
            SELECT 
                c.Nombre AS nombre_caja,
                c.Numero AS numero_caja
            FROM Configuracion conf
            INNER JOIN Caja c
                ON conf.IdCaja = c.IdCaja
            WHERE conf.NombrePC = @NombrePC";


            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(consulta, con))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        int nuevoId = Convert.ToInt32(resultado) + 1;
                        txtidpedido.Text = nuevoId.ToString();
                    }
                    else
                    {
                        //MessageBox.Show("No se encontraron pedidos.");
                        txtidpedido.Text = "1";
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                using (SqlCommand cmdBusCaja = new SqlCommand(busquedaCaja, con))
                {
                    cmdBusCaja.Parameters.AddWithValue("@NombrePC", NombrePC);

                    using (SqlDataReader reader = cmdBusCaja.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string nombreCaja = reader["nombre_caja"].ToString();
                            labelcaja.Text = $"{nombreCaja}";
                        }
                        else
                        {
                            labelcaja.Text = "Caja no encontrada.";
                        }
                    }
                }
            }

            if (detalleorden.ColumnCount == 0)
            {
                detalleorden.Columns.Add("codigoProducto", "Codigo");
                detalleorden.Columns.Add("nombreProducto", "Nombre");
                detalleorden.Columns.Add("precio", "Precio");
                detalleorden.Columns.Add("ITBIS", "ITBIS");
                detalleorden.Columns.Add("cantidad", "Cantidad");
                detalleorden.Columns.Add("subtotal", "Importe");
            }

            cmbZonaEntrega.Items.Clear();
            cmbZonaEntrega.Items.Add("Centro");
            cmbZonaEntrega.Items.Add("Norte");
            cmbZonaEntrega.Items.Add("Sur");
            cmbZonaEntrega.Items.Add("Este");
            cmbZonaEntrega.Items.Add("Oeste");
            cmbZonaEntrega.Items.Add("Periferia");

            if (cmbZonaEntrega.Items.Count > 0)
                cmbZonaEntrega.SelectedIndex = 0;

            Comprobantes();

            panelResenas.Visible = false;
            panelResenas.BringToFront();

            panelResenas.Left = (this.ClientSize.Width - panelResenas.Width) / 2;
            panelResenas.Top = (this.ClientSize.Height - panelResenas.Height) / 2;

            CargarResumenResenas();
        }


        private void Comprobantes()
        {
            if (cargandoOrden) return;

            int tipo = tipoComp.SelectedIndex == 0 ? 1 : 2;

            string query = "SELECT TOP 1 SecuenciaActual FROM Comprobantes WHERE Tipo = @Tipo ORDER BY IdComprobante DESC";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Tipo", tipo);

                    object resultado = cmd.ExecuteScalar();

                    int nuevaSecuencia = 1;

                    if (resultado != null && resultado != DBNull.Value)
                        nuevaSecuencia = Convert.ToInt32(resultado) + 1;

                    Comprobantetxt.Text = nuevaSecuencia.ToString("D8");
                }
            }
        }

        private void buscarproductobtn_Click(object sender, EventArgs e)
        {
            panelproducto.Location = new Point(0, 0);
            panelproducto.BringToFront();
            panelproducto.Visible = true;

            string consulta = @"
                SELECT 
                    pv.IdProducto,
                    pv.CodigoBarra,
                    pv.Nombre,
                    pv.PrecioVenta,
                    pv.Itbis,
                    pv.Existencia
                FROM ProductoVenta pv
                INNER JOIN ProductoTipo pt
                ON pv.IdProductoTipo = pt.IdProductoTipo
                WHERE 
                pv.Activo = 1
                AND pt.Ingrediente = 0;";
            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexionString);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);
            tablapanelproducto.DataSource = dt;

            tablapanelproducto.Columns["IdProducto"].HeaderText = "ID";
            tablapanelproducto.Columns["CodigoBarra"].HeaderText = "Código";
            tablapanelproducto.Columns["Nombre"].HeaderText = "Nombre";
            tablapanelproducto.Columns["PrecioVenta"].HeaderText = "Venta";
            tablapanelproducto.Columns["ITBIS"].HeaderText = "ITBIS";
            tablapanelproducto.Columns["Existencia"].HeaderText = "Existencia";
        }

        private void buscarclientebtn_Click(object sender, EventArgs e)
        {
            panelclientes.Location = new Point(0, 0);
            panelclientes.BringToFront();
            panelclientes.Visible = true;

            string consultaCliente = @"
            SELECT  
                e.IdCliente,
                e.IdPersona,
                p.NombreCompleto AS Nombre,
                pd.Numero AS Cedula,
                tl.Numero AS Telefono,
	            dr.Direccion AS Direccion,
                pd.IdTipoDocumento AS TipoDoc
            FROM Cliente e
            LEFT JOIN Persona p ON e.IdPersona = p.IdPersona
            LEFT JOIN PersonaDocumento pd ON p.IdPersona = pd.IdPersona
            LEFT JOIN PersonaTelefono tl ON p.IdPersona = tl.IdPersona AND tl.EsPrincipal = 1
            LEFT JOIN PersonaDireccion dr ON p.IdPersona = dr.IdPersona AND dr.EsPrincipal = 1
            WHERE e.Activo = 1 AND p.Activo = 1 AND e.IdCliente > 1;"; //Esto es para que no traiga AL CONTADO directamente sino que traiga los demas clientes

            using (SqlDataAdapter adaptador = new SqlDataAdapter(consultaCliente, conexionString))
            {
                DataTable dt = new DataTable();

                adaptador.Fill(dt);

                tablaclientes.DataSource = dt;

                if (tablaclientes.Columns.Contains("TipoDoc"))
                    tablaclientes.Columns["TipoDoc"].Visible = false;

                if (tablaclientes.Columns.Contains("IdPersona"))
                    tablaclientes.Columns["IdPersona"].Visible = false;
            }
        }

        private void bajarproductobtn_Click(object sender, EventArgs e)
        {
            string codigoProducto = txtcodigoproducto.Text;
            string nombreProducto = txtnombreproducto.Text;

            if (txtcodigoproducto.Text == "")
            {
                MessageBox.Show("Agregar producto.");
                return;
            }

            if (!decimal.TryParse(txtprecioproducto.Text, out decimal precio))
            {
                MessageBox.Show("Precio inválido.");
                return;
            }

            if (!decimal.TryParse(txtiva.Text, out decimal itbis))
            {
                MessageBox.Show("ITBIS inválido.");
                return;
            }

            int cantidad = (int)numCantidad.Value;

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(detalleorden);

            decimal sub = precio * cantidad;
            decimal tot = sub + (sub * (itbis / 100));

            row.Cells[0].Value = codigoProducto;
            row.Cells[1].Value = nombreProducto;
            row.Cells[2].Value = precio;
            row.Cells[3].Value = itbis;
            row.Cells[4].Value = cantidad;
            row.Cells[5].Value = tot.ToString("N2");

            detalleorden.Rows.Add(row);

            RecalcularTotales();

            labelcantidadarticulos.Text = detalleorden.Rows.Count.ToString();

            txtcodigoproducto.Clear();
            txtnombreproducto.Clear();
            txtprecioproducto.Clear();
            txtiva.Clear();
            numCantidad.Value = 1;

            guardarpedidobtn.Enabled = true;
            detalleorden.Enabled = true;

        }

        private int ObtenerTotalComidas()
        {
            int totalComidas = 0;

            foreach (DataGridViewRow fila in detalleorden.Rows)
            {
                if (fila.IsNewRow) continue;
                totalComidas += Convert.ToInt32(fila.Cells["cantidad"].Value);
            }

            return totalComidas;
        }

        private void AsignarRepartidorAutomaticamente()
        {
            if (string.IsNullOrWhiteSpace(direccioncliente.Text))
            {
                MessageBox.Show("Debe indicar una dirección.");
                return;
            }

            // 1. Detectar zona desde la dirección
            string zonaDetectada = DetectarZonaDesdeBD(direccioncliente.Text)?.Trim();

            if (!string.IsNullOrWhiteSpace(zonaDetectada))
            {
                int idx = cmbZonaEntrega.FindStringExact(zonaDetectada);
                if (idx >= 0)
                    cmbZonaEntrega.SelectedIndex = idx;
            }

            // 2. Si no pudo detectar, usar la que esté seleccionada manualmente
            if (cmbZonaEntrega.SelectedIndex < 0)
            {
                MessageBox.Show("No se pudo determinar la zona automáticamente. Selecciónela manualmente.");
                return;
            }

            int totalComidas = ObtenerTotalComidas();

            if (totalComidas <= 0)
            {
                MessageBox.Show("Debe agregar productos antes de asignar un repartidor.");
                return;
            }

            string zona = cmbZonaEntrega.Text;

            string query = @"
    SELECT TOP 1
        e.IdEmpleado,
        p.NombreCompleto,
        r.ZonaAsignada,
        r.TipoVehiculo,
        r.RapidezPct,
        r.FamiliaridadPct,
        r.CapacidadMaxima
    FROM Repartidor r
    INNER JOIN Empleado e ON r.IdEmpleado = e.IdEmpleado
    INNER JOIN Persona p ON e.IdPersona = p.IdPersona
    WHERE e.Activo = 1
      AND p.Activo = 1
      AND e.IdRolEmpleado = 6
      AND r.CapacidadMaxima >= @TotalComidas
      AND (
            (@TotalComidas >= 10 AND r.TipoVehiculo IN ('Carro', 'MotorGrande'))
            OR
            (@TotalComidas < 10 AND r.TipoVehiculo IN ('Motor', 'MotorGrande', 'Carro'))
          )
    ORDER BY
        CASE WHEN r.ZonaAsignada = @Zona THEN 0 ELSE 1 END,
        CASE 
            WHEN @TotalComidas >= 10 AND r.TipoVehiculo IN ('Carro', 'MotorGrande') THEN 0
            WHEN @TotalComidas < 10 AND r.TipoVehiculo = 'Motor' THEN 0
            ELSE 1
        END,
        r.RapidezPct DESC,
        r.FamiliaridadPct DESC;";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Zona", zona);
                    cmd.Parameters.AddWithValue("@TotalComidas", totalComidas);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            idrepartidor.Text = dr["IdEmpleado"].ToString();
                            nombrerepartidor.Text = dr["NombreCompleto"].ToString();

                            vehiculoAsignado = dr["TipoVehiculo"]?.ToString() ?? "";
                            txtVehiculoAsignado.Text = string.IsNullOrWhiteSpace(vehiculoAsignado)
                                ? "Sin vehículo"
                                : vehiculoAsignado;

                            if (totalComidas >= 10)
                                motivoAsignacion = "Pedido grande: se asignó un vehículo con mayor capacidad.";
                            else if (dr["ZonaAsignada"].ToString().Trim().Equals(zona, StringComparison.OrdinalIgnoreCase))
                                motivoAsignacion = "Asignado por coincidencia de zona y rapidez.";
                            else
                                motivoAsignacion = "Asignado por disponibilidad y capacidad.";

                            txtMotivoAsignacion.Text = motivoAsignacion;

                            MessageBox.Show("Zona y repartidor asignados automáticamente.");
                        }
                        else
                        {
                            MessageBox.Show("No se encontró un repartidor disponible para esa zona/capacidad.");
                            idrepartidor.Clear();
                            nombrerepartidor.Clear();
                            txtVehiculoAsignado.Clear();
                            txtMotivoAsignacion.Clear();
                            vehiculoAsignado = "";
                            motivoAsignacion = "";
                        }
                    }
                }
            }
        }
        private void limpiarbtn_Click(object sender, EventArgs e)
        {

            PedidoID = 0;
            txtnombrecompleto.Text = "AL CONTADO";
            idclientetxt.Text = "1";
            numerotxt.Clear();
            estimado.Clear();

            txtcodigoproducto.Clear();
            txtnombreproducto.Clear();
            txtprecioproducto.Clear();
            numCantidad.Value = numCantidad.Minimum;
            IdClientePersonaST = "1";
            txtiva.Clear();
            labelsubtotal.Text = "0";
            labeltotal.Text = "0";
            idrepartidor.Clear();
            nombrerepartidor.Clear();
            txtVehiculoAsignado.Clear();
            txtMotivoAsignacion.Clear();
            vehiculoAsignado = "";
            motivoAsignacion = "";

            if (cmbZonaEntrega.Items.Count > 0)
                cmbZonaEntrega.SelectedIndex = 0;

            detalleorden.Rows.Clear();

            totalAcumulado = 0;
            subtotalAcumulado = 0;
            cantidadProd = 0;

            labelcantidadarticulos.Text = "0";

            txtcodigoproducto.Enabled = false;
            txtnombreproducto.Enabled = false;
            txtprecioproducto.Enabled = false;
            numCantidad.Enabled = false;
            guardarpedidobtn.Enabled = false;
            Delivery_Load(sender, e);
        }

        private void guardarpedidobtn_Click(object sender, EventArgs e)
        {
            if (txtnombrecompleto.Text == "AL CONTADO" || txtnombrecompleto.Text == "" || numerotxt.Text == "" || direccioncliente.Text == "")
            {
                MessageBox.Show("Nombre, Dirección y Teléfono deben tener información.");
                return;
            }

            if (idrepartidor.Text == "")
            {
                MessageBox.Show("Debe seleccionar un repartidor.");
                return;
            }

            if (detalleorden.Rows.Count == 0)
            {
                MessageBox.Show("El pedido no tiene productos agregados.");
                return;
            }

            bool commitRealizado = false;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    int idPedidoGenerado = 0;

                    if (EditarEstado == 0)
                    {
                        string queryInsert = @"
                        INSERT INTO Pedido
                        (Fecha, Origen, IdClientePersona, NombreCliente, Estado, EstadoDelivery, Total, Nota, Direccion, IdRepartidor, ZonaEntrega, VehiculoAsignado, MotivoAsignacion)
                         VALUES
                         (@Fecha, @Origen, @IdClientePersona, @NombreCliente, @Estado, @EstadoDelivery, @Total, @Nota, @Direccion, @Repartidor, @ZonaEntrega, @VehiculoAsignado, @MotivoAsignacion);
                         SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdInsert = new SqlCommand(queryInsert, conexion, transaccion);

                        cmdInsert.Parameters.AddWithValue("@Fecha", SistemaFecha.FechaActual);
                        cmdInsert.Parameters.AddWithValue("@Origen", "Delivery");
                        cmdInsert.Parameters.AddWithValue("@IdClientePersona", Convert.ToInt32(IdClientePersonaST));
                        cmdInsert.Parameters.AddWithValue("@NombreCliente", txtnombrecompleto.Text);
                        cmdInsert.Parameters.AddWithValue("@Estado", "Pendiente");
                        cmdInsert.Parameters.AddWithValue("@EstadoDelivery", "Pendiente");
                        cmdInsert.Parameters.AddWithValue("@Total", Convert.ToDecimal(labeltotal.Text));
                        cmdInsert.Parameters.AddWithValue("@Nota", notatxt.Text);
                        cmdInsert.Parameters.AddWithValue("@Direccion", direccioncliente.Text);
                        cmdInsert.Parameters.AddWithValue("@Repartidor", idrepartidor.Text);
                        cmdInsert.Parameters.AddWithValue("@ZonaEntrega", cmbZonaEntrega.Text);
                        cmdInsert.Parameters.AddWithValue("@VehiculoAsignado", txtVehiculoAsignado.Text);
                        cmdInsert.Parameters.AddWithValue("@MotivoAsignacion", txtMotivoAsignacion.Text);

                        idPedidoGenerado = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }

                    else if (EditarEstado == 1)
                    {
                        idPedidoGenerado = PedidoID;
                        string queryUpdate = @"
                        UPDATE Pedido SET
                         Fecha = @Fecha,
                         IdClientePersona = @IdClientePersona,
                         NombreCliente = @NombreCliente,
                         Total = @Total,
                         Nota = @Nota,
                          Direccion = @Direccion,
                         IdRepartidor = @Repartidor,
                        ZonaEntrega = @ZonaEntrega,
                        VehiculoAsignado = @VehiculoAsignado,
                        MotivoAsignacion = @MotivoAsignacion
                        WHERE IdPedido = @IdPedido";
                        SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conexion, transaccion);

                        cmdUpdate.Parameters.AddWithValue("@Fecha", SistemaFecha.FechaActual);
                        cmdUpdate.Parameters.AddWithValue("@IdClientePersona", Convert.ToInt32(IdClientePersonaST));
                        cmdUpdate.Parameters.AddWithValue("@NombreCliente", txtnombrecompleto.Text);
                        cmdUpdate.Parameters.AddWithValue("@Total", Convert.ToDecimal(labeltotal.Text));
                        cmdUpdate.Parameters.AddWithValue("@Nota", notatxt.Text);
                        cmdUpdate.Parameters.AddWithValue("@Direccion", direccioncliente.Text);
                        cmdUpdate.Parameters.AddWithValue("@Repartidor", idrepartidor.Text);
                        cmdUpdate.Parameters.AddWithValue("@IdPedido", PedidoID);
                        cmdUpdate.Parameters.AddWithValue("@ZonaEntrega", cmbZonaEntrega.Text);
                        cmdUpdate.Parameters.AddWithValue("@VehiculoAsignado", txtVehiculoAsignado.Text);
                        cmdUpdate.Parameters.AddWithValue("@MotivoAsignacion", txtMotivoAsignacion.Text);
                        cmdUpdate.ExecuteNonQuery();

                        SqlCommand cmdDel = new SqlCommand(
                            "DELETE FROM DetallePedido WHERE IdPedido = @IdPedido",
                            conexion, transaccion);

                        cmdDel.Parameters.AddWithValue("@IdPedido", PedidoID);
                        cmdDel.ExecuteNonQuery();
                    }

                    string queryDetalle = @"
                    INSERT INTO DetallePedido (IdPedido, IdProducto, Cantidad, PrecioUnitario)
                    VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario);";

                    foreach (DataGridViewRow fila in detalleorden.Rows)
                    {
                        if (fila.IsNewRow) continue;

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conexion, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@IdPedido", idPedidoGenerado);
                        cmdDetalle.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(fila.Cells["codigoProducto"].Value));
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", Convert.ToDecimal(fila.Cells["Cantidad"].Value));
                        cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", Convert.ToDecimal(fila.Cells["Precio"].Value));

                        cmdDetalle.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    commitRealizado = true;

                    if (EditarEstado == 0)
                        MessageBox.Show("Pedido guardado con éxito.");
                    else
                        MessageBox.Show("Pedido actualizado con éxito.");

                    EditarEstado = 0;
                    limpiarbtn_Click(sender, e);
                    tabControl1.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar el pedido: " + ex.Message);

                    if (!commitRealizado)
                    {
                        try { transaccion.Rollback(); } catch { }
                    }
                }
            }
        }

        public int EstadoNota = 0;

        private void nota_Click(object sender, EventArgs e)
        {
            if (EstadoNota == 1)
            {
                notapanel.Visible = false;
                notapanel.Location = new Point(1617, 50);

                EstadoNota = 0;
            }
            else
            {
                notapanel.Visible = true;
                notapanel.Location = new Point(515, 306);

                EstadoNota = 1;
            }
        }

        private void RecalcularTotales()
        {
            decimal subtotal = 0;
            decimal total = 0;

            foreach (DataGridViewRow fila in detalleorden.Rows)
            {
                if (fila.IsNewRow) continue;

                decimal precio = Convert.ToDecimal(fila.Cells["precio"].Value);
                decimal itbis = Convert.ToDecimal(fila.Cells["ITBIS"].Value);
                int cantidad = Convert.ToInt32(fila.Cells["cantidad"].Value);

                decimal sub = precio * cantidad;
                decimal tot = sub + (sub * (itbis / 100));

                subtotal += sub;
                total += tot;
            }

            labelsubtotal.Text = subtotal.ToString("F2");
            labeltotal.Text = total.ToString("F2");
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

        private void repartidorbtn_Click(object sender, EventArgs e)
        {
            panelrepartidor.Location = new Point(0, 0);
            panelrepartidor.BringToFront();
            panelrepartidor.Visible = true;

            string consultaCliente = @"
                SELECT 
                e.IdEmpleado,
                p.NombreCompleto
            FROM Empleado e
            LEFT JOIN Persona p ON e.IdPersona = p.IdPersona
            LEFT JOIN PersonaDocumento pd ON p.IdPersona = pd.IdPersona
            WHERE e.Activo = 1 AND p.Activo = 1 and e.IdRolEmpleado=6;";

            using (SqlDataAdapter adaptador = new SqlDataAdapter(consultaCliente, conexionString))
            {
                DataTable dt = new DataTable();

                adaptador.Fill(dt);

                tabladelivery.DataSource = dt;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panelproducto.Visible = false;
            panelproducto.Location = new Point(3, 499);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panelclientes.Visible = false;
            panelclientes.Location = new Point(803, 532);
        }

        private void tablapanelproducto_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tablapanelproducto.Rows[e.RowIndex];

                txtcodigoproducto.Text = row.Cells["IdProducto"].Value.ToString();
                txtnombreproducto.Text = row.Cells["Nombre"].Value.ToString();
                txtprecioproducto.Text = row.Cells["PrecioVenta"].Value.ToString();
                txtiva.Text = row.Cells["ITBIS"].Value.ToString();
            }

            panelproducto.Visible = false;
            panelproducto.Location = new Point(3, 499);
            numCantidad.Value = numCantidad.Minimum;
        }

        private void tablaclientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                idclientetxt.Clear();
                txtnombrecompleto.Clear();
                rnc.Clear();
                numerotxt.Clear();

                DataGridViewRow row = tablaclientes.Rows[e.RowIndex];
                string idCLiente = row.Cells["IdCliente"].Value.ToString();
                string nombreCompleto = row.Cells["Nombre"].Value.ToString();
                string CedulaCliente = row.Cells["Cedula"].Value.ToString();
                string telefono = row.Cells["Telefono"].Value.ToString();
                string IdClientePersona = row.Cells["IdPersona"].Value.ToString();
                string Direccion = row.Cells["Direccion"].Value.ToString();

                idclientetxt.Text = idCLiente;
                txtnombrecompleto.Text = nombreCompleto;
                rnc.Text = CedulaCliente;
                numerotxt.Text = telefono;
                IdClientePersonaST = IdClientePersona;
                direccioncliente.Text = Direccion;
                AplicarZonaAutomatica();

                int tipoDoc = Convert.ToInt32(tablaclientes.CurrentRow.Cells["TipoDoc"].Value);

                if (tipoDoc == 1)
                {
                    tipodoccmbx.SelectedIndex = 1;
                }
                else if (tipoDoc == 5) //Por alguna razón el RNC está como 5 en la base de datos
                {
                    tipodoccmbx.SelectedIndex = 0;
                }
            }

            panelclientes.Visible = false;
            panelclientes.Location = new Point(803, 532);
        }

        private void tabladelivery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tabladelivery.Rows[e.RowIndex];

                string nombreCompleto = row.Cells["NombreCompleto"].Value.ToString();
                string idRepartidor = row.Cells["IdEmpleado"].Value.ToString();
                nombrerepartidor.Text = nombreCompleto;
                idrepartidor.Text = idRepartidor;
            }

            panelrepartidor.Visible = false;
            panelrepartidor.Location = new Point(803, 532);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panelrepartidor.Visible = false;
            panelrepartidor.Location = new Point(803, 532);
        }

        private void facturarbtn_Click(object sender, EventArgs e)
        {
            if (PedidoID <= 0)
            {
                MessageBox.Show("Seleccione un pedido válido.");
                return;
            }

            if (Total <= 0)
            {
                using (SqlConnection cn = new SqlConnection(conexionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT Total FROM Pedido WHERE IdPedido = @id",
                        cn
                    );
                    cmd.Parameters.AddWithValue("@id", PedidoID);
                    object vTotal = cmd.ExecuteScalar();

                    if (vTotal == null || vTotal == DBNull.Value)
                    {
                        MessageBox.Show("No se pudo obtener el total del pedido.");
                        return;
                    }

                    Total = Convert.ToDecimal(vTotal);
                }
            }

            string estadoPedido = "";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string query = "SELECT Estado FROM Pedido WHERE IdPedido = @id";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", PedidoID);
                    estadoPedido = cmd.ExecuteScalar()?.ToString();
                }
            }

            if (estadoPedido == null)
            {
                MessageBox.Show("El pedido no existe.");
                return;
            }

            if (estadoPedido == "Facturado")
            {
                MessageBox.Show("Este pedido ya está facturado.");
                return;
            }

            if (estadoPedido == "Cancelado")
            {
                MessageBox.Show("Este pedido está cancelado y no puede facturarse.");
                return;
            }

            if (estadoPedido != "Pendiente")
            {
                MessageBox.Show("El pedido debe estar en estado Pendiente.");
                return;
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
            }
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
            RecalcularTotalesPago();
        }

        private void MostrarDevuelta()
        {
            if (TotalAplicado >= TotalPedido)
            {
                devueltatxt.Text = (TotalAplicado - TotalPedido).ToString("N2");
                totalpagar.Text = TotalAPagar.Text;
            }
            else
                devueltatxt.Text = "0.00";
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
                INSERT INTO DetallePago (IdPedido, TipoDetalle, Efectivo, Devuelta, Tarjeta, TarjetaNombre, Transferencia, Banco, Total, Estado, Referencia)
                VALUES (@IdPedido, @TipoDetalle, @Efectivo, @Devuelta, @Tarjeta, @TarjetaNombre, @Transferencia, @Banco, @Total, @Estado, @Referencia)",
                conexion, trans);

                cmd.Parameters.AddWithValue("@IdPedido", PedidoID);
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

                cmd.ExecuteNonQuery();
            }
        }

        private void FacturarPedido()
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                SqlTransaction trans = conexion.BeginTransaction();

                try
                {
                    string queryEstado = "SELECT Estado FROM Pedido WHERE IdPedido = @id";
                    SqlCommand cmdEstado = new SqlCommand(queryEstado, conexion, trans);
                    cmdEstado.Parameters.AddWithValue("@id", PedidoID);

                    string estadoActual = cmdEstado.ExecuteScalar()?.ToString();

                    if (estadoActual != "Pendiente")
                    {
                        trans.Rollback();
                        MessageBox.Show("El pedido no está pendiente.");
                        return;
                    }

                    SqlCommand cmdFacturar = new SqlCommand(
                        "UPDATE Pedido SET Estado='Facturado' WHERE IdPedido=@id",
                        conexion, trans);
                    cmdFacturar.Parameters.AddWithValue("@id", PedidoID);
                    cmdFacturar.ExecuteNonQuery();

                    SqlCommand cmdLiberarComanda = new SqlCommand(
                        "UPDATE Comanda SET Estado='Entregado' WHERE IdPedido=@id",
                        conexion, trans);
                    cmdLiberarComanda.Parameters.AddWithValue("@id", PedidoID);
                    cmdLiberarComanda.ExecuteNonQuery();

                    RegistrarPago(conexion, trans);
                    RebajarInventario(conexion, trans, PedidoID);

                    trans.Commit();

                    MessageBox.Show("Facturacion completada!.");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void RebajarInventario(SqlConnection conexion, SqlTransaction trans, int idPedido)
        {
            string queryDetalle = @"
            SELECT IdProducto, Cantidad 
            FROM DetallePedido
            WHERE IdPedido = @pedido";

            SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conexion, trans);
            cmdDetalle.Parameters.AddWithValue("@pedido", idPedido);

            using (SqlDataReader drDetalle = cmdDetalle.ExecuteReader())
            {
                List<(int idProducto, decimal cantidadVendida)> lista = new();

                while (drDetalle.Read())
                {
                    lista.Add((
                        Convert.ToInt32(drDetalle["IdProducto"]),
                        Convert.ToDecimal(drDetalle["Cantidad"])
                    ));
                }

                drDetalle.Close();

                foreach (var item in lista)
                {
                    int idProd = item.idProducto;
                    decimal cantidadVendida = item.cantidadVendida;

                    string queryIngredientes = @"
                    SELECT IdIngrediente, Cantidad
                    FROM Receta
                    WHERE IdProducto = @prod";

                    SqlCommand cmdIng = new SqlCommand(queryIngredientes, conexion, trans);
                    cmdIng.Parameters.AddWithValue("@prod", idProd);

                    using (SqlDataReader drIng = cmdIng.ExecuteReader())
                    {
                        List<(int idIng, decimal cantidadIng)> ingredientes = new();

                        while (drIng.Read())
                        {
                            ingredientes.Add((
                                Convert.ToInt32(drIng["IdIngrediente"]),
                                Convert.ToDecimal(drIng["Cantidad"])
                            ));
                        }

                        drIng.Close();

                        foreach (var ing in ingredientes)
                        {
                            int idIngrediente = ing.idIng;
                            decimal cantPorUnidad = ing.cantidadIng;

                            decimal rebaja = cantPorUnidad * cantidadVendida;

                            string queryUpdate = @"
                            UPDATE ProductoVenta
                            SET Existencia = Existencia - @rebaja
                            WHERE IdProducto = @idIng";

                            SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conexion, trans);
                            cmdUpdate.Parameters.AddWithValue("@rebaja", rebaja);
                            cmdUpdate.Parameters.AddWithValue("@idIng", idIngrediente);

                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void buscar_Click(object sender, EventArgs e)
        {
            FiltroDatosBusqueda();
        }

        private void FiltroDatosBusqueda()
        {
            using (SqlConnection conectar = new SqlConnection(conexionString))
            {
                try
                {
                    conectar.Open();

                    string estado = "";
                    if (facturadochk.Checked) estado = "Facturado";
                    else if (pendientechk.Checked) estado = "Pendiente";
                    else if (canceladochk.Checked) estado = "Cancelado";

                    string query = @"
                      SELECT 
                      IdPedido,
                      NombreCliente,
                      IdMesa,
                      Total,
                       Estado,
                      EstadoDelivery,
                      Fecha
                     FROM Pedido
                    WHERE
                     (NombreCliente LIKE @buscar
                     OR CAST(IdPedido AS VARCHAR) LIKE @buscar
                     OR CAST(IdMesa AS VARCHAR) LIKE @buscar
                     OR CAST(Total AS VARCHAR) LIKE @buscar)
                     AND Fecha >= @inicio
                     AND Fecha <= @fin
                     AND (@estado = '' OR Estado = @estado)
                     ORDER BY Fecha DESC";

                    using (SqlCommand comando = new SqlCommand(query, conectar))
                    {
                        comando.Parameters.AddWithValue("@buscar", "%" + txtbusquedafactura.Text + "%");
                        comando.Parameters.AddWithValue("@inicio", fecini.Value.Date);
                        comando.Parameters.AddWithValue("@fin", fecfin.Value.Date.AddDays(1).AddSeconds(-1));
                        comando.Parameters.AddWithValue("@estado", estado);

                        SqlDataAdapter da = new SqlDataAdapter(comando);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        tabladatospedidos.DataSource = dt;
                        if (tabladatospedidos.Columns.Contains("EstadoDelivery"))
                            tabladatospedidos.Columns["EstadoDelivery"].HeaderText = "Estado Delivery";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void pagarefectivo_Click(object sender, EventArgs e)
        {
            detallepagopanel.Visible = false;
            detallepagopanel.Location = new Point(1617, 6);

            FacturarPedido();

            if (PedidoID > 0)
            {
                ActualizarEstadoDelivery(PedidoID, "En camino");

                string linkResena = GenerarYGuardarLinkResena(PedidoID);

                if (!string.IsNullOrWhiteSpace(linkResena))
                {
                    Clipboard.SetText(linkResena);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = linkResena,
                        UseShellExecute = true
                    });

                    MessageBox.Show("Link de reseña generado, copiado al portapapeles y abierto en el navegador.");
                }
            }

            MostrarDevuelta();

            devueltapanel.Visible = true;
            devueltapanel.Location = new Point(466, 0);
        }
        private void ActualizarEstadoDelivery(int idPedido, string nuevoEstado)
        {
            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = "UPDATE dbo.Pedido SET EstadoDelivery = @EstadoDelivery WHERE IdPedido = @IdPedido";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@EstadoDelivery", nuevoEstado);
                    cmd.Parameters.AddWithValue("@IdPedido", idPedido);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void volverdetalle_Click(object sender, EventArgs e)
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
                return;
            }

            foreach (DataGridViewRow fila in detallePagoDT.SelectedRows)
            {
                if (!fila.IsNewRow)
                    detallePagoDT.Rows.Remove(fila);
            }

            RecalcularTotalAplicado();
            RecalcularTotalesPago();
        }

        private void finalizarbtn_Click(object sender, EventArgs e)
        {
            volverdetalle_Click(sender, e);
            detallepagopanel.Visible = true;
            detallepagopanel.Location = new Point(476, 0);

            devueltapanel.Visible = false;
            devueltapanel.Location = new Point(0, 0);
            Delivery_Load(sender, e);
        }

        private void tipoComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comprobantes();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                CargarComanda();
            }

            buscar.Focus();
            buscar.PerformClick();
        }

        private void recargarbtn_Click(object sender, EventArgs e)
        {
            CargarComanda();
        }

        private void CargarComanda()
        {
            flowComanda.Controls.Clear();

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string query = @"
                SELECT CM.IdPedido, CM.Cuenta, CM.IdProducto, PV.Nombre, CM.Cantidad, MS.Numero as MesaNumero
                FROM Comanda CM
                INNER JOIN ProductoVenta PV ON CM.IdProducto = PV.IdProducto
                INNER JOIN Mesa MS ON CM.IdMesa = MS.IdMesa
                WHERE CM.Estado = 'Cocina'
                ORDER BY CM.IdPedido, CM.Cuenta";

                SqlCommand cmd = new SqlCommand(query, conexion);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int idPedido = Convert.ToInt32(dr["IdPedido"]);
                        int idProducto = Convert.ToInt32(dr["IdProducto"]);
                        int cuenta = Convert.ToInt32(dr["Cuenta"]);
                        string nombre = dr["Nombre"].ToString();
                        int cantidad = Convert.ToInt32(dr["Cantidad"]);
                        int numero = Convert.ToInt32(dr["MesaNumero"]);

                        Image img = CargarImagen(Convert.ToInt32(dr["IdProducto"]));

                        Panel card = BotonComanda(idPedido, cuenta, nombre, numero, cantidad, img, idProducto);

                        flowComanda.Controls.Add(card);
                    }
                }
            }
        }

        private void BotonComanda_Click(object sender, EventArgs e)
        {
            if (!modoEntregar)
                return;

            Panel card = ObtenerPanelPadre(sender);
            if (card == null) return;

            if (ordenesSeleccionadas.Contains(card))
            {
                ordenesSeleccionadas.Remove(card);
                card.BackColor = Color.White;
            }
            else
            {
                ordenesSeleccionadas.Add(card);
                card.BackColor = Color.LightBlue;
            }
        }

        private Panel ObtenerPanelPadre(object sender)
        {
            if (sender is Panel p) return p;
            if (sender is Control c && c.Parent is Panel p2) return p2;
            return null;
        }

        private void LimpiarSeleccionVisual()
        {
            foreach (Control ctrl in flowComanda.Controls)
            {
                if (ctrl is Panel panel)
                    panel.BackColor = Color.White;
            }
        }

        private Image CargarImagen(int idProducto)
        {
            string basePath = @"C:\SistemaArchivos\Productos\";
            string[] extensiones = { ".jpg", ".png", ".jpeg" };

            foreach (string ext in extensiones)
            {
                string ruta = Path.Combine(basePath, idProducto + ext);
                if (File.Exists(ruta))
                {
                    try
                    {
                        using (var temp = new Bitmap(ruta))
                        {
                            return new Bitmap(temp);
                        }
                    }
                    catch { }
                }
            }

            return null;
        }

        private void SiEntrega_Click(object sender, EventArgs e)
        {
            if (ordenesSeleccionadas.Count == 0)
            {
                MessageBox.Show("No hay órdenes seleccionadas.");
                return;
            }

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();

                foreach (var btn in ordenesSeleccionadas)
                {
                    dynamic info = btn.Tag;

                    SqlCommand cmd = new SqlCommand(
                    @"UPDATE Comanda 
                    SET Estado = 'Entregado'
                    WHERE IdPedido = @p AND Cuenta = @c AND IdProducto = @prod",
                    cn);

                    cmd.Parameters.AddWithValue("@p", info.Pedido);
                    cmd.Parameters.AddWithValue("@c", info.Cuenta);
                    cmd.Parameters.AddWithValue("@prod", info.IdProducto);

                    cmd.ExecuteNonQuery();
                }

                EntregarOrden.BackColor = Color.FromArgb(224, 224, 224);
            }

            MessageBox.Show("Órdenes marcadas como entregadas.");
            SiEntrega.Visible = false;
            NoEntrega.Visible = false;
            CargarComanda();
        }

        private void NoEntrega_Click(object sender, EventArgs e)
        {
            modoEntregar = false;
            ordenesSeleccionadas.Clear();
            LimpiarSeleccionVisual();
            SiEntrega.Visible = false;
            NoEntrega.Visible = false;

            EntregarOrden.BackColor = Color.FromArgb(224, 224, 224);
        }

        private Panel BotonComanda(int idPedido, int cuenta, string nombre, int numero, int cantidad, Image imagen, int idProducto)
        {
            Panel card = new Panel();
            card.Width = 200;
            card.Height = 260;
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Margin = new Padding(15);
            card.Cursor = Cursors.Hand;

            card.Tag = new
            {
                Pedido = idPedido,
                Cuenta = cuenta,
                Producto = nombre,
                Mesa = numero,
                Cantidad = cantidad,
                IdProducto = idProducto
            };

            PictureBox pic = new PictureBox();
            pic.Width = 190;
            pic.Height = 120;
            pic.Top = 5;
            pic.Left = 5;
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.Image = imagen ?? Properties.Resources.paisaje;

            Label lbl = new Label();
            lbl.Width = 190;
            lbl.Height = 120;
            lbl.Left = 5;
            lbl.Top = 130;
            lbl.TextAlign = ContentAlignment.TopLeft;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            lbl.Text =
                $"ORDEN: {idPedido}, " +
                $"MESA: {numero}\n" +
                $"CUENTA: {cuenta}\n\n" +
                $"{nombre.ToUpper()}\n" +
                $"CANTIDAD: {cantidad}";

            card.Click += BotonComanda_Click;
            pic.Click += BotonComanda_Click;
            lbl.Click += BotonComanda_Click;

            card.Controls.Add(pic);
            card.Controls.Add(lbl);

            return card;
        }

        private void EntregarOrden_Click(object sender, EventArgs e)
        {
            modoEntregar = true;
            EntregarOrden.BackColor = Color.Gold;
            SiEntrega.Visible = true;
            NoEntrega.Visible = true;
        }

        private void detalleorden_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = detalleorden.Rows[e.RowIndex];

            txtcodigoproducto.Text = row.Cells["codigoProducto"].Value.ToString();
            txtnombreproducto.Text = row.Cells["nombreProducto"].Value.ToString();

            txtprecioproducto.Text = Convert.ToDecimal(row.Cells["precio"].Value).ToString("N2");
            txtiva.Text = Convert.ToDecimal(row.Cells["ITBIS"].Value).ToString("N2");

            numCantidad.Value = Convert.ToDecimal(row.Cells["cantidad"].Value);

            detalleorden.Rows.RemoveAt(e.RowIndex);

            RecalcularTotales();

            bajarproductobtn.Enabled = true;
        }

        private void numCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                bajarproductobtn_Click(sender, e);
                e.Handled = true;
            }
        }

        private void rnc_TextChanged(object sender, EventArgs e)
        {
            if (tipodoccmbx.SelectedIndex == 0)
            {
                string posicion = rnc.Text; posicion = posicion.Replace("-", "");
                if (posicion.Length > 11)
                {
                    posicion = posicion.Substring(0, 11);
                }
                if (posicion.Length > 3)
                {
                    posicion = posicion.Insert(3, "-");
                }

                rnc.Text = posicion; rnc.SelectionStart = rnc.Text.Length;
            }
            else if (tipodoccmbx.SelectedIndex == 1)
            {
                string posicion = rnc.Text; posicion = posicion.Replace("-", "");

                if (posicion.Length > 11)
                {
                    posicion = posicion.Substring(0, 11);
                }

                if (posicion.Length > 3)
                {
                    posicion = posicion.Insert(3, "-");
                }
                if (posicion.Length > 11)
                {
                    posicion = posicion.Insert(11, "-");
                }

                rnc.Text = posicion; rnc.SelectionStart = rnc.Text.Length;
            }
        }

        private void rnc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BuscarRNC();
            }
        }

        private void BuscarRNC()
        {
            string rncDigitado = rnc.Text.Replace("-", "").Trim();

            if (rncDigitado.Length == 0)
                return;

            bool encontrado = BuscarEnArchivoDGII(rncDigitado);

            if (!encontrado)
            {
                BuscarRNCenSQL(rncDigitado);
            }
        }

        private bool BuscarEnArchivoDGII(string rnc)
        {
            string archivoDGII = @"C:\SistemaArchivos\DGIITXT\DGII_RNC.TXT";

            if (!File.Exists(archivoDGII))
                return false;

            try
            {
                using (StreamReader sr = new StreamReader(archivoDGII))
                {
                    string linea;

                    while ((linea = sr.ReadLine()) != null)
                    {
                        if (linea.StartsWith(rnc + "|"))
                        {
                            string[] partes = linea.Split('|');

                            if (partes.Length < 3)
                                return false;

                            string nombre = partes[1].Trim();
                            string estado = partes[partes.Length - 2].Trim().ToUpper();

                            if (estado == "ACTIVO")
                            {
                                txtnombrecompleto.Text = nombre;
                                numerotxt.Text = "";
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private void BuscarRNCenSQL(string rnc)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();

                    string query = @"
                    SELECT TOP 1 
                            p.IdPersona,
                            p.Nombre,
                            p.Apellido,
                            (p.Nombre + ' ' + p.Apellido) AS NombreCompleto,
                            t.Numero AS TelefonoPrincipal
                        FROM Persona p
                        INNER JOIN PersonaDocumento d ON p.IdPersona = d.IdPersona
                        OUTER APPLY (
                            SELECT TOP 1 Numero 
                            FROM PersonaTelefono 
                            WHERE IdPersona = p.IdPersona AND EsPrincipal = 1
                        ) t
                        WHERE REPLACE(d.Numero, '-', '') = @RNC
                          AND d.IdTipoDocumento = 1;";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@RNC", rnc);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                txtnombrecompleto.Text = dr["NombreCompleto"].ToString();
                                numerotxt.Text = dr["TelefonoPrincipal"]?.ToString() ?? "";

                                return;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void imprimirbtn_Click(object sender, EventArgs e)
        {
            if (PedidoID > 0)
            {
                try
                {
                    GenerarFacturaPDF(PedidoID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al generar el PDF: {ex.Message}\nDetalles: {ex.StackTrace}");
                }
            }
            else
            {
                MessageBox.Show("Seleccione una factura válida para imprimir.");
            }
        }

        private void GenerarFacturaPDF(int idPedido)
        {
            try
            {
                string folderPath = @"C:\SistemaArchivos\Facturas\";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, $"Pedido_{idPedido}.pdf");

                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                document.Info.Title = $"Pedido {idPedido}";

                PdfSharp.Pdf.PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Segoe UI", 20, XFontStyleEx.Bold);
                XFont headerFont = new XFont("Segoe UI", 12, XFontStyleEx.Bold);
                XFont textFont = new XFont("Segoe UI", 12, XFontStyleEx.Regular);
                XFont smallFont = new XFont("Segoe UI", 10, XFontStyleEx.Regular);

                double currentY = 40;
                double marginLeft = 30;
                double lineHeight = 20;

                gfx.DrawString("ORDEN", titleFont, XBrushes.Black, new XRect(0, currentY, page.Width, 40), XStringFormats.TopCenter);

                currentY += 50;

                string nombreCliente = "", fecha = "", mesa = "", comprobante = "";
                decimal totalFactura = 0;

                using (SqlConnection con = new SqlConnection(conexionString))
                {
                    con.Open();

                    string sqlPedido = @"
                SELECT IdPedido, Fecha, NombreCliente, IdMesa, Total, Comprobante
                FROM Pedido
                WHERE IdPedido = @id";

                    SqlCommand cmd = new SqlCommand(sqlPedido, con);
                    cmd.Parameters.AddWithValue("@id", idPedido);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            fecha = Convert.ToDateTime(dr["Fecha"]).ToString("dd/MM/yyyy HH:mm");
                            nombreCliente = dr["NombreCliente"].ToString();
                            mesa = dr["IdMesa"].ToString();
                            comprobante = dr["Comprobante"]?.ToString() ?? "";
                            totalFactura = Convert.ToDecimal(dr["Total"]);
                        }
                    }
                }

                gfx.DrawString($"Pedido: {idPedido}", headerFont, XBrushes.Black, marginLeft, currentY); currentY += lineHeight;
                gfx.DrawString($"Cliente: {nombreCliente}", textFont, XBrushes.Black, marginLeft, currentY); currentY += lineHeight;
                gfx.DrawString($"Mesa: {mesa}", textFont, XBrushes.Black, marginLeft, currentY); currentY += lineHeight;
                gfx.DrawString($"Fecha: {fecha}", textFont, XBrushes.Black, marginLeft, currentY); currentY += lineHeight;

                if (!string.IsNullOrWhiteSpace(comprobante))
                {
                    gfx.DrawString($"Comprobante: {comprobante}", textFont, XBrushes.Black, marginLeft, currentY);
                    currentY += lineHeight;
                }

                currentY += 10;
                gfx.DrawLine(XPens.Black, marginLeft, currentY, page.Width - marginLeft, currentY);
                currentY += 20;

                gfx.DrawString("DETALLE", headerFont, XBrushes.Black, marginLeft, currentY);
                currentY += lineHeight;

                gfx.DrawString("Producto", headerFont, XBrushes.Black, marginLeft, currentY);
                gfx.DrawString("Cant.", headerFont, XBrushes.Black, marginLeft + 250, currentY);
                gfx.DrawString("Precio", headerFont, XBrushes.Black, marginLeft + 320, currentY);
                gfx.DrawString("Subtotal", headerFont, XBrushes.Black, marginLeft + 400, currentY);

                currentY += lineHeight;

                gfx.DrawLine(XPens.Black, marginLeft, currentY, page.Width - marginLeft, currentY);
                currentY += 10;


                using (SqlConnection con = new SqlConnection(conexionString))
                {
                    con.Open();

                    string sqlDetalle = @"
                SELECT d.Cantidad, d.PrecioUnitario, 
                       (d.Cantidad * d.PrecioUnitario) AS Subtotal,
                       p.Nombre
                FROM DetallePedido d
                INNER JOIN ProductoVenta p ON p.IdProducto = d.IdProducto
                WHERE d.IdPedido = @id";

                    SqlCommand cmd = new SqlCommand(sqlDetalle, con);
                    cmd.Parameters.AddWithValue("@id", idPedido);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string prod = dr["Nombre"].ToString();
                            decimal cant = Convert.ToDecimal(dr["Cantidad"]);
                            decimal precio = Convert.ToDecimal(dr["PrecioUnitario"]);
                            decimal sub = Convert.ToDecimal(dr["Subtotal"]);

                            gfx.DrawString(prod, textFont, XBrushes.Black, marginLeft, currentY);
                            gfx.DrawString(cant.ToString(), textFont, XBrushes.Black, marginLeft + 250, currentY);
                            gfx.DrawString(precio.ToString("N2"), textFont, XBrushes.Black, marginLeft + 320, currentY);
                            gfx.DrawString(sub.ToString("N2"), textFont, XBrushes.Black, marginLeft + 400, currentY);

                            currentY += lineHeight;
                        }
                    }
                }

                currentY += 10;
                gfx.DrawLine(XPens.Black, marginLeft, currentY, page.Width - marginLeft, currentY);
                currentY += 20;

                gfx.DrawString($"TOTAL: RD$ {totalFactura:N2}", titleFont, XBrushes.Black, marginLeft + 250, currentY);
                currentY += lineHeight * 2;

                gfx.DrawString("Gracias por su compra", smallFont, XBrushes.Black,
                    new XRect(marginLeft, currentY, page.Width, 20), XStringFormats.TopLeft);

                document.Save(filePath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });

                MessageBox.Show("Factura generada correctamente.", "Éxito");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message);
            }
        }

        private void tabladatospedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tabladatospedidos.Rows[e.RowIndex];

                PedidoID = Convert.ToInt32(row.Cells["IdPedido"].Value);
            }
        }

        public int sistemas = 0;

        private void deslizar_Click(object sender, EventArgs e)
        {
            if (sistemas == 0)
            {
                deslizar.Image = Proyecto_restaurante.Properties.Resources.flechaderecharoja;
                opcionesCarpeta.Visible = true;
                sistemas = 1;
            }
            else
            {
                deslizar.Image = Proyecto_restaurante.Properties.Resources.flechaizquierdaroja;
                opcionesCarpeta.Visible = false;
                sistemas = 0;
            }
        }

        private string rutaFacturas = @"C:\SistemaArchivos\Facturas";

        private void carpetaFactura_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(rutaFacturas))
                    Directory.CreateDirectory(rutaFacturas);

                System.Diagnostics.Process.Start("explorer.exe", rutaFacturas);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la carpeta: " + ex.Message);
            }
        }

        private void eliminarFacturas_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(rutaFacturas))
                {
                    MessageBox.Show("La carpeta no existe.");
                    return;
                }

                var archivos = Directory.GetFiles(rutaFacturas, "*.pdf");

                if (archivos.Length == 0)
                {
                    MessageBox.Show("No hay facturas PDF para eliminar.");
                    return;
                }

                DialogResult r = MessageBox.Show(
                    $"Se eliminarán {archivos.Length} archivos PDF.\n¿Desea continuar?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );

                if (r == DialogResult.No)
                    return;

                foreach (var archivo in archivos)
                    File.Delete(archivo);

                MessageBox.Show("Todas las facturas han sido eliminadas.");
                deslizar_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar facturas: " + ex.Message);
            }
        }
        private string ConstruirUrlResena(string token)
        {
            return $"http://localhost/PanelCentral3/PanelCentral/php/resena_delivery.php?token={token}";
        }

        private string GenerarTokenResena()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
        }

        private string GenerarYGuardarLinkResena(int idPedido)
        {
            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                // Si ya existe un link activo para ese pedido, lo reutiliza
                string sqlExiste = @"
        SELECT TOP 1 Token
        FROM dbo.LinkResenaDelivery
        WHERE IdPedido = @IdPedido
          AND Usado = 0
          AND Expirado = 0
        ORDER BY IdLinkResena DESC";

                using (SqlCommand cmdExiste = new SqlCommand(sqlExiste, con))
                {
                    cmdExiste.Parameters.AddWithValue("@IdPedido", idPedido);

                    object tokenExistente = cmdExiste.ExecuteScalar();

                    if (tokenExistente != null && tokenExistente != DBNull.Value)
                    {
                        return ConstruirUrlResena(tokenExistente.ToString());
                    }
                }

                // Si no existe, crea uno nuevo
                string tokenNuevo = GenerarTokenResena();

                string sqlInsert = @"
        INSERT INTO dbo.LinkResenaDelivery (IdPedido, Token, TelefonoDestino, Expirado, Usado)
        VALUES (@IdPedido, @Token, @TelefonoDestino, 0, 0)";

                using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, con))
                {
                    cmdInsert.Parameters.AddWithValue("@IdPedido", idPedido);
                    cmdInsert.Parameters.AddWithValue("@Token", tokenNuevo);
                    cmdInsert.Parameters.AddWithValue("@TelefonoDestino", numerotxt.Text.Trim());

                    cmdInsert.ExecuteNonQuery();
                }

                return ConstruirUrlResena(tokenNuevo);
            }
        }

        private string DetectarZonaDesdeBD(string direccion)
        {
            if (string.IsNullOrWhiteSpace(direccion))
                return "";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
        SELECT TOP 1 Zona
        FROM dbo.SectorZona
        WHERE UPPER(@Direccion) LIKE '%' + UPPER(Sector) + '%'
        ORDER BY LEN(Sector) DESC;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Direccion", direccion.Trim());

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                        return resultado.ToString().Trim();
                }
            }

            return "";
        }

        private void AplicarZonaAutomatica()
        {
            string zonaDetectada = DetectarZonaDesdeBD(direccioncliente.Text)?.Trim();

            if (string.IsNullOrWhiteSpace(zonaDetectada))
                return;

            this.BeginInvoke(new Action(() =>
            {
                int idx = cmbZonaEntrega.FindStringExact(zonaDetectada);

                if (idx >= 0)
                {
                    cmbZonaEntrega.SelectedIndex = idx;
                }
            }));
        }
        private void direccioncliente_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(direccioncliente.Text))
                AplicarZonaAutomatica();
        }


        private void CargarResumenResenas()
        {
            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
        SELECT 
            ISNULL(CAST(AVG(CAST(Puntuacion AS DECIMAL(10,2))) AS DECIMAL(10,2)), 0) AS Promedio,
            COUNT(*) AS Cantidad
        FROM dbo.ResenaDelivery";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtPromedioResena.Text = Convert.ToDecimal(dr["Promedio"]).ToString("0.00");
                        txtCantidadResenas.Text = dr["Cantidad"].ToString();
                    }
                }
            }
        }

        private void CargarUltimasResenas()
        {
            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
        SELECT TOP 10
            r.IdPedido,
            p.NombreCliente,
            r.Puntuacion,
            r.Comentario,
            r.FechaResena
        FROM dbo.ResenaDelivery r
        INNER JOIN dbo.Pedido p ON r.IdPedido = p.IdPedido
        ORDER BY r.FechaResena DESC;";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUltimasResenas.DataSource = dt;
                }
            }

            if (dgvUltimasResenas.Columns.Contains("IdPedido"))
                dgvUltimasResenas.Columns["IdPedido"].HeaderText = "Pedido";

            if (dgvUltimasResenas.Columns.Contains("NombreCliente"))
                dgvUltimasResenas.Columns["NombreCliente"].HeaderText = "Cliente";

            if (dgvUltimasResenas.Columns.Contains("Puntuacion"))
                dgvUltimasResenas.Columns["Puntuacion"].HeaderText = "Puntuación";

            if (dgvUltimasResenas.Columns.Contains("Comentario"))
                dgvUltimasResenas.Columns["Comentario"].HeaderText = "Comentario";

            if (dgvUltimasResenas.Columns.Contains("FechaResena"))
                dgvUltimasResenas.Columns["FechaResena"].HeaderText = "Fecha";
        }
        private void label18_Click(object sender, EventArgs e)
        {

        }


        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AsignarRepartidorAutomaticamente();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click_1(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label23_Click_2(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            panelResenas.Visible = false;
        }

        private void label23_Click_3(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnVerResenas_Click(object sender, EventArgs e)
        {
            CargarUltimasResenas();
            panelResenas.Visible = true;
            panelResenas.BringToFront();
        }
    }
}