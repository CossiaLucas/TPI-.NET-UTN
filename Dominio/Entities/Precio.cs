using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Precio
    {
        public DateOnly FechaDesde { get; set; }
        public decimal Valor { get; set; }

        public Precio() { }
    }
}
