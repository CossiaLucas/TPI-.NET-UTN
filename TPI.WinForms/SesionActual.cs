using TPI.Services.DTOs;
using TPI.ApiClients;

namespace TPI.WinForms
{
    public class SesionActual : ISessionService
    {
        public  LoginResponseDTO? Usuario { get; private set; }

        public  void Iniciar(LoginResponseDTO usuario) => Usuario = usuario;
        public  void CerrarSesion() => Usuario = null;

        public  bool EstaLogueado =>
            Usuario is not null && Usuario.ExpiresAt > DateTime.UtcNow;
    }
  
}
