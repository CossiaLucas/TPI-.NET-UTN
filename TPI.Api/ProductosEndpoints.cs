using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class ProductoEndpoints
    {
        public static void MapProductoEndpoints(this WebApplication app)
        {
            app.MapGet("/productos/{id}", async (int id, IProductoService service) =>
            {
                var dto = await service.GetByIdAsync(id);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("GetProducto").Produces<ProductoDTO>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);
            app.MapGet("/productos", async (IProductoService service) => Results.Ok(await service.GetAllAsync()))
                .WithName("GetAllProductos").Produces<List<ProductoDTO>>(StatusCodes.Status200OK);
            app.MapPost("/productos", async (ProductoCreateDTO dto, IProductoService service) =>
            {
                try
                {
                    var creado = await service.CreateAsync(dto);
                    return Results.Created($"/productos/{creado.Id}", creado);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("AddProducto").Produces<ProductoDTO>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);
            app.MapPut("/productos", async (ProductoUpdateDTO dto, IProductoService service) =>
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
            .WithName("UpdateProducto").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status400BadRequest);
            app.MapDelete("/productos/{id}", async (int id, IProductoService service) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteProducto").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);
        }
    }
}