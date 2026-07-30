using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Categoria
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }

        public Categoria(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
}
