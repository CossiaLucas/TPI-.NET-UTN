using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class DetalleVentaEndpoints
    {
        public static void MapDetalleVentaEndpoints(this WebApplication app)
        {
            app.MapGet("/detalles-venta/{idDetalle:int}", async (int idDetalle, IDetalleVentaService service) =>
            {
                var detalle = await service.GetByIdAsync(idDetalle);
                return detalle is null ? Results.NotFound() : Results.Ok(detalle);
            })
            .WithName("GetDetalleVenta")
            .Produces<DetalleVentaDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            app.MapGet("/ventas/{nroVenta:int}/detalles", async (int nroVenta, IDetalleVentaService service) =>
                Results.Ok(await service.GetByVentaAsync(nroVenta)))
                .WithName("GetDetallesVenta")
                .Produces<IEnumerable<DetalleVentaDTO>>(StatusCodes.Status200OK)
                .RequireAuthorization();

            app.MapPost("/detalles-venta", async (DetalleVentaDTO dto, IDetalleVentaService service) =>
            {
                try
                {
                    var creado = await service.CreateAsync(dto);
                    return Results.Created($"/detalles-venta/{creado.IdDetalle}", creado);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("AddDetalleVenta")
            .Produces<DetalleVentaDTO>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

            app.MapPut("/detalles-venta", async (DetalleVentaDTO dto, IDetalleVentaService service) =>
            {
                try
                {
                    var actualizado = await service.UpdateAsync(dto);
                    return actualizado ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("UpdateDetalleVenta")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

            app.MapDelete("/detalles-venta/{idDetalle:int}", async (int idDetalle, IDetalleVentaService service) =>
            {
                var eliminado = await service.DeleteAsync(idDetalle);
                return eliminado ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteDetalleVenta")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}
