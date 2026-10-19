using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TPI.Services.DTOs
{
    public class UsuarioDTO { 
        public int Id { get; set; } 
        public string Nombre { get; set; } = string.Empty; 
        public string Apellido { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; 
        public string DNI { get; set; } = string.Empty; 
        public DateTime FechaNacimiento { get; set; } 
        public DateTime FechaAlta { get; set; } 
        public string Telefono { get; set; } = string.Empty; 
        public bool IsAdmin { get; set; } }

    public class UsuarioCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;
        [Required, StringLength(50, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 50 caracteres.")]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Clave { get; set; } = string.Empty;
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        public string DNI { get; set; } = string.Empty;
        [CustomValidation(typeof(UsuarioCreateDTO), nameof(ValidarFechaNacimiento))]
        public DateTime FechaNacimiento { get; set; }
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; } = string.Empty;

        public static ValidationResult? ValidarFechaNacimiento(DateTime fecha, ValidationContext context) =>
            fecha.Date <= DateTime.Today ? ValidationResult.Success : new ValidationResult("La fecha de nacimiento no puede ser futura.");
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

