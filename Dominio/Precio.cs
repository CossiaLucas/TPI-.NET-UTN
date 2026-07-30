using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Precio
    {
        public DateOnly FechaDesde { get; set; }
        public decimal Valor { get; set; }

        public Precio(DateOnly fechaDesde, decimal valor)
        {
            FechaDesde = fechaDesde;
            Valor = valor;
        }
    }
}
