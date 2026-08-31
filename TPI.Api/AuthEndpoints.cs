using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Api
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/auth/login", async (LoginRequestDTO dto, IAuthService authService) =>
            {
                var result = await authService.LoginAsync(dto);
                return result is null ? Results.Unauthorized() : Results.Ok(result);
            })
            .WithName("Login")
            .Produces<LoginResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
