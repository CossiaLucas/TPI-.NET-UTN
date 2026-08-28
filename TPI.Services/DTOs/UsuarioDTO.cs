using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPI.Services.DTOs
{
    public class UsuarioDTO { 
        public int Id { get; set; } 
        public string Nombre { get; set; } = string.Empty; 
        public string Apellido { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string DNI { get; set; } = string.Empty; 
        public DateTime FechaNacimiento { get; set; } 
        public DateTime FechaAlta { get; set; } 
        public string Telefono { get; set; } = string.Empty; 
        public bool IsAdmin { get; set; } }

    public class UsuarioCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; } = string.Empty;
    }

    public class UsuarioUpdateDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Clave { get; set; } // null = no cambiar contraseña
    }
}

