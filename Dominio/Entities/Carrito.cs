using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Carrito
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public decimal Subtotal { get; set; }

        public ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

        public Carrito () { }
    }
}
