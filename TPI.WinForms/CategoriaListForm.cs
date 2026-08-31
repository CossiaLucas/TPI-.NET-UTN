using Microsoft.Extensions.DependencyInjection;
using TPI.Services.DTOs;
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
        }

        private async void Categorias_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            await ListarAsync();
        }

        private void ConfigurarGrilla()
        {
            dgvCategorias.AutoGenerateColumns = false;
            dgvCategorias.ReadOnly = true;
            dgvCategorias.AllowUserToAddRows = false;
            dgvCategorias.AllowUserToDeleteRows = false;

            dgvCategorias.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                HeaderText = "ID",
                DataPropertyName = nameof(CategoriaDTO.Id)
            });
            dgvCategorias.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nombre",
                HeaderText = "Nombre",
                DataPropertyName = nameof(CategoriaDTO.Nombre)
            });

        }

        private async Task ListarAsync()
        {
            var categorias = await _categoriaService.GetAllAsync();
            dgvCategorias.DataSource = categorias;
        }

        private async void btnActualizar_Click(object sender, EventArgs e)
        {
            await ListarAsync();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsbNuevo_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<CategoriaDetalleForm>();
            form.Inicializar(null);
            if (form.ShowDialog() == DialogResult.OK)
                _ = ListarAsync();
        }

        private void tsbEditar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow?.DataBoundItem is not CategoriaDTO seleccionada)
            {
                MessageBox.Show("Seleccioná una categoría de la grilla.");
                return;
            }

            var form = Program.ServiceProvider.GetRequiredService<CategoriaDetalleForm>();
            form.Inicializar(seleccionada);
            if (form.ShowDialog() == DialogResult.OK)
                _ = ListarAsync();
        }

        private async void tsbEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow?.DataBoundItem is not CategoriaDTO seleccionada)
            {
                MessageBox.Show("Seleccioná una categoría de la grilla.");
                return;
            }

            if (MessageBox.Show($"¿Eliminar la categoría '{seleccionada.Nombre}'?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                await _categoriaService.DeleteAsync(seleccionada.Id);
                await ListarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar: {ex.Message}");
            }
        }

      
    }
}