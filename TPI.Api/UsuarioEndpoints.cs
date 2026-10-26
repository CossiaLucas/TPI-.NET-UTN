using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class UsuarioEndpoints
    {
        public static void MapUsuarioEndpoints(this WebApplication app)
        {
            app.MapGet("/usuarios/{id}", async (int id, IUsuarioService usuarioService) =>
            {
                var dto = await usuarioService.GetByIdAsync(id);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("GetUsuario")
            .Produces<UsuarioDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization("SoloAdmin");


            app.MapGet("/usuarios", async (IUsuarioService usuarioService) =>
            {
                return Results.Ok(await usuarioService.GetAllAsync());
            })
            .WithName("GetAllUsuarios")
            .Produces<List<UsuarioDTO>>(StatusCodes.Status200OK)
            .RequireAuthorization("SoloAdmin");


            app.MapPost("/usuarios", async (UsuarioCreateDTO dto, IUsuarioService usuarioService) =>
            {
                try
                {
                    var creado = await usuarioService.CreateAsync(dto);
                    return Results.Created($"/usuarios/{creado.Id}", creado);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
                catch (DbUpdateException)
                {
                    // Índices únicos de usuario / email / DNI
                    return Results.BadRequest(new { error = "Ya existe un usuario con ese nombre de usuario, email o DNI." });
                }
            })
            .WithName("AddUsuario")
            .Produces<UsuarioDTO>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);


            app.MapPut("/usuarios", async (UsuarioUpdateDTO dto, IUsuarioService usuarioService) =>
            {
                try
                {
                    var found = await usuarioService.UpdateAsync(dto);
                    return found ? Results.NoContent() : Results.NotFound();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("UpdateUsuario")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization("SoloAdmin");


            app.MapDelete("/usuarios/{id}", async (int id, ClaimsPrincipal user, IUsuarioService usuarioService) =>
            {
                if (user.FindFirstValue(ClaimTypes.NameIdentifier) == id.ToString())
                    return Results.BadRequest(new { error = "No podés eliminar tu propio usuario." });

                try
                {
                    var deleted = await usuarioService.DeleteAsync(id);
                    return deleted ? Results.NoContent() : Results.NotFound();
                }
                catch (DbUpdateException)
                {
                    return Results.Conflict(new { error = "No se puede eliminar el usuario porque tiene ventas asociadas." });
                }
            })
            .WithName("DeleteUsuario")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization("SoloAdmin");
            }
    }
}
