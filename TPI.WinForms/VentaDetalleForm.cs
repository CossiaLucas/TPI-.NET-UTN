using Dominio.Entities;
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
using TPI.Services.Interfaces;

namespace TPI.WinForms
{
    public partial class VentaDetalleForm : Form
    {
        private readonly IVentaApiClient _ventaClient;
        private readonly IProductoService _productoService;
        private readonly IMetodoPagoService _metodoPagoService;
        private readonly ISessionService _sesion;

        private int? _nroVenta;
        private bool _soloLectura;
        private string _estadoOriginal = EstadoVenta.NoPagado;
        private readonly BindingList<LineaVenta> _lineas = new();

        public VentaDetalleForm(
            IVentaApiClient ventaClient,
            IProductoService productoService,
            IMetodoPagoService metodoPagoService,
            ISessionService sesion)
        {
            InitializeComponent();
            _ventaClient = ventaClient;
            _productoService = productoService;
            _metodoPagoService = metodoPagoService;
            _sesion = sesion;
        }

        public void Inicializar(int? nroVenta, bool soloLectura)
        {
            _nroVenta = nroVenta;
            _soloLectura = soloLectura;
        }

        private async void VentaDetalleForm_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGrilla();

                var metodos = await _metodoPagoService.GetAllAsync();
                cmbMetodoPago.DisplayMember = nameof(MetodoPagoDTO.Nombre);
                cmbMetodoPago.ValueMember = nameof(MetodoPagoDTO.IdMetodoPago);
                cmbMetodoPago.DataSource = metodos;
                cmbMetodoPago.SelectedIndex = -1;

                cmbEstado.DataSource = EstadoVenta.Todos.ToList();

                // Solo se ofrecen productos con stock disponible
                var productos = (await _productoService.GetAllAsync())
                    .Where(p => p.Stock > 0 && p.PrecioActual is  > 0)
                    .Select(p => new ProductoItem(p.Id, p.Nombre, p.PrecioActual!.Value, p.Stock))
                    .ToList();
                cmbProducto.DisplayMember = nameof(ProductoItem.Texto);
                cmbProducto.ValueMember = nameof(ProductoItem.IdProducto);
                cmbProducto.DataSource = productos;
                cmbProducto.SelectedIndex = -1;

                if (_nroVenta is null)
                    PrepararAlta();
                else
                    await CargarVentaAsync();
            }
            catch (SesionExpiradaException) { throw; }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"No se pudieron cargar los datos: {ex.Message}");
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void ConfigurarGrilla()
        {
            dgvDetalles.AutoGenerateColumns = false;
            dgvDetalles.ReadOnly = true;
            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.AllowUserToDeleteRows = false;
            dgvDetalles.MultiSelect = false;
            dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "producto", HeaderText = "Producto", DataPropertyName = nameof(LineaVenta.Producto) });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "cantidad", HeaderText = "Cantidad", DataPropertyName = nameof(LineaVenta.Cantidad) });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "precioUnitario",
                HeaderText = "Precio unit.",
                DataPropertyName = nameof(LineaVenta.PrecioUnitario),
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "subtotal",
                HeaderText = "Subtotal",
                DataPropertyName = nameof(LineaVenta.Subtotal),
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvDetalles.DataSource = _lineas;
        }

        private void PrepararAlta()
        {
            Text = "Nueva venta";
            lblNroVenta.Text = "(se asigna al guardar)";
            var u = _sesion.Usuario;
            lblComprador.Text = $"{u?.Nombre} {u?.Apellido}";

            lblEstadoTitulo.Visible = false;
            cmbEstado.Visible = false;

            ActualizarControles();
            ActualizarTotal();
        }

        private async Task CargarVentaAsync()
        {
            var venta = await _ventaClient.GetByNroVentaAsync(_nroVenta!.Value);
            if (venta is null)
            {
                MessageBox.Show("La venta ya no existe.");
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            _estadoOriginal = venta.Estado;
            lblNroVenta.Text = venta.NroVenta.ToString();
            lblComprador.Text = venta.NombreUsuario;
            cmbMetodoPago.SelectedValue = venta.IdMetodoPago;
            cmbEstado.SelectedItem = venta.Estado;

            foreach (var d in venta.Detalles)
            {
                _lineas.Add(new LineaVenta
                {
                    IdProducto = d.IdProducto,
                    Producto = d.NombreProducto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                });
            }

            // Una venta cancelada nunca se modifica
            if (venta.Estado == EstadoVenta.Cancelado)
                _soloLectura = true;

            Text = _soloLectura ? $"Venta N° {venta.NroVenta}" : $"Editar venta N° {venta.NroVenta}";
            ActualizarControles();
            ActualizarTotal();
        }

        private void ActualizarControles()
        {
            var editaCabecera = !_soloLectura;
            var editaLineas = !_soloLectura && _estadoOriginal == EstadoVenta.NoPagado;

            cmbMetodoPago.Enabled = editaCabecera;
            cmbEstado.Enabled = editaCabecera && _nroVenta is not null;
            pnlAgregarLinea.Enabled = editaLineas;
            btnQuitarLinea.Enabled = editaLineas;
            btnGuardar.Visible = editaCabecera;
            btnCancelar.Text = _soloLectura ? "Cerrar" : "Cancelar";
        }

        private void ActualizarTotal()
        {
            lblTotal.Text = $"Total: {_lineas.Sum(l => l.Subtotal):C2}";
        }

        private void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedItem is not ProductoItem item)
            {
                MessageBox.Show("Seleccioná un producto.");
                return;
            }

            var cantidad = (int)numCantidad.Value;
            var existente = _lineas.FirstOrDefault(l => l.IdProducto == item.IdProducto);

            if (existente is null)
            {
                _lineas.Add(new LineaVenta
                {
                    IdProducto = item.IdProducto,
                    Producto = item.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = item.Precio
                });
            }
            else
            {
                // Mismo producto dos veces: se suman las cantidades (igual que hace el servidor)
                existente.Cantidad += cantidad;
                _lineas.ResetBindings();
            }

            numCantidad.Value = 1;
            cmbProducto.SelectedIndex = -1;
            ActualizarTotal();
        }

        private void btnQuitarLinea_Click(object sender, EventArgs e)
        {
            if (dgvDetalles.CurrentRow?.DataBoundItem is not LineaVenta linea)
            {
                MessageBox.Show("Seleccioná una línea de la grilla.");
                return;
            }

            _lineas.Remove(linea);
            ActualizarTotal();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbMetodoPago.SelectedValue is not int idMetodoPago)
            {
                MessageBox.Show("Seleccioná un método de pago.");
                return;
            }

            if (_lineas.Count == 0)
            {
                MessageBox.Show("Agregá al menos un producto.");
                return;
            }

            var detalles = _lineas
                .Select(l => new DetalleVentaCreateDTO { IdProducto = l.IdProducto, Cantidad = l.Cantidad })
                .ToList();

            try
            {
                if (_nroVenta is null)
                {
                    var creada = await _ventaClient.CreateAsync(
                        new VentaCreateDTO { IdMetodoPago = idMetodoPago, Detalles = detalles });

                    MessageBox.Show($"Venta N° {creada.NroVenta} registrada por {creada.Total:C2}.",
                        "Venta registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var estado = (string)cmbEstado.SelectedItem!;

                    if (estado == EstadoVenta.Cancelado && _estadoOriginal != EstadoVenta.Cancelado &&
                        MessageBox.Show("Al cancelar la venta se devuelve el stock y no podrá modificarse más. ¿Continuar?",
                            "Confirmar cancelación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;

                    var ok = await _ventaClient.UpdateAsync(new VentaUpdateDTO
                    {
                        NroVenta = _nroVenta.Value,
                        IdMetodoPago = idMetodoPago,
                        Estado = estado,
                        Detalles = detalles
                    });

                    if (!ok)
                    {
                        MessageBox.Show("La venta ya no existe.");
                        DialogResult = DialogResult.Cancel;
                        Close();
                        return;
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SesionExpiradaException) { throw; }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ---- Tipos auxiliares de la pantalla ----

        public class LineaVenta
        {
            public int IdProducto { get; set; }
            public string Producto { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Subtotal => Cantidad * PrecioUnitario;
        }

        public sealed record ProductoItem(int IdProducto, string Nombre, decimal Precio, int Stock)
        {
            public string Texto => $"{Nombre} - {Precio:C2} (stock {Stock})";
        }
    }
}
