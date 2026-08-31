using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms
{
    public partial class CategoriaForm : Form
    {
        private readonly ICategoriaService _categoriaService;
        private int? _categoriaIdEnEdicion = null;
        private Label lblNombre;
        private TextBox txtNombre;
        private Button btnGuardar;
        private Button btnCancelar;
        private Label lblMensaje;

        private void InitializeComponent()
        {
            this.Text = "Categoría";
            this.ClientSize = new Size(420, 180);
            lblNombre = new Label { Text = "Nombre", Location = new Point(10, 20), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(80, 16), Size = new Size(320, 23) };
            btnGuardar = new Button { Text = "Guardar", Location = new Point(80, 60), Size = new Size(90, 30) };
            btnCancelar = new Button { Text = "Cancelar", Location = new Point(180, 60), Size = new Size(90, 30) };
            lblMensaje = new Label { Location = new Point(10, 100), Size = new Size(390, 40), ForeColor = Color.Black };
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
            this.Controls.Add(lblMensaje);
        }
        public async void CargarPara(int id)
        {
            try
            {
                var cat = await _categoriaService.GetByIdAsync(id);
                if (cat is not null)
                {
                    _categoriaIdEnEdicion = id;
                    txtNombre.Text = cat.Nombre;
                    this.Text = "Editar Categoría";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblMensaje.Text = "Ingrese un nombre";
                lblMensaje.ForeColor = Color.Red;
                return;
            }
            try
            {
                if (_categoriaIdEnEdicion.HasValue)
                {
                    await _categoriaService.UpdateAsync(new CategoriaUpdateDTO
                    {
                        Id = _categoriaIdEnEdicion.Value,
                        Nombre = txtNombre.Text.Trim()
                    });
                    lblMensaje.Text = "Categoría actualizada";
                }
                else
                {
                    await _categoriaService.CreateAsync(new CategoriaCreateDTO
                    {
                        Nombre = txtNombre.Text.Trim()
                    });
                    lblMensaje.Text = "Categoría creada";
                }
                lblMensaje.ForeColor = Color.Green;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = $"Error: {ex.Message}";
                lblMensaje.ForeColor = Color.Red;
            }
        }
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}