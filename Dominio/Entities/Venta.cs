using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int NroVenta { get; set; }
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public int IdMetodoPago { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "NoPagado";

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        public Venta() { }
    }
}
