using System.Globalization;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms
{
    /// <summary>
    /// Pantalla de reportes (solo administradores). Los datos salen de la API (/reportes/*), que los
    /// obtiene con ADO.NET. Armado por código (sin Designer) para que sea fácil de leer y mantener.
    /// </summary>
    [System.ComponentModel.DesignerCategory("Code")]
    public class ReportesForm : Form
    {
        private static readonly CultureInfo Cultura = new("es-AR");
        private static readonly string[] EstadosFiltro = { "(Sin canceladas)", "NoPagado", "Pagado", "Cancelado" };

        private sealed record FilaVentas(string Mes, int Ventas, decimal Total);
        private sealed record FilaRanking(string Producto, string Categoria, int Unidades, decimal Monto);
        private sealed record FilaStock(string Producto, string Categoria, int Stock);
        private sealed record ItemCategoria(int? Id, string Nombre)
        {
            public override string ToString() => Nombre;
        }

        private readonly IReporteService _reporteService;
        private readonly ICategoriaService _categoriaService;

        // Reporte 1: ventas por mes (con gráfico)
        private DateTimePicker _desde1 = null!, _hasta1 = null!;
        private ComboBox _estado1 = null!;
        private BarChartControl _grafico = null!;
        private DataGridView _grilla1 = null!;
        private Label _resumen1 = null!;

        // Reporte 2: productos más vendidos
        private DateTimePicker _desde2 = null!, _hasta2 = null!;
        private ComboBox _estado2 = null!, _categoria2 = null!;
        private NumericUpDown _top2 = null!;
        private DataGridView _grilla2 = null!;

        // Reporte 3: stock bajo
        private NumericUpDown _umbral3 = null!;
        private ComboBox _categoria3 = null!;
        private DataGridView _grilla3 = null!;

        public ReportesForm(IReporteService reporteService, ICategoriaService categoriaService)
        {
            _reporteService = reporteService;
            _categoriaService = categoriaService;

            Text = "Reportes";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(ArmarTabVentas());
            tabs.TabPages.Add(ArmarTabRanking());
            tabs.TabPages.Add(ArmarTabStock());
            Controls.Add(tabs);

            Load += async (_, _) => await CargarInicialAsync();
        }

        // ------------------------------------------------------------------ armado de pantallas
        private TabPage ArmarTabVentas()
        {
            var tab = new TabPage("Ventas por mes");
            var filtros = NuevoPanelFiltros();
            (_desde1, _hasta1, _estado1) = AgregarFiltrosVentas(filtros);
            var consultar = new Button { Text = "Consultar", AutoSize = true };
            consultar.Click += async (_, _) => await CargarVentasAsync();
            filtros.Controls.Add(consultar);

            _resumen1 = new Label { Dock = DockStyle.Bottom, Height = 24, TextAlign = ContentAlignment.MiddleLeft };
            _grafico = new BarChartControl { Dock = DockStyle.Top, Height = 280, Titulo = "Total vendido por mes ($)" };
            _grilla1 = NuevaGrilla();

            // El orden de Add importa en WinForms con Dock: se agrega primero lo que ocupa "el resto".
            tab.Controls.Add(_grilla1);
            tab.Controls.Add(_resumen1);
            tab.Controls.Add(_grafico);
            tab.Controls.Add(filtros);
            return tab;
        }

        private TabPage ArmarTabRanking()
        {
            var tab = new TabPage("Productos más vendidos");
            var filtros = NuevoPanelFiltros();
            (_desde2, _hasta2, _estado2) = AgregarFiltrosVentas(filtros);
            filtros.Controls.Add(Etiqueta("Categoría:"));
            _categoria2 = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
            filtros.Controls.Add(_categoria2);
            filtros.Controls.Add(Etiqueta("Top:"));
            _top2 = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 10, Width = 55 };
            filtros.Controls.Add(_top2);
            var consultar = new Button { Text = "Consultar", AutoSize = true };
            consultar.Click += async (_, _) => await CargarRankingAsync();
            filtros.Controls.Add(consultar);

            _grilla2 = NuevaGrilla();
            tab.Controls.Add(_grilla2);
            tab.Controls.Add(filtros);
            return tab;
        }

        private TabPage ArmarTabStock()
        {
            var tab = new TabPage("Stock bajo");
            var filtros = NuevoPanelFiltros();
            filtros.Controls.Add(Etiqueta("Stock menor o igual a:"));
            _umbral3 = new NumericUpDown { Minimum = 0, Maximum = 100000, Value = 5, Width = 70 };
            filtros.Controls.Add(_umbral3);
            filtros.Controls.Add(Etiqueta("Categoría:"));
            _categoria3 = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
            filtros.Controls.Add(_categoria3);
            var consultar = new Button { Text = "Consultar", AutoSize = true };
            consultar.Click += async (_, _) => await CargarStockAsync();
            filtros.Controls.Add(consultar);

            _grilla3 = NuevaGrilla();
            tab.Controls.Add(_grilla3);
            tab.Controls.Add(filtros);
            return tab;
        }

        private static FlowLayoutPanel NuevoPanelFiltros() => new()
        {
            Dock = DockStyle.Top,
            Height = 42,
            Padding = new Padding(6, 6, 6, 0),
            WrapContents = false
        };

        private static Label Etiqueta(string texto) => new()
        {
            Text = texto,
            AutoSize = true,
            Margin = new Padding(8, 6, 2, 0)
        };

        private static (DateTimePicker Desde, DateTimePicker Hasta, ComboBox Estado) AgregarFiltrosVentas(FlowLayoutPanel panel)
        {
            var hoy = DateTime.Today;
            var desde = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100, Value = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-11) };
            var hasta = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100, Value = hoy };
            var estado = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            estado.Items.AddRange(EstadosFiltro);
            estado.SelectedIndex = 0;

            panel.Controls.Add(Etiqueta("Desde:"));
            panel.Controls.Add(desde);
            panel.Controls.Add(Etiqueta("Hasta:"));
            panel.Controls.Add(hasta);
            panel.Controls.Add(Etiqueta("Estado:"));
            panel.Controls.Add(estado);
            return (desde, hasta, estado);
        }

        private static DataGridView NuevaGrilla() => new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = SystemColors.Window
        };

        // ------------------------------------------------------------------ carga de datos
        private async Task CargarInicialAsync()
        {
            try
            {
                var categorias = (await _categoriaService.GetAllAsync()).OrderBy(c => c.Nombre).ToList();
                foreach (var combo in new[] { _categoria2, _categoria3 })
                {
                    combo.Items.Add(new ItemCategoria(null, "(Todas)"));
                    foreach (var c in categorias)
                        combo.Items.Add(new ItemCategoria(c.Id, c.Nombre));
                    combo.SelectedIndex = 0;
                }

                await CargarVentasAsync();
                await CargarRankingAsync();
                await CargarStockAsync();
            }
            catch (ApiErrorException ex)
            {
                Avisar(ex.Message);
            }
        }

        private async Task CargarVentasAsync()
        {
            if (!RangoValido(_desde1, _hasta1)) return;
            try
            {
                var datos = (await _reporteService.VentasPorMesAsync(
                    _desde1.Value.Date, _hasta1.Value.Date, EstadoSeleccionado(_estado1))).ToList();

                _grafico.SetDatos(datos.Select(d => (d.Etiqueta, d.Total)));
                _grilla1.DataSource = datos.Select(d => new FilaVentas(d.Etiqueta, d.CantidadVentas, d.Total)).ToList();
                FormatearMoneda(_grilla1, nameof(FilaVentas.Total));
                _resumen1.Text = $"  Total del período: {datos.Sum(d => d.Total).ToString("C2", Cultura)}  |  " +
                                 $"Ventas: {datos.Sum(d => d.CantidadVentas)}";
            }
            catch (ApiErrorException ex)
            {
                Avisar(ex.Message);
            }
        }

        private async Task CargarRankingAsync()
        {
            if (!RangoValido(_desde2, _hasta2)) return;
            try
            {
                var idCategoria = (_categoria2.SelectedItem as ItemCategoria)?.Id;
                var datos = await _reporteService.ProductosMasVendidosAsync(
                    _desde2.Value.Date, _hasta2.Value.Date, EstadoSeleccionado(_estado2),
                    idCategoria, (int)_top2.Value);

                _grilla2.DataSource = datos
                    .Select(d => new FilaRanking(d.Nombre, d.Categoria, d.UnidadesVendidas, d.MontoTotal)).ToList();
                FormatearMoneda(_grilla2, nameof(FilaRanking.Monto));
            }
            catch (ApiErrorException ex)
            {
                Avisar(ex.Message);
            }
        }

        private async Task CargarStockAsync()
        {
            try
            {
                var idCategoria = (_categoria3.SelectedItem as ItemCategoria)?.Id;
                var datos = await _reporteService.ProductosStockBajoAsync((int)_umbral3.Value, idCategoria);
                _grilla3.DataSource = datos.Select(d => new FilaStock(d.Nombre, d.Categoria, d.Stock)).ToList();
            }
            catch (ApiErrorException ex)
            {
                Avisar(ex.Message);
            }
        }

        // ------------------------------------------------------------------ utilidades
        // Validación a nivel UI (la API valida de nuevo en el dominio).
        private bool RangoValido(DateTimePicker desde, DateTimePicker hasta)
        {
            if (desde.Value.Date <= hasta.Value.Date) return true;
            Avisar("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");
            return false;
        }

        private static string? EstadoSeleccionado(ComboBox combo) =>
            combo.SelectedIndex <= 0 ? null : (string)combo.SelectedItem!;

        private static void FormatearMoneda(DataGridView grilla, string columna)
        {
            if (grilla.Columns[columna] is { } col)
            {
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.FormatProvider = Cultura;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private static void Avisar(string mensaje) =>
            MessageBox.Show(mensaje, "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
