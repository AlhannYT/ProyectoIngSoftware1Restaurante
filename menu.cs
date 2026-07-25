using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;

namespace Proyecto_restaurante
{
    public partial class menu : Form
    {
        public menu()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }

        string conexionString = ConexionBD.ConexionSQL();

        public string usuarioActual;
        public int IdUsuarioActual;
        string nombrePC = Environment.MachineName;
        public int administrador;
        public int estadobarra = 1;
        public int sistemas = 0;

        [DllImport("user32.dll")]

        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        private void cerrarbtn_Click(object sender, EventArgs e)
        {
            DialogResult salir = MessageBox.Show("¿Desea Cerrar Sesión?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (salir == DialogResult.Yes)
            {
                inicio cerrarsesion = new inicio();
                cerrarsesion.Show();
                this.Close();
            }
        }

        private void CambiarColorMDI(Color color)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MdiClient)
                {
                    ctrl.BackColor = color;
                    break;
                }
            }
        }

        private void menu_Load(object sender, EventArgs e)
        {
            Color colorPanel = Color.Silver;

            button5.Focus();
            sistemas = 0;

            if (cambiarfechapanel.Visible == true)
            {
                cambiarfechapanel.Visible = false;
            }

            try
            {
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();

                    string query = "SELECT ColorPanel FROM Configuracion WHERE NombrePC = @NombrePC";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@NombrePC", nombrePC);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value && !string.IsNullOrWhiteSpace(result.ToString()))
                        {
                            string[] rgb = result.ToString().Split(',');
                            if (rgb.Length == 3 &&
                                int.TryParse(rgb[0].Trim(), out int r) &&
                                int.TryParse(rgb[1].Trim(), out int g) &&
                                int.TryParse(rgb[2].Trim(), out int b))
                            {
                                colorPanel = Color.FromArgb(r, g, b);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el color del menú: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (estadobarra == 1)
            {
                NombrePCtxt.Text = "PC: " + nombrePC.ToString();
            }
            else
            {
                NombrePCtxt.Text = "  ";
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.WindowState = FormWindowState.Maximized;
            oculto.BackColor = colorPanel;

            deslizar.PerformClick();
            this.CambiarColorMDI(colorPanel);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is ConsClientes)
                {
                    f.BringToFront();
                    return;
                }
            }

            ConsClientes consClientes = new ConsClientes();
            consClientes.Location = new Point(200, 50);
            consClientes.MdiParent = this;
            consClientes.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is MantMesas)
                {
                    f.BringToFront();
                    return;
                }
            }
            MantMesas mantMesas = new MantMesas();
            mantMesas.Location = new Point(200, 50);
            mantMesas.MdiParent = this;
            mantMesas.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is ConsProveedor)
                {
                    f.BringToFront();
                    return;
                }
            }
            ConsProveedor mantProv = new ConsProveedor();
            mantProv.Location = new Point(200, 50);
            mantProv.MdiParent = this;
            mantProv.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            foreach (Form f in this.MdiChildren)
            {
                if (f is ConsProductos)
                {
                    f.BringToFront();
                    return;
                }
            }
            ConsProductos ConsProductos = new ConsProductos();
            ConsProductos.Location = new Point(200, 50);
            ConsProductos.MdiParent = this;

            ConsProductos.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is Pedidos)
                {
                    f.BringToFront();
                    return;
                }
            }

            Pedidos pedidos = new Pedidos();
            pedidos.NombrePC = nombrePC;
            pedidos.NombreUsuario = usuarioActual;
            pedidos.IdUsuario = IdUsuarioActual;
            pedidos.Location = new Point(200, 50);
            pedidos.MdiParent = this;
            pedidos.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is Compras)
                {
                    f.BringToFront();
                    return;
                }
            }
            Compras compras = new Compras();
            compras.responsableCompra = usuarioActual;
            compras.Location = new Point(200, 50);
            compras.MdiParent = this;
            compras.Show();
        }


        private void button11_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is Configuracion)
                {
                    f.BringToFront();
                    return;
                }
            }
            Configuracion config = new Configuracion();

            if (administrador == 1)
            {
                config.usuarios.Visible = true;
                config.sistema.Visible = true;
                config.Location = new Point(200, 50);
                config.MdiParent = this;
                config.Show();
            }
            else
            {
                config.usuarios.Visible = false;
                config.sistema.Visible = false;
                config.Location = new Point(200, 50);
                config.MdiParent = this;
                config.Show();
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (estadobarra == 1)
            {
                barraizq.Width = 63;

                foreach (Control ctrl in barraizq.Controls)
                {
                    if (ctrl is Button btn)
                    {
                        btn.Width = 42;
                        btn.Text = "";
                        btn.ImageAlign = ContentAlignment.MiddleCenter;
                        label1.Text = "  ";
                        label2.Text = "  ";
                        label3.Text = "  ";
                        ajustestxt.Text = "  ";
                        NombrePCtxt.Text = "  ";
                        label1.ImageAlign = ContentAlignment.MiddleCenter;
                        label2.ImageAlign = ContentAlignment.MiddleCenter;
                        ajustestxt.ImageAlign = ContentAlignment.MiddleCenter;
                        toolTip1.SetToolTip(button5, "Articulos");
                        toolTip1.SetToolTip(button1, "Pedidos (Delivery)");
                        toolTip1.SetToolTip(button8, "Mesas");
                        toolTip1.SetToolTip(button2, "Clientes");
                        toolTip1.SetToolTip(button6, "Proveedores");
                        toolTip1.SetToolTip(button10, "Ordenes (Local)");
                        toolTip1.SetToolTip(button9, "Compras");
                        toolTip1.SetToolTip(reservacion, "Reservacion");
                        toolTip1.SetToolTip(button11, "Generales");
                        toolTip1.SetToolTip(button13, "Empleados");
                        toolTip1.SetToolTip(reportesbtn, "Reportes");
                        toolTip1.SetToolTip(button14, "Extras...");
                    }
                }

                cambiarfechapanel.Location = new Point(489, 73);

                estadobarra = 0;
            }
            else
            {
                barraizq.Width = 241;

                foreach (Control ctrl in barraizq.Controls)
                {
                    if (ctrl is Button btn)
                    {
                        btn.Width = 217;
                        button12.Width = 42;
                        button5.Text = "Articulos";
                        button1.Text = "Pedidos (Delivery)";
                        button8.Text = "Mesas";
                        button2.Text = "Clientes";
                        button6.Text = "Proveedores";
                        button10.Text = "Ordenes (Local)";
                        button14.Text = "Extras...";
                        button9.Text = "Compras";
                        reservacion.Text = "Reservacion";
                        button11.Text = "Generales";
                        button13.Text = "Empleados";
                        reportesbtn.Text = "Reportes";
                        NombrePCtxt.Text = "PC: " + nombrePC;
                        btn.ImageAlign = ContentAlignment.MiddleRight;
                        button12.ImageAlign = ContentAlignment.MiddleCenter;
                        label1.Text = "Mantenimientos    ";
                        label2.Text = "Procesos    ";
                        label3.Text = "Informes    ";
                        ajustestxt.Text = "Ajustes    ";
                        label1.ImageAlign = ContentAlignment.MiddleRight;
                        label2.ImageAlign = ContentAlignment.MiddleRight;
                        ajustestxt.ImageAlign = ContentAlignment.MiddleRight;
                    }
                }

                cambiarfechapanel.Location = new Point(667, 73);

                estadobarra = 1;
            }
        }

        private void reservacion_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is Reservacion)
                {
                    f.BringToFront();
                    return;
                }
            }
            Reservacion reservacion = new Reservacion();
            reservacion.Location = new Point(200, 50);
            reservacion.MdiParent = this;
            reservacion.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is MenuTipos)
                {
                    f.BringToFront();
                    return;
                }
            }
            MenuTipos menuTipos = new MenuTipos();
            menuTipos.Location = new Point(200, 50);
            menuTipos.textoinicial.Location = new Point(396, 150);
            menuTipos.textoinicial.Visible = true;
            menuTipos.MdiParent = this;
            menuTipos.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is ConsEmpleados)
                {
                    f.BringToFront();
                    return;
                }
            }
            ConsEmpleados empleados = new ConsEmpleados();
            empleados.Location = new Point(200, 50);
            empleados.MdiParent = this;
            empleados.Show();
        }

        private void abrirTablet_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is TabletSistema)
                {
                    f.BringToFront();
                    return;
                }
            }
            TabletSistema tablet = new TabletSistema();
            tablet.IdUsuario = IdUsuarioActual;
            tablet.NombreUsuario = usuarioActual;
            tablet.Location = new Point(200, 50);
            tablet.MdiParent = this;
            tablet.Show();
        }

        private void abrirtTV_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is TelevisionSistema)
                {
                    f.BringToFront();
                    return;
                }
            }
            TelevisionSistema TV = new TelevisionSistema();
            TV.Location = new Point(200, 50);
            TV.Show();
        }

        private void recargarbtn_Click(object sender, EventArgs e)
        {
            menu_Load(sender, e);
            recargarbtn.Visible = false;
        }

        private void oculto_MouseHover(object sender, EventArgs e)
        {
            recargarbtn.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is Delivery)
                {
                    f.BringToFront();
                    return;
                }
            }
            Delivery delivery = new Delivery();
            delivery.Location = new Point(200, 50);
            delivery.NombrePC = nombrePC;
            delivery.MdiParent = this;
            delivery.Show();
        }

        private void menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F)
            {
                button10_Click(sender, e);
                e.SuppressKeyPress = true;
            }

            if (e.Alt && e.KeyCode == Keys.R)
            {
                reservacion_Click(sender, e);
                e.SuppressKeyPress = true;
            }

            if (e.Alt && e.KeyCode == Keys.D)
            {
                button1_Click(sender, e);
                e.SuppressKeyPress = true;
            }

            if (e.Alt && e.KeyCode == Keys.C)
            {
                button9_Click(sender, e);
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.C)
            {
                button12_Click(sender, e);
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                //cerrarbtn_Click(sender, e);
            }

            if (e.KeyCode == Keys.F5)
            {
                e.SuppressKeyPress = true;
                RecargarMenu();
            }
        }

        private void RecargarMenu()
        {
            menu_Load(null, null);
            MessageBox.Show("Sistema Recargado Exitosamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void deslizar_Click(object sender, EventArgs e)
        {
            if (sistemas == 0)
            {
                deslizar.Image = Proyecto_restaurante.Properties.Resources.flechaizquierdaroja;
                sistemasPanel.Visible = false;
                sistemas = 1;
            }
            else
            {
                deslizar.Image = Proyecto_restaurante.Properties.Resources.flechaderecharoja;
                sistemasPanel.Visible = true;
                sistemas = 0;
            }
        }

        private void horatimer_Tick(object sender, EventArgs e)
        {
            labelhora.Text = DateTime.Now.ToString("h:mm:ss tt");

            labelfecha.Text = SistemaFecha.FechaActual.ToLongDateString() + "    ";
            labelcambiofecha.Text = cambiarFechaDTP.Value.ToLongDateString();
        }

        private void AbrirCalendario(DateTimePicker dtp)
        {
            int x = dtp.Width - 10;
            int y = dtp.Height / 2;
            int lParam = (y << 16) | (x & 0xFFFF);

            SendMessage(dtp.Handle, WM_LBUTTONDOWN, 1, lParam);
            SendMessage(dtp.Handle, WM_LBUTTONUP, 1, lParam);
        }

        private void labelfecha_Click(object sender, EventArgs e)
        {
            if(estadobarra == 1)
            {
                cambiarfechapanel.Location = new Point(667, 73);
                cambiarfechapanel.Visible = !cambiarfechapanel.Visible;
            }
            else
            {
                cambiarfechapanel.Location = new Point(489, 73);
                cambiarfechapanel.Visible = !cambiarfechapanel.Visible;
            }
        }

        private void cambiarfechapanel_VisibleChanged(object sender, EventArgs e)
        {
            if (cambiarfechapanel.Visible)
            {
                cambiarFechaDTP.Value = SistemaFecha.FechaActual;

                this.BeginInvoke((MethodInvoker)(() =>
                {
                    cambiarFechaDTP.Focus();
                    AbrirCalendario(cambiarFechaDTP);
                }));

                barraizq.Enabled = false;
            }
            else
            {
                barraizq.Enabled = true;
            }
        }

        public static class SistemaFecha
        {
            public static DateTime FechaActual { get; private set; } = DateTime.Today;

            public static void CambiarFecha(DateTime nuevaFecha)
            {
                FechaActual = nuevaFecha.Date;
            }
        }

        private void reportesbtn_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is Reportes)
                {
                    f.BringToFront();
                    return;
                }
            }
            Reportes reportes = new Reportes();
            reportes.Location = new Point(200, 50);
            reportes.MdiParent = this;
            reportes.Show();
        }

        private void menu_Click(object sender, EventArgs e)
        {
            if (cambiarfechapanel.Visible == true)
            {
                cambiarfechapanel.Visible = false;
                barraizq.Enabled = true;
            }
        }

        private void cambiarFechaBtn_Click(object sender, EventArgs e)
        {
            SistemaFecha.CambiarFecha(cambiarFechaDTP.Value);

            labelfecha.Text = SistemaFecha.FechaActual.ToLongDateString();
            labelcambiofecha.Text = SistemaFecha.FechaActual.ToLongDateString();

            cambiarfechapanel.Visible = false;

            MessageBox.Show("Cambio de fecha exitoso!", "Fecha Cambiada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            labelfecha_Click(sender, e);
            barraizq.Enabled = true;
        }
    }
}