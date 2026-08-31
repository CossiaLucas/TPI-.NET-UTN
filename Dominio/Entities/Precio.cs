using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Precio
    {
        public int IdProducto { get; set; }
        public DateTime FechaDesde { get; set; }
        public decimal Valor { get; set; }
        public Producto Producto { get; set; } = null!;

        public Precio() { }
    }
}
