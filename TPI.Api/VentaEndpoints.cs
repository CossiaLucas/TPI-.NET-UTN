using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class VentaEndpoints
    {
        public static void MapVentaEndpoints(this WebApplication app)
        {
            app.MapGet("/ventas/{nroVenta:int}", async (int nroVenta, IVentaService service) =>
            {
                var venta = await service.GetByNroVentaAsync(nroVenta);
                return venta is null ? Results.NotFound() : Results.Ok(venta);
            })
            .WithName("GetVenta")
            .Produces<VentaDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            app.MapGet("/ventas", async (IVentaService service) =>
                Results.Ok(await service.GetAllAsync()))
                .WithName("GetAllVentas")
                .Produces<IEnumerable<VentaDTO>>(StatusCodes.Status200OK)
                .RequireAuthorization();

            app.MapPost("/ventas", async (VentaDTO dto, IVentaService service) =>
            {
                try
                {
                    var creada = await service.CreateAsync(dto);
                    return Results.Created($"/ventas/{creada.NroVenta}", creada);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("AddVenta")
            .Produces<VentaDTO>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

            app.MapPut("/ventas", async (VentaDTO dto, IVentaService service) =>
            {
                try
                {
                    var actualizada = await service.UpdateAsync(dto);
                    return actualizada ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("UpdateVenta")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

            app.MapDelete("/ventas/{nroVenta:int}", async (int nroVenta, IVentaService service) =>
            {
                var eliminada = await service.DeleteAsync(nroVenta);
                return eliminada ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteVenta")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}
