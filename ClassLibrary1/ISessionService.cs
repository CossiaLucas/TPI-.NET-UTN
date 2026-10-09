using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.ApiClients
{
    //SesionActual no sirve en blazor, por eso esta interfaz 
    public interface ISessionService
    {
        LoginResponseDTO? Usuario { get; }
        bool EstaLogueado { get; }
        void Iniciar(LoginResponseDTO usuario);
        void CerrarSesion();


    }
}
