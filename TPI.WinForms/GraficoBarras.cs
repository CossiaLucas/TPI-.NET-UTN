using System.Drawing.Drawing2D;

namespace TPI.WinForms;

/// <summary>
/// Gráfico de barras dibujado con GDI+ (sin librerías externas).
/// Se alimenta con SetDatos(); se redibuja solo al cambiar los datos o el tamaño.
/// </summary>
public sealed class GraficoBarras : Control
{
    private IReadOnlyList<(string Etiqueta, decimal Valor)> _datos = Array.Empty<(string Etiqueta, decimal Valor)>();
    private string _titulo = string.Empty;
    private string _formato = "N0";

    public GraficoBarras()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
        MinimumSize = new Size(300, 160);
    }

    /// <param name="formato">Formato numérico de los valores: "N0" para cantidades, "C0" para importes.</param>
    public void SetDatos(IReadOnlyList<(string Etiqueta, decimal Valor)> datos, string titulo, string formato)
    {
        _datos = datos;
        _titulo = titulo;
        _formato = formato;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var fuenteTitulo = new Font(Font, FontStyle.Bold);
        using var fuenteChica = new Font(Font.FontFamily, Math.Max(7f, Font.Size - 1f));
        g.DrawString(_titulo, fuenteTitulo, Brushes.Black, 8, 6);

        const int izq = 80, der = 15, sup = 40, inf = 42;
        var area = new Rectangle(izq, sup, Width - izq - der, Height - sup - inf);
        if (area.Width < 40 || area.Height < 40)
            return;

        if (_datos.Count == 0 || _datos.All(d => d.Valor == 0))
        {
            using var centrado = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("Sin datos para el período seleccionado", fuenteChica, Brushes.Gray, area, centrado);
            return;
        }

        var max = EscalaMaxima(_datos.Max(d => d.Valor));

        // Líneas guía y valores del eje vertical (5 divisiones)
        using var lineaGuia = new Pen(Color.Gainsboro);
        using var lineaEje = new Pen(Color.Gray);
        using var alineadoDerecha = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        for (var i = 0; i <= 5; i++)
        {
            var y = area.Bottom - area.Height * i / 5f;
            g.DrawLine(i == 0 ? lineaEje : lineaGuia, area.Left, y, area.Right, y);
            g.DrawString((max * i / 5).ToString(_formato), fuenteChica, Brushes.DimGray,
                new RectangleF(0, y - 9, izq - 6, 18), alineadoDerecha);
        }

        // Barras
        var ranura = (float)area.Width / _datos.Count;
        var ancho = Math.Min(ranura * 0.65f, 60f);
        using var relleno = new SolidBrush(Color.SteelBlue);
        using var alineadoCentro = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Near,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        for (var i = 0; i < _datos.Count; i++)
        {
            var (etiqueta, valor) = _datos[i];
            var alto = (float)(valor / max) * area.Height;
            var x = area.Left + ranura * i + (ranura - ancho) / 2f;
            var barra = new RectangleF(x, area.Bottom - alto, ancho, alto);

            g.FillRectangle(relleno, barra);
            g.DrawString(etiqueta, fuenteChica, Brushes.Black,
                new RectangleF(area.Left + ranura * i, area.Bottom + 4, ranura, inf - 4), alineadoCentro);

            // El valor sobre la barra solo se muestra si hay lugar para no pisar a las vecinas
            if (ranura >= 60 && valor > 0)
                g.DrawString(valor.ToString(_formato), fuenteChica, Brushes.Black,
                    new RectangleF(area.Left + ranura * i, barra.Top - 16, ranura, 14), alineadoCentro);
        }
    }

    // Redondea el máximo a 1, 2, 5 o 10 x 10^n para que el eje tenga números "lindos".
    private static decimal EscalaMaxima(decimal max)
    {
        if (max <= 0)
            return 1;

        var exponente = (decimal)Math.Pow(10, Math.Floor(Math.Log10((double)max)));
        var f = max / exponente;
        var bonito = f <= 1 ? 1 : f <= 2 ? 2 : f <= 5 ? 5 : 10;
        return bonito * exponente;
    }
}
