using System.Drawing.Drawing2D;
using System.Globalization;

namespace TPI.WinForms
{
    /// <summary>
    /// Gráfico de barras dibujado con GDI+ (sin librerías externas).
    /// Se le pasan pares (etiqueta, valor) con SetDatos y se redibuja solo.
    /// </summary>
    [System.ComponentModel.DesignerCategory("Code")]
    public class BarChartControl : Control
    {
        private static readonly CultureInfo Cultura = new("es-AR");
        private List<(string Etiqueta, decimal Valor)> _datos = new();

        public string Titulo { get; set; } = string.Empty;

        public BarChartControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
        }

        public void SetDatos(IEnumerable<(string Etiqueta, decimal Valor)> datos)
        {
            _datos = datos.ToList();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            using var fuenteTitulo = new Font(Font, FontStyle.Bold);
            using var fuenteChica = new Font(Font.FontFamily, 8f);
            g.DrawString(Titulo, fuenteTitulo, Brushes.Black, 6, 4);

            const int margenIzq = 80, margenDer = 15, margenSup = 30, margenInf = 30;
            var area = new RectangleF(margenIzq, margenSup,
                Width - margenIzq - margenDer, Height - margenSup - margenInf);
            if (area.Width < 50 || area.Height < 50)
                return;

            if (_datos.Count == 0)
            {
                using var centrado = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Sin datos para mostrar", Font, Brushes.Gray, area, centrado);
                return;
            }

            // Escala del eje Y: 5 divisiones con un paso "redondo" (1, 2, 5 x 10^n).
            var maximo = _datos.Max(d => d.Valor);
            if (maximo <= 0) maximo = 1;
            const int divisiones = 5;
            var paso = PasoRedondo(maximo / divisiones);
            var escala = paso * divisiones;

            using var lapizGrilla = new Pen(Color.Gainsboro);
            using var formatoDerecha = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            for (var i = 0; i <= divisiones; i++)
            {
                var y = area.Bottom - (float)(paso * i / escala) * area.Height;
                g.DrawLine(lapizGrilla, area.Left, y, area.Right, y);
                g.DrawString((paso * i).ToString("N0", Cultura), fuenteChica, Brushes.DimGray,
                    new RectangleF(0, y - 8, margenIzq - 6, 16), formatoDerecha);
            }

            g.DrawLine(Pens.Gray, area.Left, area.Top, area.Left, area.Bottom);
            g.DrawLine(Pens.Gray, area.Left, area.Bottom, area.Right, area.Bottom);

            // Barras + etiquetas del eje X (si no entran todas, se muestra una de cada k).
            var ancho = area.Width / _datos.Count;
            var anchoBarra = Math.Max(2f, ancho * 0.6f);
            var cadaK = (int)Math.Ceiling(44f / ancho);
            using var pincel = new SolidBrush(Color.SteelBlue);
            using var formatoCentro = new StringFormat { Alignment = StringAlignment.Center };

            for (var i = 0; i < _datos.Count; i++)
            {
                var alto = (float)(_datos[i].Valor / escala) * area.Height;
                var x = area.Left + i * ancho + (ancho - anchoBarra) / 2;
                if (alto > 0)
                    g.FillRectangle(pincel, x, area.Bottom - alto, anchoBarra, alto);

                if (i % cadaK == 0)
                    g.DrawString(_datos[i].Etiqueta, fuenteChica, Brushes.Black,
                        new RectangleF(area.Left + i * ancho - 10, area.Bottom + 4, ancho + 20, 16), formatoCentro);
            }
        }

        private static decimal PasoRedondo(decimal bruto)
        {
            var magnitud = (decimal)Math.Pow(10, Math.Floor(Math.Log10((double)bruto)));
            var normal = bruto / magnitud;
            var factor = normal <= 1 ? 1 : normal <= 2 ? 2 : normal <= 5 ? 5 : 10;
            return factor * magnitud;
        }
    }
}
