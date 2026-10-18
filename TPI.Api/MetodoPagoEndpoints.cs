using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class MetodoPagoEndpoints
    {
        public static void MapMetodoPagoEndpoints(this WebApplication app)
        {
            app.MapGet("/metodos-pago", async (IMetodoPagoService service) =>
                Results.Ok(await service.GetAllAsync()))
                .WithName("GetAllMetodosPago")
                .Produces<List<MetodoPagoDTO>>(StatusCodes.Status200OK)
                .RequireAuthorization();
        }
    }
}
