using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Usuario
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string ClaveHash { get; set; }
        public string DNI { get; set; }
        public List<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public DateOnly FechaNacimiento { get; set; }
        public DateOnly FechaAlta { get; set; }
        public string Telefono { get; set; }
        public bool IsAdmin { get; set; }

        public Usuario() { }
    }
}
