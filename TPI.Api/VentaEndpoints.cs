using System.Security.Claims;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class VentaEndpoints
    {
        public static void MapVentaEndpoints(this WebApplication app)
        {
            app.MapGet("/ventas/{nroVenta:int}", async (int nroVenta, ClaimsPrincipal usuario, IVentaService service) =>
            {
                var venta = await service.GetByNroVentaAsync(nroVenta);
                if (venta is null) return Results.NotFound();
                if (!EsAdmin(usuario) && venta.IdUsuario != ObtenerIdUsuario(usuario)) return Results.Forbid();
                return Results.Ok(venta);
            })
            .WithName("GetVenta")
            .Produces<VentaDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();

            //el usuari obtiene las suyas, el admin todas
            app.MapGet("/ventas", async (string? estado, ClaimsPrincipal user, IVentaService service) =>
            {
                try
                {
                    var ventas = EsAdmin(user)
                        ? await service.GetAllAsync(estado)
                        : await service.GetByUsuarioAsync(ObtenerIdUsuario(user), estado);
                    return Results.Ok(ventas);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("GetAllVentas")
            .Produces<IEnumerable<VentaDTO>>(StatusCodes.Status200OK)
            .RequireAuthorization();

            //el comprador viene del token
            app.MapPost("/ventas", async (VentaCreateDTO dto, ClaimsPrincipal user, IVentaService service) =>
            {
                try
                {
                    var creada = await service.CreateAsync(ObtenerIdUsuario(user), dto);
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

            app.MapPut("/ventas", async (VentaUpdateDTO dto, IVentaService service) =>
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
        private static bool EsAdmin(ClaimsPrincipal user) => user.HasClaim("IsAdmin", "True");

        private static int ObtenerIdUsuario(ClaimsPrincipal user) =>
            int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
