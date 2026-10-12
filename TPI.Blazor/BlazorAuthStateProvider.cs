using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TPI.ApiClients;
using TPI.Services.DTOs;

namespace TPI.Blazor
{
    public class BlazorAuthStateProvider : AuthenticationStateProvider, ISessionService
    {
        public LoginResponseDTO? Usuario { get; private set; }
        public bool EstaLogueado => Usuario is not null && Usuario.ExpiresAt > DateTime.UtcNow;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!EstaLogueado)
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Usuario!.Id.ToString()),
            new(ClaimTypes.Name, $"{Usuario.Nombre} {Usuario.Apellido}"),
            new(ClaimTypes.Email, Usuario.Email),
            new(ClaimTypes.Role, Usuario.IsAdmin ? "Admin" : "Cliente")
        };
            var identity = new ClaimsIdentity(claims, "TPI");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        public void Iniciar(LoginResponseDTO usuario)
        {
            Usuario = usuario;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void CerrarSesion()
        {
            Usuario = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
