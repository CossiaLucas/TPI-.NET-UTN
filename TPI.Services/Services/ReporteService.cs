using Dominio.Entities;
using Dominio.Exceptions;
using Dominio.Interfaces;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    /// <summary>
    /// Lógica de negocio de los reportes: valores por defecto, validación de parámetros y armado de resultados.
    /// Las consultas SQL (ADO.NET) viven en IReporteRepository.
    /// </summary>
    public class ReporteService : IReporteService
    {
        public const int MesesPorDefecto = 12;
        public const int MaxMesesRango = 120;
        public const int TopPorDefecto = 10;
        public const int TopMaximo = 100;
        public const int UmbralPorDefecto = 5;
        public const int UmbralMaximo = 100_000;

        private readonly IReporteRepository _repository;

        public ReporteService(IReporteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VentasPorMesDTO>> VentasPorMesAsync(
            DateTime? desde, DateTime? hasta, string? estado, CancellationToken ct = default)
        {
            var (d, h) = ResolverRango(desde, hasta);
            var estadoValido = NormalizarEstado(estado);

            var filas = await _repository.ObtenerVentasPorMesAsync(d, h, estadoValido, ct);
            var porMes = filas.ToDictionary(f => (f.Anio, f.Mes));

            // Se completan con 0 los meses sin ventas del rango, para que el gráfico no tenga "saltos".
            var resultado = new List<VentasPorMesDTO>();
            var mes = new DateTime(d.Year, d.Month, 1);
            var ultimo = new DateTime(h.Year, h.Month, 1);
            while (mes <= ultimo)
            {
                porMes.TryGetValue((mes.Year, mes.Month), out var fila);
                resultado.Add(new VentasPorMesDTO
                {
                    Anio = mes.Year,
                    Mes = mes.Month,
                    Periodo = $"{mes.Year:0000}-{mes.Month:00}",
                    Etiqueta = $"{mes.Month:00}/{mes.Year:0000}",
                    CantidadVentas = fila?.CantidadVentas ?? 0,
                    Total = fila?.Total ?? 0m
                });
                mes = mes.AddMonths(1);
            }

            return resultado;
        }

        public async Task<IEnumerable<ProductoMasVendidoDTO>> ProductosMasVendidosAsync(
            DateTime? desde, DateTime? hasta, string? estado, int? idCategoria, int? top, CancellationToken ct = default)
        {
            var (d, h) = ResolverRango(desde, hasta);
            var estadoValido = NormalizarEstado(estado);
            ValidarCategoria(idCategoria);

            var topFinal = top ?? TopPorDefecto;
            if (topFinal < 1 || topFinal > TopMaximo)
                throw new ValidacionDominioException($"El 'top' debe estar entre 1 y {TopMaximo}.");

            var filas = await _repository.ObtenerProductosMasVendidosAsync(d, h, estadoValido, idCategoria, topFinal, ct);
            return filas.Select(f => new ProductoMasVendidoDTO
            {
                IdProducto = f.IdProducto,
                Nombre = f.Nombre,
                Categoria = f.Categoria,
                UnidadesVendidas = f.UnidadesVendidas,
                MontoTotal = f.MontoTotal
            }).ToList();
        }

        public async Task<IEnumerable<ProductoStockBajoDTO>> ProductosStockBajoAsync(
            int? umbral, int? idCategoria, CancellationToken ct = default)
        {
            ValidarCategoria(idCategoria);

            var umbralFinal = umbral ?? UmbralPorDefecto;
            if (umbralFinal < 0 || umbralFinal > UmbralMaximo)
                throw new ValidacionDominioException($"El umbral de stock debe estar entre 0 y {UmbralMaximo}.");

            var filas = await _repository.ObtenerProductosStockBajoAsync(umbralFinal, idCategoria, ct);
            return filas.Select(f => new ProductoStockBajoDTO
            {
                IdProducto = f.IdProducto,
                Nombre = f.Nombre,
                Categoria = f.Categoria,
                Stock = f.Stock
            }).ToList();
        }

        // ------------------------------------------------------------------ validaciones
        private static (DateTime Desde, DateTime Hasta) ResolverRango(DateTime? desde, DateTime? hasta)
        {
            var h = (hasta ?? DateTime.Today).Date;
            // Por defecto: primer día del mes de hace 11 meses => 12 meses completos incluyendo el actual.
            var d = (desde ?? new DateTime(h.Year, h.Month, 1).AddMonths(-(MesesPorDefecto - 1))).Date;

            if (d > h)
                throw new ValidacionDominioException("La fecha 'desde' no puede ser posterior a la fecha 'hasta'.");

            var meses = (h.Year - d.Year) * 12 + (h.Month - d.Month) + 1;
            if (meses > MaxMesesRango)
                throw new ValidacionDominioException(
                    $"El rango de fechas no puede superar los {MaxMesesRango} meses.");

            return (d, h);
        }

        // null/vacío => sin filtro (se excluyen canceladas). Acepta cualquier combinación de mayúsculas.
        private static string? NormalizarEstado(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return null;

            var canonico = EstadoVenta.Todos.FirstOrDefault(e =>
                string.Equals(e, estado.Trim(), StringComparison.OrdinalIgnoreCase));

            return canonico ?? throw new ValidacionDominioException(
                $"Estado inválido: '{estado}'. Valores permitidos: {string.Join(", ", EstadoVenta.Todos)}.");
        }

        private static void ValidarCategoria(int? idCategoria)
        {
            if (idCategoria is <= 0)
                throw new ValidacionDominioException("La categoría indicada no es válida.");
        }
    }
}
