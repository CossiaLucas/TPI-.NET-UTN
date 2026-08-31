namespace TPI.WinForms
{
    partial class CategoriaListForm
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
            dgvCategorias = new DataGridView();
            btnCrear = new Button();
            btnEditar = new Button();
            Eliminar = new Button();
            lblMensaje = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).BeginInit();
            SuspendLayout();
            // 
            // dgvCategorias
            // 
            dgvCategorias.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvCategorias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategorias.Location = new Point(508, 12);
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.Size = new Size(280, 195);
            dgvCategorias.TabIndex = 0;
            // 
            // btnCrear
            // 
            btnCrear.Location = new Point(29, 43);
            btnCrear.Name = "btnCrear";
            btnCrear.Size = new Size(75, 23);
            btnCrear.TabIndex = 1;
            btnCrear.Text = "Crear";
            btnCrear.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            btnEditar.Location = new Point(29, 102);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(75, 23);
            btnEditar.TabIndex = 2;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = true;
            // 
            // Eliminar
            // 
            Eliminar.Location = new Point(29, 168);
            Eliminar.Name = "Eliminar";
            Eliminar.Size = new Size(75, 23);
            Eliminar.TabIndex = 3;
            Eliminar.Text = "Eliminar";
            Eliminar.UseVisualStyleBackColor = true;
            // 
            // lblMensaje
            // 
            lblMensaje.AutoSize = true;
            lblMensaje.Location = new Point(12, 12);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(35, 15);
            lblMensaje.TabIndex = 5;
            lblMensaje.Text = "Label";
            // 
            // CategoriaListForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblMensaje);
            Controls.Add(Eliminar);
            Controls.Add(btnEditar);
            Controls.Add(btnCrear);
            Controls.Add(dgvCategorias);
            Name = "CategoriaListForm";
            Text = "CategoriaListForm";
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvCategorias;
        private Button btnCrear;
        private Button btnEditar;
        private Button Eliminar;
        private Label lblMensaje;
    }
}