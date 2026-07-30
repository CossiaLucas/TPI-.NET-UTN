using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Usuario
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }
        public string DNI { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public DateOnly FechaAlta { get; set; }
        public string Telefono { get; set; }
        public bool IsAdmin { get; set; }

        public Usuario(int id, string nom, string apel, string email, string clave, string dni, DateOnly fechanac, DateOnly fechaalta, string telef, bool isAdmin)
        {
            Id = id;
            Nombre = nom;
            Apellido = apel;
            Email = email;
            Clave = clave;
            DNI = dni;
            FechaNacimiento = fechanac;
            FechaAlta = fechaalta;
            Telefono = telef;
            IsAdmin = isAdmin;
        }
    }
}
