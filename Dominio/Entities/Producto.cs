using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Producto
    {
        public int Id { get; set; } 
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public string? FotoUrl { get; set; }

        public ICollection<Precio> Precios { get; set; } = new List<Precio>();
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        public ICollection<ItemCarrito> ItemsCarrito { get; set; } = new List<ItemCarrito>();
        public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();

        public Producto() { }
    }
}
