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
using TPI.ApiClients;

namespace TPI.WinForms
{
    public partial class ProductoListForm : Form
    {
        private readonly IProductoService _productoService;
        private readonly ISessionService _sessionService;

        // Filtros de búsqueda (se agregan a la barra de herramientas existente)
        private List<ProductoDTO> _todos = new();
        private readonly ToolStripTextBox _txtBuscar = new();
        private readonly ToolStripComboBox _cmbCategoria = new();
        private bool _actualizandoFiltros;

        public ProductoListForm(IProductoService productoService, ISessionService sessionService)
        {
            InitializeComponent();
            _productoService = productoService;
            _sessionService = sessionService;
        }

        private async void ProductoListForm_Load(object sender, EventArgs e)
        {
            var esAdmin = _sessionService.Usuario?.IsAdmin == true;
            tsbEditar.Visible = esAdmin;
            tsbNuevo.Visible = esAdmin;
            tsbEliminar.Visible = esAdmin;
            ConfigurarGrilla();
            ConfigurarFiltros();
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

        private void ConfigurarFiltros()
        {
            _txtBuscar.AutoSize = false;
            _txtBuscar.Width = 160;
            _txtBuscar.ToolTipText = "Buscar por nombre del producto";

            _cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbCategoria.AutoSize = false;
            _cmbCategoria.Width = 150;

            var btnLimpiar = new ToolStripButton("Limpiar filtros");
            btnLimpiar.Click += (_, _) =>
            {
                _txtBuscar.Text = string.Empty;
                if (_cmbCategoria.Items.Count > 0)
                    _cmbCategoria.SelectedIndex = 0;
            };

            tsCategorias.Items.Add(new ToolStripSeparator());
            tsCategorias.Items.Add(new ToolStripLabel("Nombre:"));
            tsCategorias.Items.Add(_txtBuscar);
            tsCategorias.Items.Add(new ToolStripLabel("Categoría:"));
            tsCategorias.Items.Add(_cmbCategoria);
            tsCategorias.Items.Add(btnLimpiar);

            _txtBuscar.TextChanged += (_, _) => AplicarFiltros();
            _cmbCategoria.SelectedIndexChanged += (_, _) => AplicarFiltros();
        }

        private async Task ListarAsync()
        {
            _todos = (await _productoService.GetAllAsync()).ToList();
            CargarCategoriasDelFiltro();
            AplicarFiltros();
        }

        // Las categorías del combo salen de los propios productos cargados.
        private void CargarCategoriasDelFiltro()
        {
            var seleccionada = _cmbCategoria.SelectedItem as string;

            _actualizandoFiltros = true;
            _cmbCategoria.Items.Clear();
            _cmbCategoria.Items.Add("(Todas)");
            foreach (var categoria in _todos.Select(p => p.NombreCategoria).Distinct().OrderBy(c => c))
                _cmbCategoria.Items.Add(categoria);

            var indice = seleccionada is null ? 0 : _cmbCategoria.Items.IndexOf(seleccionada);
            _cmbCategoria.SelectedIndex = indice < 0 ? 0 : indice;
            _actualizandoFiltros = false;
        }

        private void AplicarFiltros()
        {
            if (_actualizandoFiltros)
                return;

            IEnumerable<ProductoDTO> resultado = _todos;

            var texto = _txtBuscar.Text.Trim();
            if (texto.Length > 0)
                resultado = resultado.Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase));

            if (_cmbCategoria.SelectedIndex > 0 && _cmbCategoria.SelectedItem is string categoria)
                resultado = resultado.Where(p => p.NombreCategoria == categoria);

            dgvProductos.DataSource = resultado.ToList();
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
