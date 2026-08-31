using TPI.Services.DTOs;
using TPI.Services.Interfaces;
namespace TPI.Api
{
    public static class CategoriaEndpoints
    {
        public static void MapCategoriaEndpoints(this WebApplication app)
        {
            app.MapGet("/categorias/{id}", async (int id, ICategoriaService service) =>
            {
                var dto = await service.GetByIdAsync(id);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("GetCategoria").Produces<CategoriaDTO>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
            app.MapGet("/categorias", async (ICategoriaService service) => Results.Ok(await service.GetAllAsync()))
                .WithName("GetAllCategorias").Produces<IEnumerable<CategoriaDTO>>(StatusCodes.Status200OK);
            app.MapPost("/categorias", async (CategoriaDTO dto, ICategoriaService service) =>
            {
                try
                {
                    var creado = await service.CreateAsync(dto);
                    return Results.Created($"/categorias/{creado.Id}", creado);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("AddCategoria").Produces<CategoriaDTO>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);
            app.MapPut("/categorias", async (CategoriaDTO dto, ICategoriaService service) =>
            {
                try
                {
                    var found = await service.UpdateAsync(dto);
                    return found ? Results.NoContent() : Results.NotFound();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("UpdateCategoria").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status400BadRequest);
            app.MapDelete("/categorias/{id}", async (int id, ICategoriaService service) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteCategoria").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
        }
    }
}