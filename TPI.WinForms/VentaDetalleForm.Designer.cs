namespace TPI.WinForms
{
    partial class VentaDetalleForm
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
            lblNroVenta = new Label();
            lblComprador = new Label();
            cmbMetodoPago = new ComboBox();
            lblEstadoTitulo = new Label();
            cmbEstado = new ComboBox();
            pnlAgregarLinea = new Panel();
            label5 = new Label();
            label4 = new Label();
            btnAgregarLinea = new Button();
            numCantidad = new NumericUpDown();
            cmbProducto = new ComboBox();
            dgvDetalles = new DataGridView();
            btnQuitarLinea = new Button();
            lblTotal = new Label();
            btnGuardar = new Button();
            btnCancelar = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            pnlAgregarLinea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvDetalles).BeginInit();
            SuspendLayout();
            // 
            // lblNroVenta
            // 
            lblNroVenta.AutoSize = true;
            lblNroVenta.Location = new Point(96, 9);
            lblNroVenta.Name = "lblNroVenta";
            lblNroVenta.Size = new Size(38, 15);
            lblNroVenta.TabIndex = 0;
            lblNroVenta.Text = "label1";
            // 
            // lblComprador
            // 
            lblComprador.AutoSize = true;
            lblComprador.Location = new Point(245, 9);
            lblComprador.Name = "lblComprador";
            lblComprador.Size = new Size(38, 15);
            lblComprador.TabIndex = 1;
            lblComprador.Text = "label1";
            // 
            // cmbMetodoPago
            // 
            cmbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMetodoPago.FormattingEnabled = true;
            cmbMetodoPago.Location = new Point(431, 6);
            cmbMetodoPago.Name = "cmbMetodoPago";
            cmbMetodoPago.Size = new Size(121, 23);
            cmbMetodoPago.TabIndex = 2;
            // 
            // lblEstadoTitulo
            // 
            lblEstadoTitulo.AutoSize = true;
            lblEstadoTitulo.Location = new Point(612, 9);
            lblEstadoTitulo.Name = "lblEstadoTitulo";
            lblEstadoTitulo.Size = new Size(38, 15);
            lblEstadoTitulo.TabIndex = 3;
            lblEstadoTitulo.Text = "label1";
            // 
            // cmbEstado
            // 
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Location = new Point(667, 6);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(121, 23);
            cmbEstado.TabIndex = 4;
            // 
            // pnlAgregarLinea
            // 
            pnlAgregarLinea.Controls.Add(label5);
            pnlAgregarLinea.Controls.Add(label4);
            pnlAgregarLinea.Controls.Add(btnAgregarLinea);
            pnlAgregarLinea.Controls.Add(numCantidad);
            pnlAgregarLinea.Controls.Add(cmbProducto);
            pnlAgregarLinea.Location = new Point(34, 35);
            pnlAgregarLinea.Name = "pnlAgregarLinea";
            pnlAgregarLinea.Size = new Size(490, 148);
            pnlAgregarLinea.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(14, 63);
            label5.Name = "label5";
            label5.Size = new Size(58, 15);
            label5.TabIndex = 4;
            label5.Text = "Cantidad:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 14);
            label4.Name = "label4";
            label4.Size = new Size(59, 15);
            label4.TabIndex = 3;
            label4.Text = "Producto:";
            // 
            // btnAgregarLinea
            // 
            btnAgregarLinea.Location = new Point(13, 111);
            btnAgregarLinea.Name = "btnAgregarLinea";
            btnAgregarLinea.Size = new Size(75, 23);
            btnAgregarLinea.TabIndex = 2;
            btnAgregarLinea.Text = "Agregar";
            btnAgregarLinea.UseVisualStyleBackColor = true;
            btnAgregarLinea.Click += btnAgregarLinea_Click;
            // 
            // numCantidad
            // 
            numCantidad.Location = new Point(98, 61);
            numCantidad.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCantidad.Name = "numCantidad";
            numCantidad.Size = new Size(120, 23);
            numCantidad.TabIndex = 1;
            numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // cmbProducto
            // 
            cmbProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProducto.FormattingEnabled = true;
            cmbProducto.Location = new Point(98, 14);
            cmbProducto.Name = "cmbProducto";
            cmbProducto.Size = new Size(334, 23);
            cmbProducto.TabIndex = 0;
            // 
            // dgvDetalles
            // 
            dgvDetalles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDetalles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDetalles.Location = new Point(12, 198);
            dgvDetalles.Name = "dgvDetalles";
            dgvDetalles.Size = new Size(776, 203);
            dgvDetalles.TabIndex = 6;
            // 
            // btnQuitarLinea
            // 
            btnQuitarLinea.Location = new Point(34, 415);
            btnQuitarLinea.Name = "btnQuitarLinea";
            btnQuitarLinea.Size = new Size(149, 23);
            btnQuitarLinea.TabIndex = 7;
            btnQuitarLinea.Text = "Quitar linea seleccionada";
            btnQuitarLinea.UseVisualStyleBackColor = true;
            btnQuitarLinea.Click += btnQuitarLinea_Click;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 15F);
            lblTotal.Location = new Point(474, 410);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(65, 28);
            lblTotal.TabIndex = 8;
            lblTotal.Text = "label1";
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(702, 415);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 9;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(582, 415);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 10;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 11;
            label1.Text = "Nro de venta:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(168, 9);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 12;
            label2.Text = "Comprador:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(327, 9);
            label3.Name = "label3";
            label3.Size = new Size(98, 15);
            label3.TabIndex = 13;
            label3.Text = "Metodo de pago:";
            // 
            // VentaDetalleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Controls.Add(lblTotal);
            Controls.Add(btnQuitarLinea);
            Controls.Add(dgvDetalles);
            Controls.Add(pnlAgregarLinea);
            Controls.Add(cmbEstado);
            Controls.Add(lblEstadoTitulo);
            Controls.Add(cmbMetodoPago);
            Controls.Add(lblComprador);
            Controls.Add(lblNroVenta);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "VentaDetalleForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "VentaDetalleForm";
            Load += VentaDetalleForm_Load;
            pnlAgregarLinea.ResumeLayout(false);
            pnlAgregarLinea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvDetalles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNroVenta;
        private Label lblComprador;
        private ComboBox cmbMetodoPago;
        private Label lblEstadoTitulo;
        private ComboBox cmbEstado;
        private Panel pnlAgregarLinea;
        private Button btnAgregarLinea;
        private NumericUpDown numCantidad;
        private ComboBox cmbProducto;
        private DataGridView dgvDetalles;
        private Button btnQuitarLinea;
        private Label lblTotal;
        private Button btnGuardar;
        private Button btnCancelar;
        private Label label1;
        private Label label5;
        private Label label4;
        private Label label2;
        private Label label3;
    }
}