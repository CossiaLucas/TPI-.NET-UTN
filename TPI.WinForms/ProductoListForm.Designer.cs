namespace TPI.WinForms
{
    partial class ProductoListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductoListForm));
            tcCategorias = new ToolStripContainer();
            tlCategorias = new TableLayoutPanel();
            dgvProductos = new DataGridView();
            btnActualizar = new Button();
            btnSalir = new Button();
            tsCategorias = new ToolStrip();
            tsbNuevo = new ToolStripButton();
            tsbEditar = new ToolStripButton();
            tsbEliminar = new ToolStripButton();
            tcCategorias.ContentPanel.SuspendLayout();
            tcCategorias.TopToolStripPanel.SuspendLayout();
            tcCategorias.SuspendLayout();
            tlCategorias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            tsCategorias.SuspendLayout();
            SuspendLayout();
            // 
            // tcCategorias
            // 
            // 
            // tcCategorias.ContentPanel
            // 
            tcCategorias.ContentPanel.Controls.Add(tlCategorias);
            tcCategorias.ContentPanel.Size = new Size(800, 425);
            tcCategorias.Dock = DockStyle.Fill;
            tcCategorias.Location = new Point(0, 0);
            tcCategorias.Name = "tcCategorias";
            tcCategorias.Size = new Size(800, 450);
            tcCategorias.TabIndex = 1;
            tcCategorias.Text = "toolStripContainer1";
            // 
            // tcCategorias.TopToolStripPanel
            // 
            tcCategorias.TopToolStripPanel.Controls.Add(tsCategorias);
            // 
            // tlCategorias
            // 
            tlCategorias.ColumnCount = 2;
            tlCategorias.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlCategorias.ColumnStyles.Add(new ColumnStyle());
            tlCategorias.Controls.Add(dgvProductos, 0, 0);
            tlCategorias.Controls.Add(btnActualizar, 0, 1);
            tlCategorias.Controls.Add(btnSalir, 1, 1);
            tlCategorias.Dock = DockStyle.Fill;
            tlCategorias.Location = new Point(0, 0);
            tlCategorias.Name = "tlCategorias";
            tlCategorias.RowCount = 2;
            tlCategorias.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlCategorias.RowStyles.Add(new RowStyle());
            tlCategorias.Size = new Size(800, 425);
            tlCategorias.TabIndex = 0;
            // 
            // dgvProductos
            // 
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tlCategorias.SetColumnSpan(dgvProductos, 2);
            dgvProductos.Dock = DockStyle.Fill;
            dgvProductos.Location = new Point(3, 3);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.Size = new Size(794, 390);
            dgvProductos.TabIndex = 0;
            // 
            // btnActualizar
            // 
            btnActualizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnActualizar.Location = new Point(641, 399);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(75, 23);
            btnActualizar.TabIndex = 1;
            btnActualizar.Text = "Actualizar";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // btnSalir
            // 
            btnSalir.Location = new Point(722, 399);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(75, 23);
            btnSalir.TabIndex = 2;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // tsCategorias
            // 
            tsCategorias.Dock = DockStyle.None;
            tsCategorias.Items.AddRange(new ToolStripItem[] { tsbNuevo, tsbEditar, tsbEliminar });
            tsCategorias.Location = new Point(4, 0);
            tsCategorias.Name = "tsCategorias";
            tsCategorias.Size = new Size(112, 25);
            tsCategorias.TabIndex = 0;
            // 
            // tsbNuevo
            // 
            tsbNuevo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbNuevo.Image = (Image)resources.GetObject("tsbNuevo.Image");
            tsbNuevo.ImageTransparentColor = Color.Magenta;
            tsbNuevo.Name = "tsbNuevo";
            tsbNuevo.Size = new Size(23, 22);
            tsbNuevo.Text = "Nuevo";
            tsbNuevo.Click += tsbNuevo_Click;
            // 
            // tsbEditar
            // 
            tsbEditar.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbEditar.Image = (Image)resources.GetObject("tsbEditar.Image");
            tsbEditar.ImageTransparentColor = Color.Magenta;
            tsbEditar.Name = "tsbEditar";
            tsbEditar.Size = new Size(23, 22);
            tsbEditar.Text = "Editar";
            tsbEditar.Click += tsbEditar_Click;
            // 
            // tsbEliminar
            // 
            tsbEliminar.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbEliminar.Image = (Image)resources.GetObject("tsbEliminar.Image");
            tsbEliminar.ImageTransparentColor = Color.Magenta;
            tsbEliminar.Name = "tsbEliminar";
            tsbEliminar.Size = new Size(23, 22);
            tsbEliminar.Text = "Eliminar";
            tsbEliminar.Click += tsbEliminar_Click;
            // 
            // ProductoListForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tcCategorias);
            Name = "ProductoListForm";
            Text = "Lista de Productos";
            Load += ProductoListForm_Load;
            tcCategorias.ContentPanel.ResumeLayout(false);
            tcCategorias.TopToolStripPanel.ResumeLayout(false);
            tcCategorias.TopToolStripPanel.PerformLayout();
            tcCategorias.ResumeLayout(false);
            tcCategorias.PerformLayout();
            tlCategorias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            tsCategorias.ResumeLayout(false);
            tsCategorias.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStripContainer tcCategorias;
        private TableLayoutPanel tlCategorias;
        private DataGridView dgvProductos;
        private Button btnActualizar;
        private Button btnSalir;
        private ToolStrip tsCategorias;
        private ToolStripButton tsbNuevo;
        private ToolStripButton tsbEditar;
        private ToolStripButton tsbEliminar;
    }
}