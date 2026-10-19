using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class ReporteEndpoints
    {
        public static void MapReporteEndpoints(this WebApplication app)
        {
            // Los reportes son solo para administradores.
            var group = app.MapGroup("/reportes").RequireAuthorization("SoloAdmin");

            group.MapGet("/ventas-por-mes", async (
                DateTime? desde, DateTime? hasta, string? estado, IReporteService service, CancellationToken ct) =>
            {
                try
                {
                    return Results.Ok(await service.VentasPorMesAsync(desde, hasta, estado, ct));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("ReporteVentasPorMes")
            .Produces<IEnumerable<VentasPorMesDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/productos-mas-vendidos", async (
                DateTime? desde, DateTime? hasta, string? estado, int? idCategoria, int? top,
                IReporteService service, CancellationToken ct) =>
            {
                try
                {
                    return Results.Ok(await service.ProductosMasVendidosAsync(desde, hasta, estado, idCategoria, top, ct));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("ReporteProductosMasVendidos")
            .Produces<IEnumerable<ProductoMasVendidoDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/productos-stock-bajo", async (
                int? umbral, int? idCategoria, IReporteService service, CancellationToken ct) =>
            {
                try
                {
                    return Results.Ok(await service.ProductosStockBajoAsync(umbral, idCategoria, ct));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("ReporteProductosStockBajo")
            .Produces<IEnumerable<ProductoStockBajoDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
