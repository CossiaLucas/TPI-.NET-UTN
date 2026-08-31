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
    public partial class CategoriaDetalleForm : Form
    {
        private readonly ICategoriaService _categoriaService;
        private CategoriaDTO? _categoriaActual;

        public CategoriaDetalleForm(ICategoriaService categoriaService)
        {
            InitializeComponent();
            _categoriaService = categoriaService;
        }

        public void Inicializar(CategoriaDTO? categoria)
        {
            _categoriaActual = categoria;
            txtNombre.Text = categoria?.Nombre ?? string.Empty;
            this.Text = categoria is null ? "Nueva categoría" : "Editar categoría";
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            try
            {
                if (_categoriaActual is null)
                {
                    await _categoriaService.CreateAsync(new CategoriaDTO { Nombre = txtNombre.Text });
                }
                else
                {
                    await _categoriaService.UpdateAsync(new CategoriaDTO
                    {
                        Id = _categoriaActual.Id,
                        Nombre = txtNombre.Text
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
