using Microsoft.EntityFrameworkCore;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class MetodoPagoEndpoints
    {
        public static void MapMetodoPagoEndpoints(this WebApplication app)
        {
            // Lectura: cualquier usuario autenticado (el comprador necesita elegir el método al armar una venta).
            app.MapGet("/metodos-pago", async (IMetodoPagoService service) =>
                Results.Ok(await service.GetAllAsync()))
                .WithName("GetAllMetodosPago")
                .Produces<List<MetodoPagoDTO>>(StatusCodes.Status200OK)
                .RequireAuthorization();

            app.MapGet("/metodos-pago/{id:int}", async (int id, IMetodoPagoService service) =>
            {
                var dto = await service.GetByIdAsync(id);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("GetMetodoPago")
            .Produces<MetodoPagoDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            // Alta, modificación y baja: solo administradores.
            app.MapPost("/metodos-pago", async (MetodoPagoDTO dto, IMetodoPagoService service) =>
            {
                try
                {
                    var creado = await service.CreateAsync(dto);
                    return Results.Created($"/metodos-pago/{creado.IdMetodoPago}", creado);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("AddMetodoPago")
            .Produces<MetodoPagoDTO>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization("SoloAdmin");

            app.MapPut("/metodos-pago", async (MetodoPagoDTO dto, IMetodoPagoService service) =>
            {
                try
                {
                    var found = await service.UpdateAsync(dto);
                    return found ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("UpdateMetodoPago")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization("SoloAdmin");

            app.MapDelete("/metodos-pago/{id:int}", async (int id, IMetodoPagoService service) =>
            {
                try
                {
                    var deleted = await service.DeleteAsync(id);
                    return deleted ? Results.NoContent() : Results.NotFound();
                }
                catch (DbUpdateException)
                {
                    return Results.Conflict(new { error = "No se puede eliminar el método de pago porque tiene ventas asociadas." });
                }
            })
            .WithName("DeleteMetodoPago")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization("SoloAdmin");
        }
    }
}
