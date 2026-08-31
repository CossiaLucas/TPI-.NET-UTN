using Microsoft.Extensions.DependencyInjection;
using TPI.Services.Interfaces;


namespace TPI.WinForms
{
    public partial class CategoriaListForm : Form
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriaListForm(ICategoriaService categoriaService)
        {
            InitializeComponent();
            _categoriaService = categoriaService;
            this.Load += CategoriaListForm_Load;
        }

        private async void CategoriaListForm_Load(object sender, EventArgs e) => await CargarCategorias();

        private async Task CargarCategorias()
        {
            try
            {
                var categorias = await _categoriaService.GetAllAsync();
                dgvCategorias.DataSource = categorias.Select(c => new { c.Id, c.Nombre }).ToList();
                lblMensaje.Text = $"Se cargaron {categorias.Count} categorías";
            }
            catch (Exception ex) { lblMensaje.Text = $"Error: {ex.Message}"; }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<CategoriaForm>();
            if (form.ShowDialog() == DialogResult.OK) CargarCategorias().GetAwaiter().GetResult();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.SelectedRows.Count == 0) { lblMensaje.Text = "Seleccione una categoría"; return; }
            var id = Convert.ToInt32(dgvCategorias.SelectedRows[0].Cells["Id"].Value);
            var form = Program.ServiceProvider.GetRequiredService<CategoriaForm>();
            form.CargarPara(id);
            if (form.ShowDialog() == DialogResult.OK) CargarCategorias().GetAwaiter().GetResult();
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.SelectedRows.Count == 0) { lblMensaje.Text = "Seleccione una categoría"; return; }
            var id = Convert.ToInt32(dgvCategorias.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show($"¿Eliminar?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await _categoriaService.DeleteAsync(id);
                await CargarCategorias();
            }
        }
    }
}