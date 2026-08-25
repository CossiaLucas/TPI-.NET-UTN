using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
        public class ItemCarrito
        {
            public int Id { get; set; }
            public int CarritoId { get; set; }
            public Carrito Carrito { get; set; }
            public int ProductoId { get; set; }
            public Producto Producto { get; set; }
            public int Cantidad { get; set; }
            public decimal Subtotal { get; set; }

        public ItemCarrito() { }
    }
}
