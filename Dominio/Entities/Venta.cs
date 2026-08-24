using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Venta
    {
        public int NroVenta { get; set; }
        public Usuario User { get; set; }
        public Producto Producto { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }

        public Venta(int nroVenta, Usuario user, Producto prod, MetodoPago metod, DateOnly fecha, decimal total, string estado)
        {
            NroVenta = nroVenta;
            User = user;
            Producto = prod;
            MetodoPago = metod;
            Fecha = fecha;
            Total = total;
            Estado = estado;
        }
    }
}
