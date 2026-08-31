using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Categoria
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public Categoria() { }
    }
}
