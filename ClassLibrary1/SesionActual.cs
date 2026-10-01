
using TPI.Services.DTOs;

namespace TPI.ApiClients
{
    public static class SesionActual
    {
        public static LoginResponseDTO? Usuario { get; private set; }

        public static void Iniciar(LoginResponseDTO usuario) => Usuario = usuario;
        public static void CerrarSesion() => Usuario = null;

        public static bool EstaLogueado =>
            Usuario is not null && Usuario.ExpiresAt > DateTime.UtcNow;
    }
}
