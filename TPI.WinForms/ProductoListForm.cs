using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms
{
    public partial class ProductoListForm : Form
    {
        private readonly IProductoService _productoService;

        public ProductoListForm(IProductoService productoService)
        {
            InitializeComponent();
            _productoService = productoService;
        }

        private async void ProductoListForm_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            await ListarAsync();
        }

        private void ConfigurarGrilla()
        {
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idProducto",
                HeaderText = "ID",
                DataPropertyName = nameof(ProductoDTO.Id)
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nombre",
                HeaderText = "Nombre",
                DataPropertyName = nameof(ProductoDTO.Nombre)
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "categoria",
                HeaderText = "Categoría",
                DataPropertyName = nameof(ProductoDTO.NombreCategoria)
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "stock",
                HeaderText = "Stock",
                DataPropertyName = nameof(ProductoDTO.Stock)
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "precioVigente",
                HeaderText = "Precio",
                DataPropertyName = nameof(ProductoDTO.PrecioActual)
            });
        }

        private async Task ListarAsync()
        {
            var productos = await _productoService.GetAllAsync();
            dgvProductos.DataSource = productos;
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
            var form = Program.ServiceProvider.GetRequiredService<ProductoDetalleForm>();
            form.Inicializar(null);
            if (form.ShowDialog() == DialogResult.OK)
                _ = ListarAsync();
        }

        private void tsbEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow?.DataBoundItem is not ProductoDTO seleccionado)
            {
                MessageBox.Show("Seleccioná un producto de la grilla.");
                return;
            }

            var form = Program.ServiceProvider.GetRequiredService<ProductoDetalleForm>();
            form.Inicializar(seleccionado);
            if (form.ShowDialog() == DialogResult.OK)
                _ = ListarAsync();
        }

        private async void tsbEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow?.DataBoundItem is not ProductoDTO seleccionado)
            {
                MessageBox.Show("Seleccioná un producto de la grilla.");
                return;
            }

            if (MessageBox.Show($"¿Eliminar el producto '{seleccionado.Nombre}'?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                await _productoService.DeleteAsync(seleccionado.Id);
                await ListarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar: {ex.Message}");
            }
        }
    }
}
