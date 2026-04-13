using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
    public partial class Pedidos : Form
    {
        public Pedidos()
        {
            InitializeComponent();
        }

        private int cantidadProd = 0;
        private string pedidoActual;
        public string NombrePC;
        public string NombreUsuario;
        public int IdUsuario = 0;
        private int Autorizar = 0;

        private int IDMesa = 0;
        private int idMesaSeleccionada = 0;
        private int NumeroMesa = 0;
        private int OcupadoMesa = 0;
        private int ReservadoMesa = 0;
        private int EditarEstado = 0;
        private int CuentaSeparada = 0;
        private int ModoElminar = 0;
        private int OrdenGrupo = 0;
        public int EliminarFila = 0;
        private int PedidoID;
        private decimal Total;
        private string IdClientePersonaST = "1"; //Al contado por defecto

        decimal TotalPedido = 0m;
        decimal TotalAplicado = 0m;
        decimal TotalRestante = 0;

        private decimal totalAcumulado = 0;
        private decimal subtotalAcumulado = 0;
        public string comprobanteFinal;
        bool cargandoOrden = false;
        bool cargandoGrupos = false;

        bool CambiarPrecio = false;
        bool PrecioMinimo = false;

        bool permisoSi = false;
        private int gruposMinimos = 1;

        bool modoUnion = false;
        bool modoSeparar = false;

        bool modoEntregar = false;

        private bool facturarDesdeMesa = false;

        List<int> mesasDelGrupo = new List<int>();

        List<int> mesasSeleccionadasUnion = new List<int>();

        private List<Panel> ordenesSeleccionadas = new List<Panel>();

        private List<CuentaItem> listaGrupos = new List<CuentaItem>();

        string conexionString = ConexionBD.ConexionSQL();

        private void buscarclientebtn_Click(object sender, EventArgs e)
        {
            panelclientes.Location = new Point(0, 0);
            panelclientes.Visible = true;

            string consultaCliente = @"
            SELECT 
                e.IdCliente,
                e.IdPersona,
                p.NombreCompleto AS Nombre,
                pd.Numero AS Cedula,
                tl.Numero AS Telefono,
                pd.IdTipoDocumento AS TipoDoc
            FROM Cliente e
            LEFT JOIN Persona p ON e.IdPersona = p.IdPersona
            LEFT JOIN PersonaDocumento pd ON p.IdPersona = pd.IdPersona
            LEFT JOIN PersonaTelefono tl ON p.IdPersona = tl.IdPersona AND tl.EsPrincipal = 1
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

        private void Cerrar()
        {
            DialogResult salir = MessageBox.Show("¿Desea Salir?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (salir == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panelclientes.Visible = false;
            panelclientes.Location = new Point(803, 532);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panelproducto.Visible = false;
            panelproducto.Location = new Point(3, 499);
        }

        private void buscarproductobtn_Click(object sender, EventArgs e)
        {
            panelproducto.Location = new Point(0, 0);
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
            WHERE pv.Activo = 1 AND pt.Ingrediente = 0 OR pt.Bebida = 1;";

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

        private void Pedidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0 && e.KeyCode == Keys.F1)
            {
                CrearOrden_Click(null, null);
            }

            if (tabControl1.SelectedIndex == 0 && e.KeyCode == Keys.F2)
            {
                EditarOrden_Click(null, null);
            }

            if (tabControl1.SelectedIndex == 0 && e.KeyCode == Keys.F3)
            {
                FacturarOrden_Click(null, null);
            }

            if (tabControl1.SelectedIndex == 2 && e.KeyCode == Keys.F3)
            {
                facturarbtn.PerformClick();
            }

            if (tabControl1.SelectedIndex == 1 && e.Control && e.KeyCode == Keys.Space)
            {
                separarcuenta.PerformClick();
            }

            if (tabControl1.SelectedIndex == 1 && e.KeyCode == Keys.Delete)
            {
                eliminarFila.PerformClick();
            }

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

            if (panelCuentaSeparada.Enabled == true && ModoElminar == 0 && tabControl1.SelectedIndex == 1 && e.Alt && e.KeyCode == Keys.Q)
            {
                nuevoGrupo.Image = Proyecto_restaurante.Properties.Resources.menos_pequeno;
                nuevoGrupo.BackColor = Color.LightCoral;
                ModoElminar = 1;
            }
            else if (panelCuentaSeparada.Enabled == true && ModoElminar == 1 && tabControl1.SelectedIndex == 1 && e.Alt && e.KeyCode == Keys.Q)
            {
                nuevoGrupo.Image = Proyecto_restaurante.Properties.Resources.mas;
                nuevoGrupo.BackColor = SystemColors.ActiveCaption;
                ModoElminar = 0;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Cerrar();
            }
        }

        private void guardarordenbtn_Click(object sender, EventArgs e)
        {
            bool commitRealizado = false;
            int idPedidoGenerado = 0;

            if (EditarEstado == 0)
            {
                comprobanteFinal = GenerarComprobante();
            }

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    if (EditarEstado == 0)
                    {
                        string queryPedido = @"
                        INSERT INTO Pedido 
                        (Fecha, IdMesa, Origen, IdClientePersona, NombreCliente, Estado, Total, Nota, Comprobante, CreadoPor)
                        VALUES 
                        (@Fecha, @IdMesa, @Origen, @IdClientePersona, @NombreCliente, @Estado, @Total, @Nota, @Comprobante, @Usuario);
                        SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdPedido = new SqlCommand(queryPedido, conexion, transaccion);

                        cmdPedido.Parameters.AddWithValue("@Fecha", SistemaFecha.FechaActual);
                        cmdPedido.Parameters.AddWithValue("@IdMesa", idMesaSeleccionada);
                        cmdPedido.Parameters.AddWithValue("@Origen", "Local");
                        cmdPedido.Parameters.AddWithValue("@IdClientePersona", Convert.ToInt32(IdClientePersonaST));
                        cmdPedido.Parameters.AddWithValue("@NombreCliente", txtnombrecompleto.Text);
                        cmdPedido.Parameters.AddWithValue("@Estado", "Pendiente");
                        cmdPedido.Parameters.AddWithValue("@Total", Convert.ToDecimal(labeltotal.Text));
                        cmdPedido.Parameters.AddWithValue("@Nota", notatxt.Text);
                        cmdPedido.Parameters.AddWithValue("@Comprobante", comprobanteFinal);
                        cmdPedido.Parameters.AddWithValue("@Usuario", IdUsuario);

                        idPedidoGenerado = Convert.ToInt32(cmdPedido.ExecuteScalar());

                        ActualizarSecuencia(tipoComp.SelectedIndex == 0 ? 1 : 2);
                    }
                    else if (EditarEstado == 1)
                    {
                        idPedidoGenerado = PedidoID;

                        string queryUpdatePedido = @"
                        UPDATE Pedido
                        SET Fecha = @Fecha,
                            IdMesa = @IdMesa,
                            IdClientePersona = @IdClientePersona,
                            NombreCliente = @NombreCliente,
                            Total = @Total,
                            Nota = @Nota
                        WHERE IdPedido = @IdPedido";

                        SqlCommand cmdUpdate = new SqlCommand(queryUpdatePedido, conexion, transaccion);

                        cmdUpdate.Parameters.AddWithValue("@Fecha", SistemaFecha.FechaActual);
                        cmdUpdate.Parameters.AddWithValue("@IdMesa", idMesaSeleccionada);
                        cmdUpdate.Parameters.AddWithValue("@IdClientePersona", Convert.ToInt32(IdClientePersonaST));
                        cmdUpdate.Parameters.AddWithValue("@NombreCliente", txtnombrecompleto.Text);
                        cmdUpdate.Parameters.AddWithValue("@Total", Convert.ToDecimal(labeltotal.Text));
                        cmdUpdate.Parameters.AddWithValue("@Nota", notatxt.Text);
                        cmdUpdate.Parameters.AddWithValue("@IdPedido", PedidoID);
                        cmdUpdate.ExecuteNonQuery();

                        SqlCommand cmdDeleteDetalle = new SqlCommand(
                            "DELETE FROM DetallePedido WHERE IdPedido = @IdPedido",
                            conexion, transaccion);
                        cmdDeleteDetalle.Parameters.AddWithValue("@IdPedido", PedidoID);
                        cmdDeleteDetalle.ExecuteNonQuery();

                        SqlCommand cmdDeleteComanda = new SqlCommand(
                            "DELETE FROM Comanda WHERE IdPedido = @IdPedido AND Estado = 'Cocina'",
                            conexion, transaccion);
                        cmdDeleteComanda.Parameters.AddWithValue("@IdPedido", PedidoID);
                        cmdDeleteComanda.ExecuteNonQuery();
                    }

                    string queryDetalle = @"
                    INSERT INTO DetallePedido (IdPedido, IdProducto, Cantidad, PrecioUnitario, Cuenta)
                    VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario, @Cuenta);";

                    foreach (DataGridViewRow fila in detalleorden.Rows)
                    {
                        if (fila.IsNewRow) continue;

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conexion, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@IdPedido", idPedidoGenerado);
                        cmdDetalle.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(fila.Cells["codigoProducto"].Value));
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", Convert.ToDecimal(fila.Cells["Cantidad"].Value));
                        cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", Convert.ToDecimal(fila.Cells["Precio"].Value));
                        cmdDetalle.Parameters.AddWithValue("@Cuenta", Convert.ToDecimal(fila.Cells["cuenta"].Value));

                        cmdDetalle.ExecuteNonQuery();
                    }

                    string queryInsertarComanda = @"
                    INSERT INTO Comanda (IdPedido, IdMesa, IdProducto, Cantidad, Estado, Cuenta)
                    VALUES (@IdPedido, @IdMesa, @IdProducto, @Cantidad, @Estado, @Cuenta);";

                    foreach (DataGridViewRow fila in detalleorden.Rows)
                    {
                        if (fila.IsNewRow) continue;

                        SqlCommand cmdComanda = new SqlCommand(queryInsertarComanda, conexion, transaccion);

                        cmdComanda.Parameters.AddWithValue("@IdPedido", idPedidoGenerado);
                        cmdComanda.Parameters.AddWithValue("@IdMesa", idMesaSeleccionada);
                        cmdComanda.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(fila.Cells["codigoProducto"].Value));
                        cmdComanda.Parameters.AddWithValue("@Cantidad", Convert.ToDecimal(fila.Cells["Cantidad"].Value));
                        cmdComanda.Parameters.AddWithValue("@Cuenta", Convert.ToDecimal(fila.Cells["cuenta"].Value));
                        cmdComanda.Parameters.AddWithValue("@Estado", "Cocina");

                        cmdComanda.ExecuteNonQuery();
                    }

                    string queryMesa = "UPDATE Mesa SET Ocupado = 1 WHERE IdMesa = @IdMesa";

                    foreach (int mesa in mesasDelGrupo)
                    {
                        SqlCommand cmdMesa = new SqlCommand(queryMesa, conexion, transaccion);
                        cmdMesa.Parameters.AddWithValue("@IdMesa", mesa);
                        cmdMesa.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    commitRealizado = true;

                    if (EditarEstado == 0)
                        MessageBox.Show("Orden creada con éxito.");
                    else
                        MessageBox.Show("Orden actualizada con éxito.");

                    EditarEstado = 0;
                    limpiarbtn_Click(sender, e);
                    tabControl1.SelectedIndex = 0;
                    Pedidos_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar la orden: " + ex.Message);

                    if (!commitRealizado)
                    {
                        try { transaccion.Rollback(); } catch { }
                    }
                }
            }
        }

        private string GenerarComprobante()
        {
            string prefijo = "";

            if (tipoComp.SelectedIndex == 0)
                prefijo = "B01";
            else if (tipoComp.SelectedIndex == 1)
                prefijo = "B02";

            string secuencia = Comprobantetxt.Text;

            secuencia = int.Parse(secuencia).ToString("00000000");

            return prefijo + secuencia;
        }

        private void ActualizarSecuencia(int tipo)
        {
            string sql = @"
            UPDATE Comprobantes
            SET SecuenciaActual = SecuenciaActual + 1
            WHERE Tipo = @Tipo";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void habilitarbotones()
        {
            buscarclientebtn.Enabled = true;
            buscarproductobtn.Enabled = true;
            buscarproductobtn.Enabled = true;
            bajarproductobtn.Enabled = true;
            numCantidad.Enabled = true;
            nota.Enabled = true;
            guardarordenbtn.Enabled = true;
            idclientetxt.Enabled = true;
            txtnombrecompleto.Enabled = true;
            numerotxt.Enabled = true;
            tipodoccmbx.Enabled = true;
            detalleorden.Enabled = true;

            if (grupoCuenta.Items.Count > 0)
            {
                separarcuenta.BackColor = Color.Gold;
                separarcuenta.Enabled = false;
            }
            else
            {
                separarcuenta.Enabled = true;
            }

            eliminarFila.Enabled = true;
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

                idclientetxt.Text = idCLiente;
                txtnombrecompleto.Text = nombreCompleto;
                rnc.Text = CedulaCliente;
                numerotxt.Text = telefono;
                IdClientePersonaST = IdClientePersona;

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

            habilitarbotones();
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

            if (grupoCuenta.Items.Count > 0 || CuentaSeparada == 1)
            {
                int grupoNumero = Convert.ToInt32(grupoCuenta.SelectedValue);
                decimal sub = precio * cantidad;
                decimal tot = sub + (sub * (itbis / 100));

                row.Cells[0].Value = grupoNumero;
                row.Cells[1].Value = codigoProducto;
                row.Cells[2].Value = nombreProducto;
                row.Cells[3].Value = precio;
                row.Cells[4].Value = itbis;
                row.Cells[5].Value = cantidad;
                row.Cells[6].Value = tot.ToString("N2");
            }
            else
            {
                decimal sub = precio * cantidad;
                decimal tot = sub + (sub * (itbis / 100));

                row.Cells[0].Value = "0";
                row.Cells[1].Value = codigoProducto;
                row.Cells[2].Value = nombreProducto;
                row.Cells[3].Value = precio;
                row.Cells[4].Value = itbis;
                row.Cells[5].Value = cantidad;
                row.Cells[6].Value = tot.ToString("N2");
            }

            detalleorden.Rows.Add(row);

            RecalcularTotales();

            labelcantidadarticulos.Text = detalleorden.Rows.Count.ToString();

            txtcodigoproducto.Clear();
            txtnombreproducto.Clear();
            txtprecioproducto.Clear();
            txtiva.Clear();
            numCantidad.Value = 1;

            guardarordenbtn.Enabled = true;
            detalleorden.Enabled = true;
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

        private void eliminarFila_Click(object sender, EventArgs e)
        {
            if (EditarEstado == 1)
            {
                int filasActuales = detalleorden.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow);

                int filasSeleccionadas = detalleorden.SelectedRows.Count;

                if (filasActuales - filasSeleccionadas <= 0)
                {
                    MessageBox.Show("No puede eliminar todas las filas de una orden existente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            foreach (DataGridViewRow fila in detalleorden.SelectedRows)
            {
                if (!fila.IsNewRow)
                    detalleorden.Rows.Remove(fila);
            }

            RecalcularTotales();
            RecalcularGruposMinimos();
            LimpiarGruposVacios();
        }

        private void RecalcularGruposMinimos()
        {
            var gruposConProductos =
                detalleorden.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells["cuenta"].Value != null && r.Cells["cuenta"].Value.ToString() != "")
                .Select(r => Convert.ToDecimal(r.Cells["cuenta"].Value))
                .Distinct()
                .ToList();

            gruposMinimos = gruposConProductos.Count == 0 ? 1 : gruposConProductos.Count;
        }

        private void LimpiarGruposVacios()
        {
            var gruposConProductos =
                detalleorden.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells["cuenta"].Value != null && r.Cells["cuenta"].Value.ToString() != "")
                .Select(r => Convert.ToDecimal(r.Cells["cuenta"].Value))
                .Distinct()
                .ToList();

            listaGrupos = listaGrupos
                .Where(g => gruposConProductos.Contains(g.Cuenta))
                .ToList();

            RefrescarComboGrupos();
        }

        private Button botonActivo = null;

        private void BtnMesa_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            dynamic mesaInfo = btn.Tag;

            int idMesa = mesaInfo.Id;
            string numMesa = mesaInfo.Numero;
            int estadoMesa = mesaInfo.Estado;
            int reservadoMesa = mesaInfo.Reservado;

            if (modoUnion && !modoSeparar)
            {
                if (!mesasSeleccionadasUnion.Contains(idMesa))
                {
                    mesasSeleccionadasUnion.Add(idMesa);
                    btn.BackColor = Color.Gold;
                }
                else
                {
                    mesasSeleccionadasUnion.Remove(idMesa);
                    RestaurarColor(btn, estadoMesa, reservadoMesa);
                }
                return;
            }

            if (modoSeparar && !modoUnion)
            {
                if (!mesasSeleccionadasUnion.Contains(idMesa))
                {
                    mesasSeleccionadasUnion.Add(idMesa);
                    btn.BackColor = Color.OrangeRed;
                }
                else
                {
                    mesasSeleccionadasUnion.Remove(idMesa);
                    RestaurarColor(btn, estadoMesa, reservadoMesa);
                }
                return;
            }

            if (botonActivo != null)
            {
                dynamic anterior = botonActivo.Tag;
                RestaurarColor(botonActivo, anterior.Estado, anterior.Reservado);
            }

            botonActivo = btn;
            btn.BackColor = Color.DodgerBlue;
            idMesaSeleccionada = idMesa;
            NumeroMesa = Convert.ToInt32(numMesa);

            if (estadoMesa == 1)
            {
                CrearOrden.Enabled = false;
                EditarOrden.Enabled = true;
                FacturarOrden.Enabled = true;
                OcupadoMesa = 1;
                ReservadoMesa = 0;
            }
            else
            {
                CrearOrden.Enabled = true;
                EditarOrden.Enabled = false;
                FacturarOrden.Enabled = false;
                OcupadoMesa = 0;
                ReservadoMesa = 0;
            }

            if (reservadoMesa == 1)
            {
                CrearOrden.Enabled = true;
                EditarOrden.Enabled = false;
                FacturarOrden.Enabled = false;
                ReservadoMesa = 1;
                OcupadoMesa = 0;
            }
        }

        private void RestaurarColor(Button btn, int ocupado, int reservado)
        {
            if (reservado == 1)
                btn.BackColor = Color.MediumPurple;
            else if (ocupado == 1)
                btn.BackColor = Color.LightCoral;
            else
                btn.BackColor = Color.LightGreen;
        }

        private Button CrearBotonMesa(int id, string numero, string capacidad, int ocupado, int reservado, List<string> unidas)
        {
            Button btn = new Button();
            btn.Width = 150;
            btn.Height = 100;
            btn.Margin = new Padding(10);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            if (reservado == 1)
                btn.BackColor = Color.MediumPurple;
            else if (ocupado == 1)
                btn.BackColor = Color.LightCoral;
            else
                btn.BackColor = Color.LightGreen;

            btn.Tag = new
            {
                Id = id,
                Numero = numero,
                Estado = ocupado,
                Reservado = reservado,
                ListaMesas = unidas
            };

            string textoUnidas = unidas.Count > 0 ? string.Join(", ", unidas) : "-";

            btn.Text =
                $"Mesa #{numero}\n" +
                $"Unidas: {textoUnidas}\n" +
                $"Capacidad: {capacidad}";

            btn.Click += BtnMesa_Click;
            return btn;
        }

        public class MesaInfo
        {
            public int Id { get; set; }
            public string Numero { get; set; }
            public string Capacidad { get; set; }
            public int Ocupado { get; set; }
            public int Reservado { get; set; }
            public int IdGrupo { get; set; }
            public int EsPrincipal { get; set; }
        }

        private void Pedidos_Load(object sender, EventArgs e)
        {
            fechapedido.Value = SistemaFecha.FechaActual;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            cajerolabel.Text = "     Cajero: " + NombreUsuario;

            if (!cargandoOrden)
                tipoComp.SelectedIndex = 1;

            mesasprincipal.Controls.Clear();

            List<MesaInfo> mesas = new List<MesaInfo>();

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT IdMesa, Numero, Capacidad, Ocupado, Reservado, IdGrupo, EsPrincipal FROM Mesa", cn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    mesas.Add(new MesaInfo
                    {
                        Id = Convert.ToInt32(dr["IdMesa"]),
                        Numero = dr["Numero"] == DBNull.Value ? "?" : dr["Numero"].ToString(),
                        Capacidad = dr["Capacidad"] == DBNull.Value ? "0" : dr["Capacidad"].ToString(),
                        Reservado = dr["Reservado"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Reservado"]),
                        Ocupado = dr["Ocupado"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Ocupado"]),
                        IdGrupo = dr["IdGrupo"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IdGrupo"]),
                        EsPrincipal = dr["EsPrincipal"] == DBNull.Value ? 0 : Convert.ToInt32(dr["EsPrincipal"])
                    });
                }
            }

            var individuales = mesas.Where(m => m.IdGrupo == 0).ToList();
            foreach (var mesa in individuales)
            {
                var btn = CrearBotonMesa(
                    mesa.Id,
                    mesa.Numero,
                    mesa.Capacidad,
                    mesa.Ocupado,
                    mesa.Reservado,
                    new List<string>()
                );
                mesasprincipal.Controls.Add(btn);
            }

            var grupos = mesas
            .Where(m => m.IdGrupo > 0)
            .GroupBy(m => m.IdGrupo);

            foreach (var grupo in grupos)
            {
                var principal = grupo.FirstOrDefault(m => m.EsPrincipal == 1) ?? grupo.First();

                var secundarias = grupo
                    .Where(m => m.Id != principal.Id)
                    .ToList();

                var unidas = secundarias
                .Select(m => m.Numero)
                .ToList();


                int capacidadTotal = grupo.Sum(m => int.TryParse(m.Capacidad, out int c) ? c : 0);

                var btn = CrearBotonMesa(
                    principal.Id,
                    principal.Numero,
                    capacidadTotal.ToString(),
                    principal.Ocupado,
                    principal.Reservado,
                    unidas
                );

                mesasprincipal.Controls.Add(btn);
            }

            string consultaID = "SELECT TOP 1 IdPedido FROM Pedido ORDER BY IdPedido DESC";

            string busquedaCaja = @"
            SELECT 
                c.Nombre AS nombre_caja,
                c.Numero AS numero_caja
            FROM Configuracion conf
            INNER JOIN Caja c
                ON conf.IdCaja = c.IdCaja
            WHERE conf.NombrePC = @NombrePC";

            string PermisosSQL = @"
            SELECT 
                CambiarPrecio,
                PrecioMinimo
            FROM PermisosUsuario
            WHERE IdUsuario = @IdUsuario;";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(consultaID, con))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        int nuevoId = Convert.ToInt32(resultado) + 1;
                        txtidpedido.Text = nuevoId.ToString();
                    }
                    else
                    {
                        txtidpedido.Text = "1";
                    }
                }

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

                using (SqlCommand cmdPermiso = new SqlCommand(PermisosSQL, con))
                {
                    cmdPermiso.Parameters.AddWithValue("@IdUsuario", IdUsuario);

                    using (SqlDataReader dr = cmdPermiso.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            CambiarPrecio = Convert.ToInt32(dr["CambiarPrecio"]) == 1;
                            PrecioMinimo = Convert.ToInt32(dr["PrecioMinimo"]) == 1;
                        }
                    }

                    txtprecioproducto.Enabled = CambiarPrecio;
                }
            }

            if (detalleorden.ColumnCount == 0)
            {
                detalleorden.Columns.Add("cuenta", "Cuenta");
                detalleorden.Columns.Add("codigoProducto", "Codigo");
                detalleorden.Columns.Add("nombreProducto", "Nombre");
                detalleorden.Columns.Add("precio", "Precio");
                detalleorden.Columns.Add("ITBIS", "ITBIS");
                detalleorden.Columns.Add("cantidad", "Cantidad");
                detalleorden.Columns.Add("subtotal", "Importe");
            }



            NotifComanda();
            Comprobantes();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buscar.Focus();
            buscar.PerformClick();
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

        private void limpiarbtn_Click(object sender, EventArgs e)
        {
            IDMesa = 0;
            idMesaSeleccionada = -1;
            PedidoID = 0;
            EditarEstado = 0;
            NumeroMesa = 0;
            OcupadoMesa = 0;
            ReservadoMesa = 0;
            EditarEstado = 0;
            CuentaSeparada = 0;
            ModoElminar = 0;
            OrdenGrupo = 0;

            txtnombrecompleto.Text = "AL CONTADO";
            idclientetxt.Text = "1";
            IdClientePersonaST = "1";
            numerotxt.Clear();
            rnc.Clear();

            tipodoccmbx.SelectedIndex = -1;

            txtcodigoproducto.Clear();
            txtnombreproducto.Clear();
            txtprecioproducto.Clear();
            grupoCuenta.DataSource = null;
            grupoCuenta.Items.Clear();
            AvisoCuentaSeparada.Visible = false;
            numCantidad.Value = numCantidad.Minimum;
            numCantidad.Enabled = false;
            txtiva.Clear();
            labelsubtotal.Text = "0";
            labeltotal.Text = "0";
            detalleorden.Rows.Clear();
            totalAcumulado = 0;
            subtotalAcumulado = 0;
            cantidadProd = 0;
            labelcantidadarticulos.Text = "0";

            buscarproductobtn.Enabled = false;
            txtcodigoproducto.Enabled = false;
            txtnombreproducto.Enabled = false;
            txtprecioproducto.Enabled = false;
            bajarproductobtn.Enabled = false;
            guardarordenbtn.Enabled = false;
            buscarclientebtn.Enabled = false;
            nota.Enabled = false;
            guardarordenbtn.Enabled = false;
            MesaLabel.Text = "     Mesa asignada: ";
            panelCuentaSeparada.Enabled = false;
            separarcuenta.BackColor = Color.White;
            mesasprincipal.Enabled = true;
            panelacciones.Enabled = true;
            CrearOrden.Enabled = true;
            EditarOrden.Enabled = true;
            FacturarOrden.Enabled = true;
            ocupadachk.Enabled = true;
            reservadachk.Enabled = true;
            buscarmesatxt.Clear();
            buscarmesatxt.Enabled = true;
            modoSeparar = false;
            modoUnion = false;
            mesasSeleccionadasUnion.Clear();
            modoEntregar = false;
            tipoComp.Enabled = true;
            idclientetxt.Enabled = false;
            txtnombrecompleto.Enabled = false;
            rnc.Enabled = false;
            numerotxt.Enabled = false;
            detalleorden.Enabled = false;

            VerificarOrden();
            Pedidos_Load(sender, e);
        }

        private void NotifComanda()
        {
            string sql = "SELECT COUNT(*) FROM Comanda WHERE Estado = 'Cocina'";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    int cantidad = Convert.ToInt32(cmd.ExecuteScalar());

                    if (cantidad > 0)
                    {
                        ordenesNotif.Visible = true;
                    }
                    else
                    {
                        ordenesNotif.Visible = false;
                    }
                }
            }
        }

        private void tabladatospedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tabladatospedidos.Rows[e.RowIndex];

                PedidoID = Convert.ToInt32(row.Cells["IdPedido"].Value);
                IDMesa = Convert.ToInt32(row.Cells["IdMesa"].Value);
            }
        }

        private void facturarbtn_Click(object sender, EventArgs e)
        {
            if (PedidoID <= 0)
            {
                MessageBox.Show("Seleccione un pedido válido.");
                return;
            }

            if (!facturarDesdeMesa)
            {
                if (tabladatospedidos.CurrentRow == null)
                {
                    MessageBox.Show("Debe seleccionar una factura.");
                    return;
                }

                object valorTotal = tabladatospedidos.CurrentRow.Cells["Total"].Value;

                if (valorTotal == null || valorTotal == DBNull.Value || valorTotal.ToString() == "")
                {
                    MessageBox.Show("El valor del total no es válido.");
                    return;
                }

                Total = Convert.ToDecimal(valorTotal);
            }
            else
            {
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
            }

            facturarDesdeMesa = false;

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
                efectivotxt.Text = TotalPedido.ToString("N2");
                tarjetaMonto.Text = TotalPedido.ToString("N2");
                transfMonto.Text = TotalPedido.ToString("N2");
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

                    string queryMesa = "SELECT IdMesa FROM Pedido WHERE IdPedido = @id";
                    SqlCommand cmdMesa = new SqlCommand(queryMesa, conexion, trans);
                    cmdMesa.Parameters.AddWithValue("@id", PedidoID);

                    int idMesa = Convert.ToInt32(cmdMesa.ExecuteScalar());

                    SqlCommand cmdFacturar = new SqlCommand(
                        "UPDATE Pedido SET Estado='Facturado' WHERE IdPedido=@id",
                        conexion, trans);
                    cmdFacturar.Parameters.AddWithValue("@id", PedidoID);
                    cmdFacturar.ExecuteNonQuery();

                    int idGrupo = 0;

                    string queryGrupo = "SELECT IdGrupo FROM Mesa WHERE IdMesa = @idMesa";
                    using (SqlCommand cmdGrupo = new SqlCommand(queryGrupo, conexion, trans))
                    {
                        cmdGrupo.Parameters.AddWithValue("@idMesa", idMesa);
                        object grupoResult = cmdGrupo.ExecuteScalar();
                        idGrupo = grupoResult != null ? Convert.ToInt32(grupoResult) : 0;
                    }

                    if (idGrupo > 0)
                    {
                        string liberarGrupoQuery = @"
                        UPDATE Mesa 
                        SET Ocupado = 0
                        WHERE IdGrupo = @grupo";

                        using (SqlCommand cmd = new SqlCommand(liberarGrupoQuery, conexion, trans))
                        {
                            cmd.Parameters.AddWithValue("@grupo", idGrupo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string liberarMesaQuery = @"
                        UPDATE Mesa 
                        SET Ocupado = 0
                        WHERE IdMesa = @mesa";

                        using (SqlCommand cmd = new SqlCommand(liberarMesaQuery, conexion, trans))
                        {
                            cmd.Parameters.AddWithValue("@mesa", idMesa);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    SqlCommand cmdLiberarComanda = new SqlCommand(
                        "UPDATE Comanda SET Estado='Entregado' WHERE IdPedido=@id",
                        conexion, trans);
                    cmdLiberarComanda.Parameters.AddWithValue("@id", PedidoID);
                    cmdLiberarComanda.ExecuteNonQuery();

                    RegistrarPago(conexion, trans);
                    RebajarInventario(conexion, trans, PedidoID);

                    trans.Commit();

                    MessageBox.Show("Facturacion completada!.");
                    buscar.PerformClick();
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

        private void cancelarpedido_Click(object sender, EventArgs e)
        {
            if (PedidoID <= 0)
            {
                MessageBox.Show("Seleccione un pedido válido.");
                return;
            }

            if (tabladatospedidos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un pedido en la tabla.");
                return;
            }

            string estado = tabladatospedidos.SelectedRows[0].Cells["Estado"].Value.ToString();

            if (estado == "Facturado")
            {
                MessageBox.Show("No puede cancelar este pedido porque ya está FACTURADO.");
                return;
            }

            if (estado == "Cancelado")
            {
                MessageBox.Show("Este pedido ya está CANCELADO.");
                return;
            }

            if (estado == "Pendiente")
            {
                DialogResult cancelar = MessageBox.Show(
                    "¿Desea cancelar esta factura?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (cancelar == DialogResult.Yes)
                {
                    cancelarPanel.Visible = true;
                    cancelarPanel.BringToFront();
                    cancelarPanel.Location = new Point(0, 0);
                }
                else
                {
                    MessageBox.Show("Operación cancelada.");
                }
            }
        }

        private void pendientechk_CheckedChanged(object sender, EventArgs e)
        {
            if (pendientechk.Checked == true)
            {
                canceladochk.Checked = false;
                facturadochk.Checked = false;
                todoschk.Checked = false;
                PedidoID = 0;
                cancelarpedido.Enabled = true;
                facturarbtn.Enabled = true;
                FiltroDatosBusqueda();
            }
        }

        private void facturadochk_CheckedChanged(object sender, EventArgs e)
        {
            if (facturadochk.Checked == true)
            {
                canceladochk.Checked = false;
                pendientechk.Checked = false;
                todoschk.Checked = false;
                PedidoID = 0;
                cancelarpedido.Enabled = false;
                facturarbtn.Enabled = false;
                FiltroDatosBusqueda();
            }
        }

        private void canceladochk_CheckedChanged(object sender, EventArgs e)
        {
            if (canceladochk.Checked == true)
            {
                facturadochk.Checked = false;
                pendientechk.Checked = false;
                todoschk.Checked = false;
                PedidoID = 0;
                cancelarpedido.Enabled = false;
                facturarbtn.Enabled = false;
                cancelarpedido.Enabled = false;
                FiltroDatosBusqueda();
            }
        }

        private void todoschk_CheckedChanged(object sender, EventArgs e)
        {
            if (todoschk.Checked == true)
            {
                facturadochk.Checked = false;
                pendientechk.Checked = false;
                canceladochk.Checked = false;
                PedidoID = 0;
                cancelarpedido.Enabled = true;
                facturarbtn.Enabled = true;
                FiltroDatosBusqueda();
            }
        }

        private void txtbusquedafactura_TextChanged(object sender, EventArgs e)
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
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void GenerarFacturaPDF(int idPedido, int cuenta)
        {
            try
            {
                string folderPath = @"C:\SistemaArchivos\Facturas\";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(
                    folderPath,
                    cuenta == 0
                        ? $"Pedido_{idPedido}.pdf"
                        : $"Pedido_{idPedido}_Cuenta_{cuenta}.pdf"
                );

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

                gfx.DrawString(
                    cuenta == 0 ? "ORDEN" : $"ORDEN - CUENTA {cuenta}",
                    titleFont,
                    XBrushes.Black,
                    new XRect(0, currentY, page.Width, 40),
                    XStringFormats.TopCenter
                );

                currentY += 50;

                string nombreCliente = "", fecha = "", mesa = "", comprobante = "";

                using (SqlConnection con = new SqlConnection(conexionString))
                {
                    con.Open();

                    string sqlPedido = @"
                    SELECT IdPedido, Fecha, NombreCliente, IdMesa, Comprobante
                    FROM Pedido
                    WHERE IdPedido = @id";

                    using (SqlCommand cmd = new SqlCommand(sqlPedido, con))
                    {
                        cmd.Parameters.AddWithValue("@id", idPedido);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                fecha = Convert.ToDateTime(dr["Fecha"]).ToString("dd/MM/yyyy HH:mm");
                                nombreCliente = dr["NombreCliente"].ToString();
                                mesa = dr["IdMesa"].ToString();
                                comprobante = dr["Comprobante"]?.ToString() ?? "";
                            }
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

                decimal totalFactura = 0;

                using (SqlConnection con = new SqlConnection(conexionString))
                {
                    con.Open();

                    string sqlDetalle = @"
                    SELECT d.Cantidad,
                           d.PrecioUnitario,
                           (d.Cantidad * d.PrecioUnitario) AS Subtotal,
                           p.Nombre
                    FROM DetallePedido d
                    INNER JOIN ProductoVenta p ON p.IdProducto = d.IdProducto
                    WHERE d.IdPedido = @id AND d.Cuenta = @cuenta";

                    using (SqlCommand cmd = new SqlCommand(sqlDetalle, con))
                    {
                        cmd.Parameters.AddWithValue("@id", idPedido);
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string prod = dr["Nombre"].ToString();
                                decimal cant = Convert.ToDecimal(dr["Cantidad"]);
                                decimal precio = Convert.ToDecimal(dr["PrecioUnitario"]);
                                decimal sub = Convert.ToDecimal(dr["Subtotal"]);

                                totalFactura += sub;

                                gfx.DrawString(prod, textFont, XBrushes.Black, marginLeft, currentY);
                                gfx.DrawString(cant.ToString(), textFont, XBrushes.Black, marginLeft + 250, currentY);
                                gfx.DrawString(precio.ToString("N2"), textFont, XBrushes.Black, marginLeft + 320, currentY);
                                gfx.DrawString(sub.ToString("N2"), textFont, XBrushes.Black, marginLeft + 400, currentY);

                                currentY += lineHeight;
                            }
                        }
                    }
                }

                currentY += 10;
                gfx.DrawLine(XPens.Black, marginLeft, currentY, page.Width - marginLeft, currentY);
                currentY += 20;

                gfx.DrawString($"TOTAL: RD$ {totalFactura:N2}", titleFont, XBrushes.Black, marginLeft + 250, currentY);

                document.Save(filePath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message);
            }
        }

        private void GenerarFacturasPorPedido(int idPedido)
        {
            List<int> cuentas = new List<int>();

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();

                string sql = @"
                SELECT DISTINCT Cuenta
                FROM DetallePedido
                WHERE IdPedido = @id
                ORDER BY Cuenta";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", idPedido);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            cuentas.Add(Convert.ToInt32(dr["Cuenta"]));
                        }
                    }
                }
            }

            if (cuentas.Count == 1 && cuentas[0] == 0)
            {
                GenerarFacturaPDF(idPedido, 0);
                return;
            }

            foreach (int cuenta in cuentas.Where(c => c > 0))
            {
                GenerarFacturaPDF(idPedido, cuenta);
            }
        }

        private void imprimirbtn_Click(object sender, EventArgs e)
        {
            if (PedidoID > 0)
            {
                try
                {
                    GenerarFacturasPorPedido(PedidoID);
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

        private void txtproductobusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltroDatosProducto(txtproductobusqueda.Text);
        }

        private void FiltroDatosProducto(string busqueda)
        {
            //No funciona todavia, es codigo viejo
            /*
            using (SqlConnection conectar = new SqlConnection(conexionString))
            {
                try
                {
                    conectar.Open();

                    string query = @"
                    SELECT id, codigo_producto, nombre_producto, categoria, precio_venta, iva, existencia, codigo_de_barra 
                    FROM productos
                    WHERE (CAST(id AS VARCHAR) LIKE @buscar OR
                    codigo_producto LIKE @buscar OR
                    nombre_producto LIKE @buscar OR
                    categoria LIKE @buscar OR
                    codigo_de_barra LIKE @buscar)
                    AND existencia > 0";

                    using (SqlCommand comando = new SqlCommand(query, conectar))
                    {
                        comando.Parameters.AddWithValue("@buscar", "%" + busqueda + "%");

                        SqlDataAdapter da = new SqlDataAdapter(comando);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        tablapanelproducto.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }*/
        }

        private void FiltroDatosClientes(string busqueda)
        {
            //No funciona todavia, es codigo viejo
            /*
            using (SqlConnection conectar = new SqlConnection(conexionString))
            {
                try
                {
                    conectar.Open();

                    string query = @"
                            SELECT id, nombre_cliente, apellido_cliente, identificacion, telefono
                            FROM cliente
                            WHERE (CAST(id AS VARCHAR) LIKE @buscar OR
                            nombre_cliente LIKE @buscar OR
                            apellido_cliente LIKE @buscar OR
                            identificacion LIKE @buscar OR
                            telefono LIKE @buscar)
                            AND estado = 1";

                    using (SqlCommand comando = new SqlCommand(query, conectar))
                    {
                        comando.Parameters.AddWithValue("@buscar", "%" + busqueda + "%");

                        SqlDataAdapter da = new SqlDataAdapter(comando);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        tablaclientes.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }*/
        }

        private void txtclientebusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltroDatosClientes(txtclientebusqueda.Text);
        }

        private void txtnombrecompleto_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtnombrecompleto.Text))
            {
                habilitarbotones();
            }
        }

        private void pagarefectivo_Click(object sender, EventArgs e)
        {
            detallepagopanel.Visible = false;
            detallepagopanel.Location = new Point(1617, 6);

            FacturarPedido();
            MostrarDevuelta();

            devueltapanel.Visible = true;
            devueltapanel.Location = new Point(466, 0);
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
            button9.Enabled = true;
            RecalcularTotalesPago();
            RecargarRestante();
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
                INSERT INTO DetallePago (IdPedido, TipoDetalle, Efectivo, Devuelta, Tarjeta, TarjetaNombre, Transferencia, Banco, Total, Estado, Referencia, Origen)
                VALUES (@IdPedido, @TipoDetalle, @Efectivo, @Devuelta, @Tarjeta, @TarjetaNombre, @Transferencia, @Banco, @Total, @Estado, @Referencia, 1)",
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

        private void VerificarOrden()
        {
            if (mesasprincipal.Enabled == true)
            {
                bloqueomesas.Visible = false;
                bloqueomesas.Location = new Point(804, 109);
                flecharoja.Visible = false;
            }
            else if (mesasprincipal.Enabled == false)
            {
                bloqueomesas.Visible = true;
                bloqueomesas.BringToFront();
                bloqueomesas.Location = new Point(6, 104);
                flecharoja.Visible = true;
            }
        }

        private void BtnCrearOrden()
        {
            if (panelacciones.Enabled == false)
            {
                MessageBox.Show("Acciones Bloqueadas.");
                return;
            }

            dynamic mesaInfo = botonActivo.Tag;

            int idMesa = mesaInfo.Id;
            List<string> unidas = mesaInfo.ListaMesas;

            idMesaSeleccionada = idMesa;
            mesasDelGrupo = new List<int>() { idMesa };
            mesasDelGrupo.AddRange(unidas.Select(u => Convert.ToInt32(u)));

            string textoMesas = string.Join(", ", mesasDelGrupo);

            tabControl1.SelectedIndex = 1;
            IDMesa = idMesa;

            MesaLabel.Text = $"     Mesa asignada: {textoMesas}";
            mesasprincipal.Enabled = false;
            panelacciones.Enabled = false;
            ocupadachk.Enabled = false;
            reservadachk.Enabled = false;
            buscarmesatxt.Enabled = false;
            EditarEstado = 0;
            tipodoccmbx.SelectedIndex = 0;
            habilitarbotones();
            VerificarOrden();
        }

        private void CrearOrden_Click(object sender, EventArgs e)
        {
            if (botonActivo == null || idMesaSeleccionada == -1)
            {
                MessageBox.Show("Debe seleccionar una mesa.");
                return;
            }

            if (OcupadoMesa == 1)
            {
                MessageBox.Show("Mesa ocupada.");
                return;
            }

            if (ReservadoMesa == 1 && Autorizar == 0)
            {
                DialogResult autorizar = MessageBox.Show("Mesa Reservada, ¿Crear Orden Nueva?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (autorizar == DialogResult.Yes)
                {
                    autorizacionpanel.Visible = true;
                    autorizacionpanel.BringToFront();
                    autorizacionpanel.Location = new Point(6, 104);
                    txtusuario.Text = NombreUsuario;
                    if (txtusuario.Text == NombreUsuario)
                    {
                        txtpass.Focus();
                    }
                    else
                    {
                        txtusuario.Focus();
                    }
                    panelacciones.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Operación cancelada.");
                    return;
                }
            }
            else
            {
                BtnCrearOrden();
            }
        }

        private void FacturarOrden_Click(object sender, EventArgs e)
        {
            if (idMesaSeleccionada == -1 || idMesaSeleccionada == 0)
            {
                MessageBox.Show("Debe seleccionar una mesa.");
                return;
            }

            if (panelacciones.Enabled == false)
            {
                MessageBox.Show("Acciones Bloqueadas.");
                return;
            }

            FacturarMesa(idMesaSeleccionada);
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
                button9.Enabled = false;
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
            Pedidos_Load(sender, e);
        }

        private void numCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                bajarproductobtn_Click(sender, e);
                e.Handled = true;
            }
        }

        private void EditarOrden_Click(object sender, EventArgs e)
        {
            if (idMesaSeleccionada == -1 || idMesaSeleccionada == 0)
            {
                MessageBox.Show("Debe seleccionar una mesa.");
                return;
            }

            if (panelacciones.Enabled == false)
            {
                MessageBox.Show("Acciones Bloqueadas.");
                return;
            }

            if (OcupadoMesa == 0)
            {
                MessageBox.Show("Mesa no está ocupada.");
                return;
            }

            tabControl1.SelectedIndex = 1;

            mesasprincipal.Enabled = false;
            panelacciones.Enabled = false;

            EditarEstado = 1;

            mesasDelGrupo.Clear();
            mesasDelGrupo.Add(idMesaSeleccionada);

            CargarOrdenDeMesa(idMesaSeleccionada, NumeroMesa);
            VerificarOrden();
        }

        private void CargarOrdenDeMesa(int idMesa, int numMesa)
        {
            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();

                string sqlCOM = @"
                SELECT TOP 1 IdPedido, Fecha, IdMesa, IdClientePersona, 
                       NombreCliente, Total, Nota, Comprobante
                FROM Pedido
                WHERE IdMesa = @IdMesa AND Estado = 'Pendiente'
                ORDER BY Fecha DESC;";

                int idPedido = 0;

                using (SqlCommand cmd = new SqlCommand(sqlCOM, cn))
                {
                    cmd.Parameters.AddWithValue("@IdMesa", idMesa);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            MessageBox.Show("La mesa no tiene ordenes pendientes.");
                            limpiarbtn.PerformClick();
                            tabControl1.SelectedIndex = 0;
                            return;
                        }

                        idPedido = Convert.ToInt32(dr["IdPedido"]);
                        PedidoID = idPedido;

                        txtidpedido.Text = idPedido.ToString();
                        fechapedido.Value = Convert.ToDateTime(dr["Fecha"]);
                        idclientetxt.Text = dr["IdClientePersona"].ToString();
                        txtnombrecompleto.Text = dr["NombreCliente"].ToString();
                        labeltotal.Text = Convert.ToDecimal(dr["Total"]).ToString();
                        notatxt.Text = dr["Nota"] == DBNull.Value ? "" : dr["Nota"].ToString();

                        cargandoOrden = true;

                        Comprobantetxt.Text = dr["Comprobante"] == DBNull.Value ? "" : dr["Comprobante"].ToString();

                        MesaLabel.Text = $"     Mesa asignada: {NumeroMesa}";
                    }
                }

                DetalleOrden(idPedido, cn);

                var cuentas = CuentasPedido(idPedido);

                grupoCuenta.Items.Clear();

                grupoCuenta.DataSource = null;

                gruposMinimos = cuentas.Count;

                listaGrupos = cuentas.Select(c => new CuentaItem { Cuenta = c }).ToList();

                cargandoGrupos = true;

                grupoCuenta.DataSource = null;
                grupoCuenta.DisplayMember = "NombreGrupo";
                grupoCuenta.ValueMember = "Cuenta";
                grupoCuenta.DataSource = listaGrupos;

                cargandoGrupos = false;


                if (grupoCuenta.Items.Count > 0)
                {
                    grupoCuenta.SelectedIndex = 0;
                    panelCuentaSeparada.Enabled = true;
                    AvisoCuentaSeparada.Visible = true;
                }

                tipoComp.SelectedIndex = -1;

                tipoComp.Enabled = false;

                cargandoOrden = false;

                habilitarbotones();
            }
        }

        private List<decimal> CuentasPedido(int idPedido)
        {
            List<decimal> cuentas = new List<decimal>();

            string sql = @"SELECT DISTINCT Cuenta 
                   FROM DetallePedido 
                   WHERE IdPedido = @IdPedido
                   ORDER BY Cuenta";

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@IdPedido", idPedido);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            cuentas.Add(Convert.ToDecimal(dr["Cuenta"]));
                        }
                    }
                }
            }

            return cuentas;
        }

        public class CuentaItem
        {
            public decimal Cuenta { get; set; }
            public string NombreGrupo => "Grupo " + Cuenta;
        }

        private void DetalleOrden(int idPedido, SqlConnection cn)
        {
            string sqlDet = @"
            SELECT  d.IdProducto,
                    d.Cuenta,
                    p.Nombre,
                    d.PrecioUnitario,
                    p.Itbis,
                    d.Cantidad,
                    d.Cantidad * d.PrecioUnitario AS Importe
            FROM DetallePedido d
            INNER JOIN ProductoVenta p ON p.IdProducto = d.IdProducto
            WHERE d.IdPedido =  @IdPedido;";

            using (SqlCommand cmd = new SqlCommand(sqlDet, cn))
            {
                cmd.Parameters.AddWithValue("@IdPedido", idPedido);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    detalleorden.Rows.Clear();

                    while (dr.Read())
                    {
                        int fila = detalleorden.Rows.Add();

                        detalleorden.Rows[fila].Cells["cuenta"].Value = dr["Cuenta"];
                        detalleorden.Rows[fila].Cells["codigoProducto"].Value = dr["IdProducto"];
                        detalleorden.Rows[fila].Cells["nombreProducto"].Value = dr["Nombre"];
                        detalleorden.Rows[fila].Cells["precio"].Value = dr["PrecioUnitario"];
                        detalleorden.Rows[fila].Cells["ITBIS"].Value = dr["Itbis"];
                        detalleorden.Rows[fila].Cells["cantidad"].Value = dr["Cantidad"];
                        detalleorden.Rows[fila].Cells["subtotal"].Value = dr["Importe"];
                    }
                }
            }
        }

        private void FacturarMesa(int idMesa)
        {
            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();

                string sqlCab = @"
                SELECT TOP 1 IdPedido, Fecha, IdMesa, IdClientePersona, 
                       NombreCliente, Total, Nota
                FROM Pedido
                WHERE IdMesa = @IdMesa AND Estado = 'Pendiente'
                ORDER BY Fecha DESC;";

                using (SqlCommand cmd = new SqlCommand(sqlCab, cn))
                {
                    cmd.Parameters.AddWithValue("@IdMesa", idMesa);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            MessageBox.Show("La mesa no tiene órdenes pendientes.");
                            return;
                        }

                        PedidoID = Convert.ToInt32(dr["IdPedido"]);
                        Total = Convert.ToDecimal(dr["Total"]);

                        tabControl1.SelectedIndex = 2;
                        facturarDesdeMesa = true;
                        facturarbtn.PerformClick();
                    }
                }
            }
        }

        private void UnirMesa_Click(object sender, EventArgs e)
        {
            modoUnion = true;
            Pedidos_Load(sender, e);
            mesasSeleccionadasUnion.Clear();
            UnirMesa.BackColor = Color.Lime;
            SepararMesa.BackColor = SystemColors.ButtonHighlight;
            SepararMesa.Enabled = false;

            SiUnion.Visible = true;
            NoUnion.Visible = true;

            CrearOrden.Enabled = false;
            EditarOrden.Enabled = false;
            FacturarOrden.Enabled = false;
            VerComanda.Enabled = false;

            MessageBox.Show("Seleccione las mesas para unir.");
        }

        private void NoUnion_Click(object sender, EventArgs e)
        {
            modoUnion = false;
            modoSeparar = false;

            mesasSeleccionadasUnion.Clear();

            UnirMesa.BackColor = SystemColors.ButtonHighlight;
            SepararMesa.BackColor = SystemColors.ButtonHighlight;

            SiUnion.Visible = false;
            NoUnion.Visible = false;

            UnirMesa.Enabled = true;
            SepararMesa.Enabled = true;

            CrearOrden.Enabled = true;
            EditarOrden.Enabled = true;
            FacturarOrden.Enabled = true;
            VerComanda.Enabled = true;

            Pedidos_Load(sender, e);
        }

        private void SiUnion_Click(object sender, EventArgs e)
        {
            if (modoUnion)
            {
                if (mesasSeleccionadasUnion.Count < 2)
                {
                    MessageBox.Show("Debe seleccionar al menos 2 mesas para unir.");
                    return;
                }

                UnirMesasSeleccionadas();
            }
            else if (modoSeparar)
            {
                if (mesasSeleccionadasUnion.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar una mesa del grupo que desea separar.");
                    return;
                }

                SepararMesasSeleccionadas();
            }

            modoUnion = false;
            modoSeparar = false;

            SiUnion.Visible = false;
            NoUnion.Visible = false;

            CrearOrden.Enabled = true;
            EditarOrden.Enabled = true;
            FacturarOrden.Enabled = true;
            VerComanda.Enabled = true;

            UnirMesa.BackColor = SystemColors.ButtonHighlight;
            SepararMesa.BackColor = SystemColors.ButtonHighlight;

            Pedidos_Load(sender, e);
        }

        private void UnirMesasSeleccionadas()
        {
            int idGrupo = ObtenerNuevoIdGrupo();
            int mesaPrincipal = mesasSeleccionadasUnion.First();

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction();

                try
                {
                    SqlCommand c1 = new SqlCommand(
                        "UPDATE Mesa SET IdGrupo=@grupo, EsPrincipal=1 WHERE IdMesa=@id",
                        cn, tr);
                    c1.Parameters.AddWithValue("@grupo", idGrupo);
                    c1.Parameters.AddWithValue("@id", mesaPrincipal);
                    c1.ExecuteNonQuery();

                    string q2 = "UPDATE Mesa SET IdGrupo=@grupo, EsPrincipal=0 WHERE IdMesa=@id";
                    foreach (int mesa in mesasSeleccionadasUnion.Skip(1))
                    {
                        SqlCommand c2 = new SqlCommand(q2, cn, tr);
                        c2.Parameters.AddWithValue("@grupo", idGrupo);
                        c2.Parameters.AddWithValue("@id", mesa);
                        c2.ExecuteNonQuery();
                    }

                    tr.Commit();
                    SepararMesa.Enabled = true;
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        private void SepararMesasSeleccionadas()
        {
            int mesaSeleccionada = mesasSeleccionadasUnion.First();
            int idGrupo = 0;

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT IdGrupo FROM Mesa WHERE IdMesa = @id", cn);
                cmd.Parameters.AddWithValue("@id", mesaSeleccionada);

                idGrupo = Convert.ToInt32(cmd.ExecuteScalar());
            }

            if (idGrupo == 0)
            {
                MessageBox.Show("Esta mesa no pertenece a ningún grupo.");
                UnirMesa.Enabled = true;
                return;
            }

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Mesa SET IdGrupo = 0, EsPrincipal = 0 WHERE IdGrupo = @grupo", cn, tr);

                    cmd.Parameters.AddWithValue("@grupo", idGrupo);
                    cmd.ExecuteNonQuery();

                    tr.Commit();
                    UnirMesa.Enabled = true;
                    SepararMesa.Enabled = true;

                    MessageBox.Show("Las mesas fueron separadas correctamente.");

                    CrearOrden.Enabled = true;
                    EditarOrden.Enabled = true;
                    FacturarOrden.Enabled = true;
                    VerComanda.Enabled = true;
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    MessageBox.Show("Error separando las mesas: " + ex.Message);
                }
            }
        }

        private int ObtenerNuevoIdGrupo()
        {
            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(IdGrupo),0)+1 FROM Mesa", cn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void SepararMesa_Click(object sender, EventArgs e)
        {
            modoSeparar = true;
            modoUnion = false;
            Pedidos_Load(sender, e);

            mesasSeleccionadasUnion.Clear();

            SepararMesa.BackColor = Color.Lime;
            UnirMesa.BackColor = SystemColors.ButtonHighlight;
            UnirMesa.Enabled = false;

            SiUnion.Visible = true;
            NoUnion.Visible = true;

            MessageBox.Show("Seleccione las mesas que desea separar.");
        }

        private void continuar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            limpiarbtn_Click(sender, e);
        }

        private void autorizar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    string queryUsuario = @"
                    SELECT IdUsuario 
                    FROM Usuario 
                    WHERE Login = @usuario AND Contrasena = @pass AND Activo = 1";

                    int idUsuario = 0;
                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conexion))
                    {
                        cmd.Parameters.AddWithValue("@usuario", txtusuario.Text);
                        cmd.Parameters.AddWithValue("@pass", txtpass.Text);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado == null)
                        {
                            MessageBox.Show("Credenciales incorrectas / Usuario Inactivo.");
                            return;
                        }

                        idUsuario = Convert.ToInt32(resultado);
                    }

                    string queryPermiso = @"
                    SELECT CrearOrdenReservacion 
                    FROM PermisosUsuario 
                    WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand cmdPermiso = new SqlCommand(queryPermiso, conexion))
                    {
                        cmdPermiso.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        object permisoResult = cmdPermiso.ExecuteScalar();

                        if (permisoResult != null)
                            permisoSi = Convert.ToBoolean(permisoResult);
                    }

                    if (permisoSi)
                    {
                        panelacciones.Enabled = true;
                        Autorizar = 1;

                        CrearOrden_Click(sender, e);
                        autorizacionpanel.Visible = false;
                        autorizacionpanel.BringToFront();
                        autorizacionpanel.Location = new Point(807, 5);
                        txtusuario.Clear();
                        txtpass.Clear();
                    }
                    else
                    {
                        MessageBox.Show("No tiene Permisos para esta operación");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cancelarAutorizar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Operación Cancelada.");
            autorizacionpanel.Visible = false;
            autorizacionpanel.BringToFront();
            autorizacionpanel.Location = new Point(807, 5);
            txtusuario.Clear();
            txtpass.Clear();
            panelacciones.Enabled = true;
            return;
        }

        private void numerotxt_TextChanged(object sender, EventArgs e)
        {
            string posNum = numerotxt.Text;
            posNum = posNum.Replace("-", "");

            if (posNum.Length > 10)
            {
                posNum = posNum.Substring(0, 10);
            }

            if (posNum.Length > 3)
            {
                posNum = posNum.Insert(3, "-");
            }

            if (posNum.Length > 7)
            {
                posNum = posNum.Insert(7, "-");
            }

            numerotxt.Text = posNum;
            numerotxt.SelectionStart = numerotxt.Text.Length;
        }

        private void buscar_Click(object sender, EventArgs e)
        {
            FiltroDatosBusqueda();
        }

        private void separarcuenta_Click(object sender, EventArgs e)
        {
            if (CuentaSeparada == 1)
            {
                MessageBox.Show("La cuenta ya está separada.");
                return;
            }
            else
            {
                DialogResult confirmar = MessageBox.Show("Activar Cuenta Separada LIMPIA la orden actual", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmar == DialogResult.Yes)
                {
                    panelCuentaSeparada.Enabled = true;
                    CuentaSeparada = 1;
                    separarcuenta.BackColor = Color.Gold;
                    detalleorden.Rows.Clear();
                    nuevoGrupo.PerformClick();
                    AvisoCuentaSeparada.Visible = true;
                    separarcuenta.Enabled = false;
                }
            }
        }

        private void nuevoGrupo_Click(object sender, EventArgs e)
        {
            if (ModoElminar == 0)
            {
                toolTip1.SetToolTip(nuevoGrupo, "Nuevo Grupo");

                decimal nuevaCuenta;

                if (listaGrupos.Count == 0)
                    nuevaCuenta = 1;
                else
                    nuevaCuenta = listaGrupos.Max(g => g.Cuenta) + 1;

                listaGrupos.Add(new CuentaItem { Cuenta = nuevaCuenta });

                RefrescarComboGrupos();

                grupoCuenta.SelectedValue = nuevaCuenta;
            }
            else if (ModoElminar == 1)
            {
                toolTip1.SetToolTip(nuevoGrupo, "Eliminar Grupo");

                if (listaGrupos.Count <= gruposMinimos)
                {
                    MessageBox.Show("No puede eliminar más grupos.\nLa orden original tiene " + gruposMinimos + " grupos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                decimal cuentaActual = Convert.ToDecimal(grupoCuenta.SelectedValue);

                var grupo = listaGrupos.FirstOrDefault(g => g.Cuenta == cuentaActual);
                listaGrupos.Remove(grupo);

                RefrescarComboGrupos();

                if (grupoCuenta.Items.Count > 0)
                    grupoCuenta.SelectedIndex = grupoCuenta.Items.Count - 1;
            }
        }

        private void RefrescarComboGrupos()
        {
            grupoCuenta.DataSource = null;
            grupoCuenta.DisplayMember = "NombreGrupo";
            grupoCuenta.ValueMember = "Cuenta";
            grupoCuenta.DataSource = listaGrupos.ToList();
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
                if (posicion.Length > 11)
                {
                    posicion = posicion.Insert(11, "-");
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

                rnc.Text = posicion; rnc.SelectionStart = rnc.Text.Length;
            }
        }

        private void tipodoccmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            rnc.Enabled = true;
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

        private void tipoComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comprobantes();
        }

        private void VerComanda_Click(object sender, EventArgs e)
        {
            CargarComanda();

            panelComanda.Visible = true;
            panelComanda.BringToFront();
            panelComanda.Location = new Point(0, 0);
        }

        private void salirComanda_Click(object sender, EventArgs e)
        {
            panelComanda.Visible = false;
            panelComanda.BringToFront();
            panelComanda.Location = new Point(1419, 5);
            NotifComanda();
        }

        private void EntregarOrden_Click(object sender, EventArgs e)
        {
            modoEntregar = true;
            EntregarOrden.BackColor = Color.Gold;
            SiEntrega.Visible = true;
            NoEntrega.Visible = true;
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

        private void detalleorden_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            RecalcularTotales();
        }

        private void detalleorden_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string columna = detalleorden.Columns[e.ColumnIndex].Name;

            if (columna != "precio" && columna != "cantidad")
            {
                e.Cancel = true;
            }
        }

        private void detalleorden_SelectionChanged(object sender, EventArgs e)
        {
            if (detalleorden.SelectedRows.Count > 0)
            {
                var cuenta = detalleorden.SelectedRows[0].Cells["cuenta"].Value;
                grupoCuenta.SelectedItem = cuenta;
            }
        }

        private void passView_CheckedChanged(object sender, EventArgs e)
        {
            if (passView.Checked == true || passView.Checked == true)
            {
                txtpass.UseSystemPasswordChar = false;
                passCancelar.UseSystemPasswordChar = false;
            }
            else
            {
                txtpass.UseSystemPasswordChar = true;
                passCancelar.UseSystemPasswordChar = true;
            }
        }

        private void volverCancelar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Operación Cancelada.");
            cancelarPanel.Visible = false;
            cancelarPanel.Location = new Point(807, 5);
            usuarioCancelar.Clear();
            passCancelar.Clear();
        }

        private void autorizarCancelar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    string queryUsuario = @"
                    SELECT IdUsuario 
                    FROM Usuario 
                    WHERE Login = @usuario AND Contrasena = @pass AND Activo = 1";

                    int idUsuario = 0;

                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conexion))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuarioCancelar.Text);
                        cmd.Parameters.AddWithValue("@pass", passCancelar.Text);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado == null)
                        {
                            MessageBox.Show("Credenciales incorrectas / Usuario Inactivo.");
                            return;
                        }

                        idUsuario = Convert.ToInt32(resultado);
                    }

                    string queryPermiso = @"
                    SELECT CancelarDoc 
                    FROM PermisosUsuario 
                    WHERE IdUsuario = @IdUsuario";

                    bool permisoSi = false;

                    using (SqlCommand cmdPermiso = new SqlCommand(queryPermiso, conexion))
                    {
                        cmdPermiso.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        object permisoResult = cmdPermiso.ExecuteScalar();
                        permisoSi = permisoResult != null && Convert.ToBoolean(permisoResult);
                    }

                    if (!permisoSi)
                    {
                        MessageBox.Show("No tiene permisos para esta operación");
                        return;
                    }

                    string queryEstado = "SELECT Estado FROM Pedido WHERE IdPedido = @id";
                    string estadoActual = "";

                    using (SqlCommand cmd = new SqlCommand(queryEstado, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", PedidoID);
                        estadoActual = (cmd.ExecuteScalar() ?? "").ToString();
                    }

                    if (estadoActual == "Facturado")
                    {
                        MessageBox.Show("No se puede cancelar un pedido que ya está facturado.");
                        return;
                    }

                    int idMesa = 0;
                    int idGrupo = 0;

                    string queryMesa = "SELECT IdMesa FROM Pedido WHERE IdPedido = @id";
                    using (SqlCommand cmd = new SqlCommand(queryMesa, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", PedidoID);
                        idMesa = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string queryGrupo = "SELECT IdGrupo FROM Mesa WHERE IdMesa = @idMesa";
                    using (SqlCommand cmd = new SqlCommand(queryGrupo, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idMesa", idMesa);
                        object grupoResult = cmd.ExecuteScalar();
                        idGrupo = grupoResult != null ? Convert.ToInt32(grupoResult) : 0;
                    }

                    using (SqlTransaction trans = conexion.BeginTransaction())
                    {
                        try
                        {
                            if (antesDe.Checked)
                            {
                                string queryDetalles = @"
                                SELECT IdProducto, Cantidad 
                                FROM DetallePedido 
                                WHERE IdPedido = @id";

                                using (SqlCommand cmd = new SqlCommand(queryDetalles, conexion, trans))
                                {
                                    cmd.Parameters.AddWithValue("@id", PedidoID);

                                    using (SqlDataReader dr = cmd.ExecuteReader())
                                    {
                                        while (dr.Read())
                                        {
                                            int idProd = Convert.ToInt32(dr["IdProducto"]);
                                            decimal cant = Convert.ToDecimal(dr["Cantidad"]);

                                            string queryDevolver = @"
                                            UPDATE ProductoVenta 
                                            SET Existencia = Existencia + @cant
                                            WHERE IdProducto = @idp";

                                            using (SqlCommand cmd2 = new SqlCommand(queryDevolver, conexion, trans))
                                            {
                                                cmd2.Parameters.AddWithValue("@cant", cant);
                                                cmd2.Parameters.AddWithValue("@idp", idProd);
                                                cmd2.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }

                            string queryCancelar = @"
                            UPDATE Pedido 
                            SET Estado = 'Cancelado'
                            WHERE IdPedido = @id";

                            using (SqlCommand cmd = new SqlCommand(queryCancelar, conexion, trans))
                            {
                                cmd.Parameters.AddWithValue("@id", PedidoID);
                                cmd.ExecuteNonQuery();
                            }

                            if (idGrupo > 0)
                            {
                                string liberarGrupoQuery = @"
                                UPDATE Mesa 
                                SET Ocupado = 0
                                WHERE IdGrupo = @grupo";

                                using (SqlCommand cmd = new SqlCommand(liberarGrupoQuery, conexion, trans))
                                {
                                    cmd.Parameters.AddWithValue("@grupo", idGrupo);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string liberarMesaQuery = @"
                                UPDATE Mesa 
                                SET Ocupado = 0
                                WHERE IdMesa = @mesa";

                                using (SqlCommand cmd = new SqlCommand(liberarMesaQuery, conexion, trans))
                                {
                                    cmd.Parameters.AddWithValue("@mesa", idMesa);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            trans.Commit();

                            MessageBox.Show("Pedido cancelado con éxito.");
                            cancelarPanel.Visible = false;
                            tabControl1_SelectedIndexChanged(sender, e);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            MessageBox.Show("Error al cancelar: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error: " + ex.Message);
                }
            }
        }

        private void antesDe_CheckedChanged(object sender, EventArgs e)
        {
            if (antesDe.Checked == true)
            {
                despuesDe.Checked = false;
            }
        }

        private void despuesDe_CheckedChanged(object sender, EventArgs e)
        {
            if (despuesDe.Checked == true)
            {
                antesDe.Checked = false;
            }
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

            if (grupoCuenta.Items.Count > 0)
            {
                int cuenta = Convert.ToInt32(row.Cells["cuenta"].Value);

                if (!cargandoGrupos)
                {
                    grupoCuenta.SelectedValue = cuenta;

                    if (grupoCuenta.SelectedValue == null)
                    {
                        int index = listaGrupos.FindIndex(g => g.Cuenta == cuenta);
                        if (index >= 0)
                            grupoCuenta.SelectedIndex = index;
                    }
                }
            }

            detalleorden.Rows.RemoveAt(e.RowIndex);

            RecalcularTotales();

            bajarproductobtn.Enabled = true;
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

        private void txtprecioproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;

            if (!int.TryParse(txtcodigoproducto.Text, out int idProducto))
            {
                MessageBox.Show("Producto inválido.");
                return;
            }

            if (!decimal.TryParse(txtprecioproducto.Text, out decimal precioVenta))
            {
                MessageBox.Show("Precio inválido.");
                return;
            }

            decimal precioCompra = 0;

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();

                string sql = @"
                SELECT PrecioCompra
                FROM ProductoVenta
                WHERE IdProducto = @IdProducto";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        MessageBox.Show("No se encontró el producto.");
                        return;
                    }

                    precioCompra = Convert.ToDecimal(result);
                }
            }

            if (PrecioMinimo && precioVenta < precioCompra)
            {
                MessageBox.Show(
                    $"El precio de venta no puede ser menor al precio de compra.\n\n" +
                    $"Precio compra: {precioCompra:N2}",
                    "Precio mínimo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtprecioproducto.Text = precioCompra.ToString("N2");
                txtprecioproducto.SelectAll();
                return;
            }

            bajarproductobtn.Focus();
        }

        private void txtprecioproducto_Leave(object sender, EventArgs e)
        {
            txtprecioproducto_KeyDown(sender, new KeyEventArgs(Keys.Enter));
        }

        private void RecargarRestante()
        {
            if(restante1txt.Text == "0.00" || restante2txt.Text == "0.00" || restante3txt.Text == "0.00")
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
                button9.Enabled = false;
            }
            else
            {
                button9.Enabled = true;
            }

            efectivotxt.Text = restante1txt.Text;
            tarjetaMonto.Text = restante2txt.Text;
            transfMonto.Text = restante3txt.Text;
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
    }
}