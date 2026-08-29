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
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; }
        public List<Precio> HistorialPrecios { get; set; } = new();
        public string FotoUrl { get; set; }

        public Producto() { }
    }
}
