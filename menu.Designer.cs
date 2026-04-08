namespace Proyecto_restaurante
{
    partial class menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(menu));
            usuariolabel = new Label();
            panel1 = new Panel();
            cerrarbtn = new Button();
            panel5 = new Panel();
            panel2 = new Panel();
            pictureBox2 = new PictureBox();
            button2 = new Button();
            button5 = new Button();
            button6 = new Button();
            button8 = new Button();
            label1 = new Label();
            button9 = new Button();
            button10 = new Button();
            label2 = new Label();
            barrasup = new Panel();
            panel6 = new Panel();
            label7 = new Label();
            labelfecha = new Label();
            label6 = new Label();
            labelhora = new Label();
            pictureBox1 = new PictureBox();
            barraizq = new Panel();
            panel4 = new Panel();
            NombrePCtxt = new Label();
            button12 = new Button();
            reservacion = new Button();
            button11 = new Button();
            ajustestxt = new Label();
            label3 = new Label();
            button14 = new Button();
            button13 = new Button();
            reportesbtn = new Button();
            button1 = new Button();
            creditoslabel = new Label();
            abrirtTV = new Button();
            abrirTablet = new Button();
            label4 = new Label();
            label5 = new Label();
            oculto = new Panel();
            recargarbtn = new Button();
            toolTip1 = new ToolTip(components);
            deslizar = new Button();
            panel3 = new Panel();
            label37 = new Label();
            sistemasPanel = new Panel();
            horatimer = new System.Windows.Forms.Timer(components);
            cambiarfechapanel = new Panel();
            cambiarFechaDTP = new DateTimePicker();
            cambiarFechaBtn = new Button();
            labelcambiofecha = new Label();
            button3 = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            barrasup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            barraizq.SuspendLayout();
            panel4.SuspendLayout();
            oculto.SuspendLayout();
            panel3.SuspendLayout();
            sistemasPanel.SuspendLayout();
            cambiarfechapanel.SuspendLayout();
            SuspendLayout();
            // 
            // usuariolabel
            // 
            usuariolabel.AutoSize = true;
            usuariolabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            usuariolabel.Location = new Point(2, 2);
            usuariolabel.Margin = new Padding(2, 0, 2, 0);
            usuariolabel.Name = "usuariolabel";
            usuariolabel.Size = new Size(145, 21);
            usuariolabel.TabIndex = 2;
            usuariolabel.Text = "USUARIO ACTUAL: ";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.BackColor = Color.White;
            panel1.Controls.Add(cerrarbtn);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(usuariolabel);
            panel1.Location = new Point(705, 4);
            panel1.Margin = new Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(318, 60);
            panel1.TabIndex = 3;
            // 
            // cerrarbtn
            // 
            cerrarbtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cerrarbtn.BackColor = Color.Red;
            cerrarbtn.Cursor = Cursors.Hand;
            cerrarbtn.FlatStyle = FlatStyle.Flat;
            cerrarbtn.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cerrarbtn.Image = Properties.Resources.salida;
            cerrarbtn.Location = new Point(207, -22);
            cerrarbtn.Margin = new Padding(2, 3, 2, 3);
            cerrarbtn.Name = "cerrarbtn";
            cerrarbtn.Size = new Size(113, 85);
            cerrarbtn.TabIndex = 0;
            cerrarbtn.TabStop = false;
            cerrarbtn.Text = "Cerrar Sesión";
            cerrarbtn.TextAlign = ContentAlignment.BottomCenter;
            cerrarbtn.UseVisualStyleBackColor = false;
            cerrarbtn.Click += cerrarbtn_Click;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Lime;
            panel5.Location = new Point(-1, 51);
            panel5.Margin = new Padding(2, 3, 2, 3);
            panel5.Name = "panel5";
            panel5.Size = new Size(211, 6);
            panel5.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel2.BackColor = Color.FromArgb(0, 192, 0);
            panel2.Controls.Add(pictureBox2);
            panel2.Location = new Point(636, 4);
            panel2.Margin = new Padding(2, 3, 2, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(70, 61);
            panel2.TabIndex = 4;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.perfil_blanco;
            pictureBox2.Location = new Point(2, 2);
            pictureBox2.Margin = new Padding(2, 3, 2, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(66, 57);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            // 
            // button2
            // 
            button2.Cursor = Cursors.Hand;
            button2.Image = Properties.Resources.cliente31;
            button2.ImageAlign = ContentAlignment.MiddleRight;
            button2.Location = new Point(9, 180);
            button2.Margin = new Padding(2);
            button2.Name = "button2";
            button2.Size = new Size(217, 38);
            button2.TabIndex = 4;
            button2.Text = "Clientes";
            button2.TextAlign = ContentAlignment.MiddleLeft;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button5
            // 
            button5.Cursor = Cursors.Hand;
            button5.Image = Properties.Resources.productonuevo1;
            button5.ImageAlign = ContentAlignment.MiddleRight;
            button5.Location = new Point(9, 104);
            button5.Margin = new Padding(2);
            button5.Name = "button5";
            button5.Size = new Size(217, 38);
            button5.TabIndex = 2;
            button5.Text = "Articulos";
            button5.TextAlign = ContentAlignment.MiddleLeft;
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Cursor = Cursors.Hand;
            button6.Image = Properties.Resources.proveedor2;
            button6.ImageAlign = ContentAlignment.MiddleRight;
            button6.Location = new Point(9, 218);
            button6.Margin = new Padding(2);
            button6.Name = "button6";
            button6.Size = new Size(217, 38);
            button6.TabIndex = 8;
            button6.Text = "Proveedores";
            button6.TextAlign = ContentAlignment.MiddleLeft;
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button8
            // 
            button8.Cursor = Cursors.Hand;
            button8.Image = Properties.Resources.mesa;
            button8.ImageAlign = ContentAlignment.MiddleRight;
            button8.Location = new Point(9, 142);
            button8.Margin = new Padding(2);
            button8.Name = "button8";
            button8.Size = new Size(217, 38);
            button8.TabIndex = 3;
            button8.Text = "Mesas";
            button8.TextAlign = ContentAlignment.MiddleLeft;
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Image = Properties.Resources.mantenimiento1;
            label1.ImageAlign = ContentAlignment.MiddleRight;
            label1.Location = new Point(5, 66);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(229, 32);
            label1.TabIndex = 14;
            label1.Text = "Mantenimientos    ";
            // 
            // button9
            // 
            button9.Anchor = AnchorStyles.Left;
            button9.Cursor = Cursors.Hand;
            button9.Image = Properties.Resources.carrito32x;
            button9.ImageAlign = ContentAlignment.MiddleRight;
            button9.Location = new Point(9, 509);
            button9.Margin = new Padding(2);
            button9.Name = "button9";
            button9.Size = new Size(217, 38);
            button9.TabIndex = 11;
            button9.Text = "Compras";
            button9.TextAlign = ContentAlignment.MiddleLeft;
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Left;
            button10.Cursor = Cursors.Hand;
            button10.Image = Properties.Resources.recepcion;
            button10.ImageAlign = ContentAlignment.MiddleRight;
            button10.Location = new Point(9, 395);
            button10.Margin = new Padding(2);
            button10.Name = "button10";
            button10.Size = new Size(217, 38);
            button10.TabIndex = 10;
            button10.Text = "Ordenes (Local)";
            button10.TextAlign = ContentAlignment.MiddleLeft;
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Image = Properties.Resources.proceso;
            label2.ImageAlign = ContentAlignment.MiddleRight;
            label2.Location = new Point(5, 355);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(144, 32);
            label2.TabIndex = 17;
            label2.Text = "Procesos    ";
            // 
            // barrasup
            // 
            barrasup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            barrasup.BackColor = Color.FromArgb(64, 64, 64);
            barrasup.BorderStyle = BorderStyle.FixedSingle;
            barrasup.Controls.Add(panel6);
            barrasup.Controls.Add(label7);
            barrasup.Controls.Add(labelfecha);
            barrasup.Controls.Add(label6);
            barrasup.Controls.Add(labelhora);
            barrasup.Controls.Add(panel1);
            barrasup.Controls.Add(panel2);
            barrasup.Controls.Add(pictureBox1);
            barrasup.Dock = DockStyle.Top;
            barrasup.Location = new Point(239, 0);
            barrasup.Margin = new Padding(2);
            barrasup.Name = "barrasup";
            barrasup.Size = new Size(1027, 71);
            barrasup.TabIndex = 18;
            // 
            // panel6
            // 
            panel6.BackColor = SystemColors.WindowFrame;
            panel6.Location = new Point(329, 7);
            panel6.Name = "panel6";
            panel6.Size = new Size(4, 55);
            panel6.TabIndex = 8;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.White;
            label7.Location = new Point(344, 35);
            label7.Name = "label7";
            label7.Size = new Size(75, 30);
            label7.TabIndex = 7;
            label7.Text = "Fecha:";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelfecha
            // 
            labelfecha.AutoSize = true;
            labelfecha.BackColor = Color.Gray;
            labelfecha.Cursor = Cursors.Hand;
            labelfecha.Font = new Font("Segoe UI", 15.75F);
            labelfecha.ForeColor = Color.White;
            labelfecha.Image = Properties.Resources.angulo_abajo_blanco;
            labelfecha.ImageAlign = ContentAlignment.MiddleRight;
            labelfecha.Location = new Point(425, 35);
            labelfecha.Name = "labelfecha";
            labelfecha.Size = new Size(91, 30);
            labelfecha.TabIndex = 7;
            labelfecha.Text = "Fecha    ";
            labelfecha.TextAlign = ContentAlignment.MiddleLeft;
            toolTip1.SetToolTip(labelfecha, "Cambiar Fecha");
            labelfecha.Click += labelfecha_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.White;
            label6.Location = new Point(344, 2);
            label6.Name = "label6";
            label6.Size = new Size(67, 30);
            label6.TabIndex = 7;
            label6.Text = "Hora:";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelhora
            // 
            labelhora.AutoSize = true;
            labelhora.Font = new Font("Segoe UI", 15.75F);
            labelhora.ForeColor = Color.White;
            labelhora.Location = new Point(425, 2);
            labelhora.Name = "labelhora";
            labelhora.Size = new Size(58, 30);
            labelhora.TabIndex = 7;
            labelhora.Text = "Hora";
            labelhora.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Dock = DockStyle.Left;
            pictureBox1.Image = Properties.Resources.logo_completo;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(326, 69);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // barraizq
            // 
            barraizq.BackColor = SystemColors.WindowFrame;
            barraizq.BorderStyle = BorderStyle.FixedSingle;
            barraizq.Controls.Add(panel4);
            barraizq.Controls.Add(button12);
            barraizq.Controls.Add(reservacion);
            barraizq.Controls.Add(button11);
            barraizq.Controls.Add(ajustestxt);
            barraizq.Controls.Add(label1);
            barraizq.Controls.Add(label3);
            barraizq.Controls.Add(label2);
            barraizq.Controls.Add(button14);
            barraizq.Controls.Add(button13);
            barraizq.Controls.Add(button2);
            barraizq.Controls.Add(reportesbtn);
            barraizq.Controls.Add(button9);
            barraizq.Controls.Add(button1);
            barraizq.Controls.Add(button10);
            barraizq.Controls.Add(button8);
            barraizq.Controls.Add(button5);
            barraizq.Controls.Add(button6);
            barraizq.Dock = DockStyle.Left;
            barraizq.Location = new Point(0, 0);
            barraizq.Margin = new Padding(2);
            barraizq.Name = "barraizq";
            barraizq.Size = new Size(239, 839);
            barraizq.TabIndex = 19;
            // 
            // panel4
            // 
            panel4.BackColor = Color.White;
            panel4.Controls.Add(NombrePCtxt);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(237, 23);
            panel4.TabIndex = 20;
            // 
            // NombrePCtxt
            // 
            NombrePCtxt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            NombrePCtxt.AutoSize = true;
            NombrePCtxt.BackColor = Color.Transparent;
            NombrePCtxt.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            NombrePCtxt.ForeColor = Color.Black;
            NombrePCtxt.Location = new Point(2, -76);
            NombrePCtxt.Margin = new Padding(2, 0, 2, 0);
            NombrePCtxt.Name = "NombrePCtxt";
            NombrePCtxt.Size = new Size(38, 21);
            NombrePCtxt.TabIndex = 2;
            NombrePCtxt.Text = "PC: ";
            // 
            // button12
            // 
            button12.Cursor = Cursors.Hand;
            button12.Image = Properties.Resources._3_rayas;
            button12.Location = new Point(9, 35);
            button12.Name = "button12";
            button12.Size = new Size(42, 29);
            button12.TabIndex = 1;
            button12.UseVisualStyleBackColor = true;
            button12.Click += button12_Click;
            // 
            // reservacion
            // 
            reservacion.Anchor = AnchorStyles.Left;
            reservacion.Cursor = Cursors.Hand;
            reservacion.Image = Properties.Resources.calendario_reloj;
            reservacion.ImageAlign = ContentAlignment.MiddleRight;
            reservacion.Location = new Point(9, 471);
            reservacion.Margin = new Padding(2);
            reservacion.Name = "reservacion";
            reservacion.Size = new Size(217, 38);
            reservacion.TabIndex = 12;
            reservacion.Text = "Reservación";
            reservacion.TextAlign = ContentAlignment.MiddleLeft;
            reservacion.UseVisualStyleBackColor = true;
            reservacion.Click += reservacion_Click;
            // 
            // button11
            // 
            button11.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button11.Cursor = Cursors.Hand;
            button11.Image = Properties.Resources.ajuste;
            button11.ImageAlign = ContentAlignment.MiddleRight;
            button11.Location = new Point(9, 784);
            button11.Margin = new Padding(2);
            button11.Name = "button11";
            button11.Size = new Size(217, 38);
            button11.TabIndex = 13;
            button11.Text = "Generales";
            button11.TextAlign = ContentAlignment.MiddleLeft;
            button11.UseVisualStyleBackColor = true;
            button11.Click += button11_Click;
            // 
            // ajustestxt
            // 
            ajustestxt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ajustestxt.AutoSize = true;
            ajustestxt.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ajustestxt.ForeColor = Color.White;
            ajustestxt.Image = Properties.Resources.opciones;
            ajustestxt.ImageAlign = ContentAlignment.MiddleRight;
            ajustestxt.Location = new Point(5, 742);
            ajustestxt.Margin = new Padding(2, 0, 2, 0);
            ajustestxt.Name = "ajustestxt";
            ajustestxt.Size = new Size(125, 32);
            ajustestxt.TabIndex = 19;
            ajustestxt.Text = "Ajustes    ";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Image = Properties.Resources.formularios1;
            label3.ImageAlign = ContentAlignment.MiddleRight;
            label3.Location = new Point(5, 566);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(145, 32);
            label3.TabIndex = 17;
            label3.Text = "Informes    ";
            // 
            // button14
            // 
            button14.Cursor = Cursors.Hand;
            button14.Image = Properties.Resources.tipos;
            button14.ImageAlign = ContentAlignment.MiddleRight;
            button14.Location = new Point(9, 294);
            button14.Margin = new Padding(2);
            button14.Name = "button14";
            button14.Size = new Size(217, 38);
            button14.TabIndex = 4;
            button14.Text = "Tipos";
            button14.TextAlign = ContentAlignment.MiddleLeft;
            button14.UseVisualStyleBackColor = true;
            button14.Click += button14_Click;
            // 
            // button13
            // 
            button13.Cursor = Cursors.Hand;
            button13.Image = Properties.Resources.empleado;
            button13.ImageAlign = ContentAlignment.MiddleRight;
            button13.Location = new Point(9, 256);
            button13.Margin = new Padding(2);
            button13.Name = "button13";
            button13.Size = new Size(217, 38);
            button13.TabIndex = 4;
            button13.Text = "Empleados";
            button13.TextAlign = ContentAlignment.MiddleLeft;
            button13.UseVisualStyleBackColor = true;
            button13.Click += button13_Click;
            // 
            // reportesbtn
            // 
            reportesbtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            reportesbtn.Cursor = Cursors.Hand;
            reportesbtn.Image = Properties.Resources.pedido;
            reportesbtn.ImageAlign = ContentAlignment.MiddleRight;
            reportesbtn.Location = new Point(9, 606);
            reportesbtn.Margin = new Padding(2);
            reportesbtn.Name = "reportesbtn";
            reportesbtn.Size = new Size(217, 38);
            reportesbtn.TabIndex = 10;
            reportesbtn.Text = "Reportes";
            reportesbtn.TextAlign = ContentAlignment.MiddleLeft;
            reportesbtn.UseVisualStyleBackColor = true;
            reportesbtn.Click += reportesbtn_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Left;
            button1.Cursor = Cursors.Hand;
            button1.Image = Properties.Resources.delivery1;
            button1.ImageAlign = ContentAlignment.MiddleRight;
            button1.Location = new Point(9, 433);
            button1.Margin = new Padding(2);
            button1.Name = "button1";
            button1.Size = new Size(217, 38);
            button1.TabIndex = 10;
            button1.Text = "Pedidos (Delivery)";
            button1.TextAlign = ContentAlignment.MiddleLeft;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // creditoslabel
            // 
            creditoslabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            creditoslabel.AutoSize = true;
            creditoslabel.BackColor = Color.Transparent;
            creditoslabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            creditoslabel.Location = new Point(689, -73);
            creditoslabel.Margin = new Padding(2, 0, 2, 0);
            creditoslabel.Name = "creditoslabel";
            creditoslabel.Size = new Size(331, 21);
            creditoslabel.TabIndex = 2;
            creditoslabel.Text = "Wilder (2-21-0179) / Alhann (2-21-0018) ©";
            // 
            // abrirtTV
            // 
            abrirtTV.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            abrirtTV.Cursor = Cursors.Hand;
            abrirtTV.Image = Properties.Resources.tv;
            abrirtTV.Location = new Point(129, 48);
            abrirtTV.Margin = new Padding(2);
            abrirtTV.Name = "abrirtTV";
            abrirtTV.Size = new Size(44, 38);
            abrirtTV.TabIndex = 0;
            abrirtTV.TextAlign = ContentAlignment.MiddleLeft;
            abrirtTV.UseVisualStyleBackColor = true;
            abrirtTV.Click += abrirtTV_Click;
            // 
            // abrirTablet
            // 
            abrirTablet.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            abrirTablet.Cursor = Cursors.Hand;
            abrirTablet.Image = Properties.Resources.tablet;
            abrirTablet.Location = new Point(129, 4);
            abrirTablet.Margin = new Padding(2);
            abrirTablet.Name = "abrirTablet";
            abrirTablet.Size = new Size(44, 38);
            abrirTablet.TabIndex = 0;
            abrirTablet.TextAlign = ContentAlignment.MiddleLeft;
            abrirTablet.UseVisualStyleBackColor = true;
            abrirTablet.Click += abrirTablet_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(64, 64, 64);
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(32, 57);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(94, 21);
            label4.TabIndex = 2;
            label4.Text = "Sistema TV";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(64, 64, 64);
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.White;
            label5.Location = new Point(5, 13);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(121, 21);
            label5.TabIndex = 2;
            label5.Text = "Sistema Tablet";
            // 
            // oculto
            // 
            oculto.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            oculto.Controls.Add(recargarbtn);
            oculto.Location = new Point(1201, 74);
            oculto.Name = "oculto";
            oculto.Size = new Size(62, 50);
            oculto.TabIndex = 21;
            oculto.MouseHover += oculto_MouseHover;
            // 
            // recargarbtn
            // 
            recargarbtn.Image = Properties.Resources.actualizar;
            recargarbtn.Location = new Point(29, 3);
            recargarbtn.Name = "recargarbtn";
            recargarbtn.Size = new Size(29, 29);
            recargarbtn.TabIndex = 57;
            recargarbtn.TabStop = false;
            recargarbtn.UseVisualStyleBackColor = true;
            recargarbtn.Visible = false;
            recargarbtn.Click += recargarbtn_Click;
            // 
            // deslizar
            // 
            deslizar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            deslizar.Image = Properties.Resources.flechaizquierdaroja;
            deslizar.Location = new Point(1230, 722);
            deslizar.Name = "deslizar";
            deslizar.Size = new Size(33, 89);
            deslizar.TabIndex = 26;
            toolTip1.SetToolTip(deslizar, "Sistemas de simulación");
            deslizar.UseVisualStyleBackColor = true;
            deslizar.Click += deslizar_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(creditoslabel);
            panel3.Controls.Add(label37);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(239, 813);
            panel3.Name = "panel3";
            panel3.Size = new Size(1027, 26);
            panel3.TabIndex = 23;
            // 
            // label37
            // 
            label37.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label37.AutoSize = true;
            label37.Font = new Font("Segoe UI", 12F);
            label37.ForeColor = Color.Black;
            label37.Image = Properties.Resources.teclado__1_;
            label37.ImageAlign = ContentAlignment.MiddleLeft;
            label37.Location = new Point(4, -73);
            label37.Name = "label37";
            label37.Size = new Size(646, 21);
            label37.TabIndex = 75;
            label37.Text = "      : F5: Recargar Menu, Alt+F: Ordenes, Alt+D: Pedidos, Alt+R: Reservacion, Alt+C: Compras";
            // 
            // sistemasPanel
            // 
            sistemasPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sistemasPanel.BackColor = Color.FromArgb(64, 64, 64);
            sistemasPanel.Controls.Add(abrirTablet);
            sistemasPanel.Controls.Add(label4);
            sistemasPanel.Controls.Add(label5);
            sistemasPanel.Controls.Add(abrirtTV);
            sistemasPanel.Location = new Point(1053, 722);
            sistemasPanel.Name = "sistemasPanel";
            sistemasPanel.Size = new Size(177, 90);
            sistemasPanel.TabIndex = 25;
            sistemasPanel.Visible = false;
            // 
            // horatimer
            // 
            horatimer.Enabled = true;
            horatimer.Tick += horatimer_Tick;
            // 
            // cambiarfechapanel
            // 
            cambiarfechapanel.BackColor = Color.Gray;
            cambiarfechapanel.Controls.Add(cambiarFechaDTP);
            cambiarfechapanel.Controls.Add(cambiarFechaBtn);
            cambiarfechapanel.Controls.Add(labelcambiofecha);
            cambiarfechapanel.Controls.Add(button3);
            cambiarfechapanel.Location = new Point(667, 73);
            cambiarfechapanel.Name = "cambiarfechapanel";
            cambiarfechapanel.Size = new Size(258, 250);
            cambiarfechapanel.TabIndex = 28;
            cambiarfechapanel.Visible = false;
            cambiarfechapanel.VisibleChanged += cambiarfechapanel_VisibleChanged;
            // 
            // cambiarFechaDTP
            // 
            cambiarFechaDTP.Anchor = AnchorStyles.Top;
            cambiarFechaDTP.Checked = false;
            cambiarFechaDTP.Format = DateTimePickerFormat.Short;
            cambiarFechaDTP.Location = new Point(8, 6);
            cambiarFechaDTP.Name = "cambiarFechaDTP";
            cambiarFechaDTP.Size = new Size(242, 23);
            cambiarFechaDTP.TabIndex = 0;
            // 
            // cambiarFechaBtn
            // 
            cambiarFechaBtn.Cursor = Cursors.Hand;
            cambiarFechaBtn.Image = Properties.Resources.cambiar_fecha;
            cambiarFechaBtn.ImageAlign = ContentAlignment.MiddleRight;
            cambiarFechaBtn.Location = new Point(134, 205);
            cambiarFechaBtn.Margin = new Padding(2);
            cambiarFechaBtn.Name = "cambiarFechaBtn";
            cambiarFechaBtn.Size = new Size(116, 38);
            cambiarFechaBtn.TabIndex = 2;
            cambiarFechaBtn.Text = "Cambiar fecha";
            cambiarFechaBtn.TextAlign = ContentAlignment.MiddleLeft;
            cambiarFechaBtn.UseVisualStyleBackColor = true;
            cambiarFechaBtn.Click += cambiarFechaBtn_Click;
            // 
            // labelcambiofecha
            // 
            labelcambiofecha.BackColor = Color.Transparent;
            labelcambiofecha.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelcambiofecha.ForeColor = Color.White;
            labelcambiofecha.Location = new Point(8, 81);
            labelcambiofecha.Name = "labelcambiofecha";
            labelcambiofecha.Size = new Size(242, 103);
            labelcambiofecha.TabIndex = 7;
            labelcambiofecha.Text = "Fecha de cambio";
            // 
            // button3
            // 
            button3.Cursor = Cursors.Hand;
            button3.Image = Properties.Resources.atrás;
            button3.ImageAlign = ContentAlignment.MiddleRight;
            button3.Location = new Point(8, 205);
            button3.Margin = new Padding(2);
            button3.Name = "button3";
            button3.Size = new Size(116, 38);
            button3.TabIndex = 2;
            button3.Text = "Volver";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // menu
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Silver;
            BackgroundImage = Properties.Resources.tenedor1;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(1266, 839);
            Controls.Add(cambiarfechapanel);
            Controls.Add(deslizar);
            Controls.Add(sistemasPanel);
            Controls.Add(panel3);
            Controls.Add(oculto);
            Controls.Add(barrasup);
            Controls.Add(barraizq);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            KeyPreview = true;
            Margin = new Padding(2, 3, 2, 3);
            Name = "menu";
            Text = "Menu";
            WindowState = FormWindowState.Maximized;
            Load += menu_Load;
            Click += menu_Click;
            KeyDown += menu_KeyDown;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            barrasup.ResumeLayout(false);
            barrasup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            barraizq.ResumeLayout(false);
            barraizq.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            oculto.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            sistemasPanel.ResumeLayout(false);
            sistemasPanel.PerformLayout();
            cambiarfechapanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        public Label usuariolabel;
        private Panel panel2;
        private Button cerrarbtn;
        private PictureBox pictureBox2;
        private Button button2;
        private Button button5;
        private Button button6;
        private Button button8;
        private Label label1;
        private Button button9;
        private Button button10;
        private Label label2;
        public Label ajustestxt;
        public Button button11;
        public Panel panel1;
        public Panel panel5;
        private Button reservacion;
        public Panel barrasup;
        public Panel barraizq;
        public Label creditoslabel;
        private Button button12;
        private Button button13;
        private Button button14;
        private PictureBox pictureBox1;
        private Label label3;
        private Button reportesbtn;
        public Button abrirtTV;
        public Button abrirTablet;
        public Label label4;
        public Label label5;
        private Panel oculto;
        public Button recargarbtn;
        private Button button1;
        private ToolTip toolTip1;
        private Panel panel3;
        public Label NombrePCtxt;
        private Panel sistemasPanel;
        private Button deslizar;
        private Label label37;
        private Panel panel4;
        private Label labelfecha;
        private Label labelhora;
        private System.Windows.Forms.Timer horatimer;
        private Label label7;
        private Label label6;
        private Panel panel6;
        private Panel cambiarfechapanel;
        private DateTimePicker cambiarFechaDTP;
        private Button cambiarFechaBtn;
        private Button button3;
        private Label labelcambiofecha;
    }
}