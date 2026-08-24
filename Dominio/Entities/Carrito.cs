using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Carrito
    {
        public Usuario User { get; set; } //ID
        public Producto Producto { get; set; } //ID
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }

        public Carrito(Usuario usuario, Producto producto, int cant)
        {
            User = usuario;
            List<Producto> productos = new List<Producto>();
            productos.Add(producto);
            Cantidad = cant;
            //Subtotal = producto.Precio * cant;
        }
    }
}
