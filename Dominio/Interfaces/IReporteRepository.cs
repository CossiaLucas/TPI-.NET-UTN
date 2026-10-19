using Dominio.Reportes;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Consultas de solo lectura para reportes (agregaciones). Se implementa con ADO.NET.
    /// Regla común de "estado": si es null se excluyen las ventas Canceladas (cuentan Pagado y NoPagado);
    /// si tiene valor, solo se consideran las ventas con ese estado.
    /// Las fechas son inclusivas por día: [desde 00:00, hasta 23:59:59].
    /// </summary>
    public interface IReporteRepository
    {
        Task<List<VentasPorMes>> ObtenerVentasPorMesAsync(
            DateTime desde, DateTime hasta, string? estado, CancellationToken ct = default);

        Task<List<ProductoMasVendido>> ObtenerProductosMasVendidosAsync(
            DateTime desde, DateTime hasta, string? estado, int? idCategoria, int top, CancellationToken ct = default);

        Task<List<ProductoStockBajo>> ObtenerProductosStockBajoAsync(
            int umbral, int? idCategoria, CancellationToken ct = default);
    }
}
