using Dominio.Entities;
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
using TPI.ApiClients;
using TPI.Services.DTOs;

namespace TPI.WinForms
{
    public partial class Ventas : Form
    {
        private readonly IVentaApiClient _ventaClient;
        private readonly ISessionService _sesion;

        private bool EsAdmin => _sesion.Usuario?.IsAdmin == true;

        public Ventas(IVentaApiClient ventaClient, ISessionService sesion)
        {
            InitializeComponent();
            _ventaClient = ventaClient;
            _sesion = sesion;
        }

        private async void Ventas_Load(object sender, EventArgs e)
        {
            tsbEliminar.Visible = EsAdmin;
            tsbEditar.ToolTipText = EsAdmin ? "Editar" : "Ver detalle";

            ConfigurarGrilla();
            await ListarAsync();
        }

        private void ConfigurarGrilla()
        {
            dgvVentas.AutoGenerateColumns = false;
            dgvVentas.ReadOnly = true;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.MultiSelect = false;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "nroVenta", HeaderText = "N° Venta", DataPropertyName = nameof(VentaDTO.NroVenta) });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "comprador", HeaderText = "Comprador", DataPropertyName = nameof(VentaDTO.NombreUsuario) });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "metodoPago", HeaderText = "Método de pago", DataPropertyName = nameof(VentaDTO.NombreMetodoPago) });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "total",
                HeaderText = "Total",
                DataPropertyName = nameof(VentaDTO.Total),
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "estado", HeaderText = "Estado", DataPropertyName = nameof(VentaDTO.Estado) });
        }

        private async Task ListarAsync()
        {
            try
            {
                var ventas = await _ventaClient.GetAllAsync();
                dgvVentas.DataSource = ventas.OrderByDescending(v => v.NroVenta).ToList();
            }
            catch (SesionExpiradaException) { throw; }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"No se pudieron cargar las ventas: {ex.Message}");
            }
        }

        private async void btnActualizar_Click(object sender, EventArgs e) => await ListarAsync();

        private void btnSalir_Click(object sender, EventArgs e) => Close();

        private async void tsbNuevo_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<VentaDetalleForm>();
            form.Inicializar(null, soloLectura: false);
            if (form.ShowDialog() == DialogResult.OK)
                await ListarAsync();
        }

        private async void tsbEditar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaDTO seleccionada)
            {
                MessageBox.Show("Seleccioná una venta de la grilla.");
                return;
            }

            var form = Program.ServiceProvider.GetRequiredService<VentaDetalleForm>();
            form.Inicializar(seleccionada.NroVenta, soloLectura: !EsAdmin);
            if (form.ShowDialog() == DialogResult.OK)
                await ListarAsync();
        }

        private async void tsbEliminar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaDTO seleccionada)
            {
                MessageBox.Show("Seleccioná una venta de la grilla.");
                return;
            }

            var aviso = seleccionada.Estado == EstadoVenta.Cancelado
                ? string.Empty
                : "\nSe devolverá el stock de sus productos.";

            if (MessageBox.Show($"¿Eliminar la venta N° {seleccionada.NroVenta}?{aviso}", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                await _ventaClient.DeleteAsync(seleccionada.NroVenta);
                await ListarAsync();
            }
            catch (SesionExpiradaException) { throw; }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"No se pudo eliminar: {ex.Message}");
            }
        }
    }
}
