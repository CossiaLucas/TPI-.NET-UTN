using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IReporteService
    {
        /// <summary>
        /// Ventas agrupadas por mes. Por defecto: los últimos 12 meses (incluido el actual).
        /// estado = null excluye las canceladas; con valor (NoPagado / Pagado / Cancelado) filtra por ese estado.
        /// </summary>
        Task<IEnumerable<VentasPorMesDTO>> VentasPorMesAsync(
            DateTime? desde, DateTime? hasta, string? estado, CancellationToken ct = default);

        /// <summary>Ranking de productos por unidades vendidas. Por defecto: top 10 de los últimos 12 meses.</summary>
        Task<IEnumerable<ProductoMasVendidoDTO>> ProductosMasVendidosAsync(
            DateTime? desde, DateTime? hasta, string? estado, int? idCategoria, int? top, CancellationToken ct = default);

        /// <summary>Productos con stock menor o igual al umbral (por defecto 5), del más escaso al menos escaso.</summary>
        Task<IEnumerable<ProductoStockBajoDTO>> ProductosStockBajoAsync(
            int? umbral, int? idCategoria, CancellationToken ct = default);
    }
}
