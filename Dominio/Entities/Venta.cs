using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        public int NroVenta { get; set; }
        public Usuario User { get; set; }
        public Producto Producto { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal Total { get; set; }
        public enum EstadoVenta
        {
            NoPagado,
            Pagado,
            Rechazado
        }

        public Venta() { }
    }
}
