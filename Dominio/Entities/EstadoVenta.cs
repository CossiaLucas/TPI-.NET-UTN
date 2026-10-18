using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public static class EstadoVenta
    {
        public const string NoPagado = "NoPagado";
        public const string Pagado = "Pagado";
        public const string Cancelado = "Cancelado";
        public static readonly string[] Todos = { NoPagado, Pagado, Cancelado };
        public static bool EsValido(string estado)
        {
                return estado == NoPagado || estado == Pagado || estado == Cancelado;
        }   


    }
}
