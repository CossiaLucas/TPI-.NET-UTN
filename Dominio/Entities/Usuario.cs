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
        public int IdDireccion { get; set; }
        public Direccion Direccion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string Clave { get; set; }
        public string Email { get; set; }
        public string Dni { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? Telefono { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public bool IsAdmin { get; set; }
        public Carrito? Carrito { get; set; }

        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        public Usuario() { }
    }
}
