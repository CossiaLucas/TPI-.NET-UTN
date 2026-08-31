namespace TPI.WinForms
{
    partial class MainForm
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


    private Label lblBienvenida;
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuProductos;
        private ToolStripMenuItem menuCategorias;
        private ToolStripMenuItem menuLogout;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados deben eliminarse; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            menuProductos = new ToolStripMenuItem();
            menuCategorias = new ToolStripMenuItem();
            menuLogout = new ToolStripMenuItem();
            lblBienvenida = new Label();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { menuProductos, menuCategorias, menuLogout });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(800, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip";
            // 
            // menuProductos
            // 
            menuProductos.Name = "menuProductos";
            menuProductos.Size = new Size(73, 20);
            menuProductos.Text = "Productos";
            menuProductos.Click += menuProductos_Click;
            // 
            // menuCategorias
            // 
            menuCategorias.Name = "menuCategorias";
            menuCategorias.Size = new Size(75, 20);
            menuCategorias.Text = "Categorías";
            menuCategorias.Click += menuCategorias_Click;
            // 
            // menuLogout
            // 
            menuLogout.Name = "menuLogout";
            menuLogout.Size = new Size(87, 20);
            menuLogout.Text = "Cerrar sesión";
            menuLogout.Click += menuLogout_Click;
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 18F);
            lblBienvenida.Location = new Point(30, 60);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(153, 32);
            lblBienvenida.TabIndex = 1;
            lblBienvenida.Text = "Hola, usuario";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblBienvenida);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TPI - Sistema de Gestión";
            Load += MainForm_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }


}
