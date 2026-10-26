using Dominio.Entities;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms;

/// <summary>
/// Reportes (solo administradores): Ventas por mes y Productos más vendidos (ambos con gráfico) y
/// Productos con stock bajo. Formulario construido por código, sin Designer.
/// </summary>
public sealed class ReportesGraficosForm : Form
{
    private const string SinCanceladas = "(Sin canceladas)";

    private readonly IReporteService _reportes;

    // --- Ventas por mes
    private readonly DateTimePicker _vmDesde = new() { Format = DateTimePickerFormat.Short, Width = 110 };
    private readonly DateTimePicker _vmHasta = new() { Format = DateTimePickerFormat.Short, Width = 110 };
    private readonly ComboBox _vmEstado = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
    private readonly GraficoBarras _vmGrafico = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _vmGrilla = CrearGrilla();
    private readonly Label _vmResumen = new() { AutoSize = true, Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold) };

    // --- Productos más vendidos
    private readonly DateTimePicker _mvDesde = new() { Format = DateTimePickerFormat.Short, Width = 110 };
    private readonly DateTimePicker _mvHasta = new() { Format = DateTimePickerFormat.Short, Width = 110 };
    private readonly ComboBox _mvEstado = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
    private readonly NumericUpDown _mvTop = new() { Minimum = 1, Maximum = 100, Value = 10, Width = 60 };
    private readonly GraficoBarras _mvGrafico = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _mvGrilla = CrearGrilla();
    private readonly Label _mvResumen = new() { AutoSize = true, Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold) };

    // --- Stock bajo
    private readonly NumericUpDown _sbUmbral = new() { Minimum = 0, Maximum = 100000, Value = 5, Width = 80 };
    private readonly DataGridView _sbGrilla = CrearGrilla();
    private readonly Label _sbResumen = new() { AutoSize = true, Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold) };

    public ReportesGraficosForm(IReporteService reportes)
    {
        _reportes = reportes;

        Text = "Reportes";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1000, 660);

        // Por defecto: los últimos 12 meses completos, igual que la API.
        var hoy = DateTime.Today;
        var desdeDefecto = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-11);
        _vmDesde.Value = desdeDefecto; _vmHasta.Value = hoy;
        _mvDesde.Value = desdeDefecto; _mvHasta.Value = hoy;

        foreach (var combo in new[] { _vmEstado, _mvEstado })
        {
            combo.Items.Add(SinCanceladas);
            foreach (var estado in EstadoVenta.Todos)
                combo.Items.Add(estado);
            combo.SelectedIndex = 0;
        }

        ConfigurarColumnas();

        var pestanias = new TabControl { Dock = DockStyle.Fill };
        pestanias.TabPages.Add(ArmarPestania("Ventas por mes",
            FiltrosVentasPorMes(), _vmResumen, _vmGrafico, _vmGrilla));
        pestanias.TabPages.Add(ArmarPestania("Productos más vendidos",
            FiltrosMasVendidos(), _mvResumen, _mvGrafico, _mvGrilla));
        pestanias.TabPages.Add(ArmarPestania("Stock bajo",
            FiltrosStockBajo(), _sbResumen, null, _sbGrilla));
        Controls.Add(pestanias);

        Load += async (_, _) =>
        {
            // Se generan los tres; si uno falla (ya se avisó al usuario) no se insiste con los demás.
            _ = await GenerarVentasPorMesAsync() && await GenerarMasVendidosAsync() && await GenerarStockBajoAsync();
        };
    }

    // ------------------------------------------------------------------ armado de la pantalla
    private static DataGridView CrearGrilla() => new()
    {
        Dock = DockStyle.Fill,
        AutoGenerateColumns = false,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        RowHeadersVisible = false,
        MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };

    private static DataGridViewTextBoxColumn Columna(string titulo, string propiedad, string? formato = null, float peso = 100) =>
        new()
        {
            HeaderText = titulo,
            DataPropertyName = propiedad,
            FillWeight = peso,
            DefaultCellStyle = new DataGridViewCellStyle { Format = formato ?? string.Empty }
        };

    private void ConfigurarColumnas()
    {
        _vmGrilla.Columns.Add(Columna("Período", nameof(VentasPorMesDTO.Etiqueta)));
        _vmGrilla.Columns.Add(Columna("Cantidad de ventas", nameof(VentasPorMesDTO.CantidadVentas)));
        _vmGrilla.Columns.Add(Columna("Total", nameof(VentasPorMesDTO.Total), "C2"));

        _mvGrilla.Columns.Add(Columna("Producto", nameof(ProductoMasVendidoDTO.Nombre), peso: 160));
        _mvGrilla.Columns.Add(Columna("Categoría", nameof(ProductoMasVendidoDTO.Categoria)));
        _mvGrilla.Columns.Add(Columna("Unidades vendidas", nameof(ProductoMasVendidoDTO.UnidadesVendidas)));
        _mvGrilla.Columns.Add(Columna("Monto total", nameof(ProductoMasVendidoDTO.MontoTotal), "C2"));

        _sbGrilla.Columns.Add(Columna("Producto", nameof(ProductoStockBajoDTO.Nombre), peso: 160));
        _sbGrilla.Columns.Add(Columna("Categoría", nameof(ProductoStockBajoDTO.Categoria)));
        _sbGrilla.Columns.Add(Columna("Stock", nameof(ProductoStockBajoDTO.Stock)));
    }

    private static Label Etiqueta(string texto) =>
        new() { Text = texto, AutoSize = true, Margin = new Padding(10, 7, 2, 0) };

    private static Button Boton(string texto, Func<Task> accion)
    {
        var boton = new Button { Text = texto, AutoSize = true, Margin = new Padding(14, 2, 3, 2) };
        boton.Click += async (_, _) => await accion();
        return boton;
    }

    private FlowLayoutPanel FiltrosVentasPorMes()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        panel.Controls.AddRange(new Control[]
        {
            Etiqueta("Desde:"), _vmDesde, Etiqueta("Hasta:"), _vmHasta,
            Etiqueta("Estado:"), _vmEstado, Boton("Generar", () => GenerarVentasPorMesAsync())
        });
        return panel;
    }

    private FlowLayoutPanel FiltrosMasVendidos()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        panel.Controls.AddRange(new Control[]
        {
            Etiqueta("Desde:"), _mvDesde, Etiqueta("Hasta:"), _mvHasta,
            Etiqueta("Estado:"), _mvEstado, Etiqueta("Top:"), _mvTop, Boton("Generar", () => GenerarMasVendidosAsync())
        });
        return panel;
    }

    private FlowLayoutPanel FiltrosStockBajo()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        panel.Controls.AddRange(new Control[]
        {
            Etiqueta("Stock menor o igual a:"), _sbUmbral, Boton("Generar", () => GenerarStockBajoAsync())
        });
        return panel;
    }

    private static TabPage ArmarPestania(string titulo, Control filtros, Control resumen, Control? grafico, Control grilla)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, Padding = new Padding(8) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(filtros, 0, 0);
        resumen.Margin = new Padding(12, 6, 3, 6);
        layout.Controls.Add(resumen, 0, 1);

        if (grafico is not null)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            layout.Controls.Add(grafico, 0, 2);
            layout.Controls.Add(grilla, 0, 3);
        }
        else
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.Controls.Add(grilla, 0, 2);
        }

        var pestania = new TabPage(titulo);
        pestania.Controls.Add(layout);
        return pestania;
    }

    // ------------------------------------------------------------------ generación de reportes
    private static string? Estado(ComboBox combo) =>
        combo.SelectedIndex > 0 ? combo.SelectedItem as string : null;

    private static void Aviso(string mensaje) =>
        MessageBox.Show(mensaje, "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private async Task<bool> GenerarVentasPorMesAsync()
    {
        if (_vmDesde.Value.Date > _vmHasta.Value.Date)
        {
            Aviso("La fecha 'Desde' no puede ser posterior a 'Hasta'.");
            return false;
        }

        try
        {
            var datos = (await _reportes.VentasPorMesAsync(_vmDesde.Value.Date, _vmHasta.Value.Date, Estado(_vmEstado))).ToList();
            _vmGrilla.DataSource = datos;
            _vmGrafico.SetDatos(
                datos.Select(d => (Etiqueta: d.Etiqueta, Valor: d.Total)).ToList(),
                "Ventas por mes (importe total)", "C0");
            _vmResumen.Text = $"{datos.Sum(d => d.CantidadVentas)} ventas  |  Total: {datos.Sum(d => d.Total):C2}";
            return true;
        }
        catch (ApiErrorException ex) { Aviso(ex.Message); return false; }
        catch (HttpRequestException ex) { Aviso($"No se pudo generar el reporte: {ex.Message}"); return false; }
    }

    private async Task<bool> GenerarMasVendidosAsync()
    {
        if (_mvDesde.Value.Date > _mvHasta.Value.Date)
        {
            Aviso("La fecha 'Desde' no puede ser posterior a 'Hasta'.");
            return false;
        }

        try
        {
            var datos = (await _reportes.ProductosMasVendidosAsync(
                _mvDesde.Value.Date, _mvHasta.Value.Date, Estado(_mvEstado), null, (int)_mvTop.Value)).ToList();
            _mvGrilla.DataSource = datos;
            _mvGrafico.SetDatos(
                datos.Select(d => (Etiqueta: d.Nombre, Valor: (decimal)d.UnidadesVendidas)).ToList(),
                $"Top {_mvTop.Value:0} de productos más vendidos (unidades)", "N0");
            _mvResumen.Text = $"{datos.Sum(d => d.UnidadesVendidas)} unidades  |  Monto: {datos.Sum(d => d.MontoTotal):C2}";
            return true;
        }
        catch (ApiErrorException ex) { Aviso(ex.Message); return false; }
        catch (HttpRequestException ex) { Aviso($"No se pudo generar el reporte: {ex.Message}"); return false; }
    }

    private async Task<bool> GenerarStockBajoAsync()
    {
        try
        {
            var datos = (await _reportes.ProductosStockBajoAsync((int)_sbUmbral.Value, null)).ToList();
            _sbGrilla.DataSource = datos;
            _sbResumen.Text = $"{datos.Count} productos con stock menor o igual a {_sbUmbral.Value:0}";
            return true;
        }
        catch (ApiErrorException ex) { Aviso(ex.Message); return false; }
        catch (HttpRequestException ex) { Aviso($"No se pudo generar el reporte: {ex.Message}"); return false; }
    }
}
