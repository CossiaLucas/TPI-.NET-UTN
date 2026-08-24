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
        public Categoria Categoria { get; set; }
        public decimal Precio { get; set; }
        public string Imagen { get; set; }

        public Producto(int id, string nom, string desc, int stock, Precio precio, Categoria categoria, string image) 
        {
            Id = id;
            Nombre = nom;
            Descripcion = desc;
            Stock = stock;
            Precio = precio.Valor;
            Categoria = categoria;
            Imagen = image;
        }
    }
}
