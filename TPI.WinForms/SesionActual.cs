using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.WinForms
{
    public static class SesionActual
    {
        public static LoginResponseDTO? Usuario { get; private set; }

        public static void Iniciar(LoginResponseDTO usuario) => Usuario = usuario;

        public static void CerrarSesion() => Usuario = null;

        public static bool EstaLogueado => Usuario is not null;
    }
}
