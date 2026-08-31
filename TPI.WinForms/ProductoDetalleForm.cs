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
    public partial class ProductoDetalleForm : Form
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private ProductoDTO? _productoActual;

        public ProductoDetalleForm(IProductoService productoService, ICategoriaService categoriaService)
        {
            InitializeComponent();
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public void Inicializar(ProductoDTO? producto)
        {
            _productoActual = producto;
            txtDescripcion.Text = producto?.Descripcion ?? string.Empty;
            txtNombre.Text = producto?.Nombre ?? string.Empty;
            numStock.Value = producto?.Stock ?? 0;
            numPrecio.Value = producto?.PrecioActual ?? 0;
            txtFotoUrl.Text = producto?.FotoUrl ?? string.Empty;

            this.Text = producto is null ? "Nuevo producto" : "Editar producto";
        }

        private async void ProductoDetalleForm_Load(object sender, EventArgs e)
        {
            var categorias = await _categoriaService.GetAllAsync();
            cmbCategoria.DataSource = categorias;
            cmbCategoria.DisplayMember = nameof(CategoriaDTO.Nombre);
            cmbCategoria.ValueMember = nameof(CategoriaDTO.Id);

            if (_productoActual is not null)
            {
                txtNombre.Text = _productoActual.Nombre;
                txtDescripcion.Text = _productoActual.Descripcion;
                numStock.Value = _productoActual.Stock;
                numPrecio.Value = _productoActual.PrecioActual ?? 0;
                txtFotoUrl.Text = _productoActual.FotoUrl;
                cmbCategoria.SelectedValue = _productoActual.IdCategoria;
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }
            if (cmbCategoria.SelectedValue is null)
            {
                MessageBox.Show("Seleccioná una categoría.");
                return;
            }

            try
            {
                if (_productoActual is null)
                {
                    await _productoService.CreateAsync(new ProductoDTO
                    {
                        IdCategoria = (int)cmbCategoria.SelectedValue,
                        Nombre = txtNombre.Text,
                        Descripcion = txtDescripcion.Text,
                        Stock = (int)numStock.Value,
                        FotoUrl = string.IsNullOrWhiteSpace(txtFotoUrl.Text) ? null : txtFotoUrl.Text,
                        PrecioActual = numPrecio.Value
                    });
                }
                else
                {
                    await _productoService.UpdateAsync(new ProductoDTO
                    {
                        Id = _productoActual.Id,
                        IdCategoria = (int)cmbCategoria.SelectedValue,
                        Nombre = txtNombre.Text,
                        Descripcion = txtDescripcion.Text,
                        Stock = (int)numStock.Value,
                        FotoUrl = string.IsNullOrWhiteSpace(txtFotoUrl.Text) ? null : txtFotoUrl.Text,
                        PrecioActual = numPrecio.Value
                    });
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


    }
}
