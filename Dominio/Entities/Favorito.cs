using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Favorito
    {
        Usuario Usuario { get; set; } //ID
        Producto Producto { get; set; } //ID
        public DateOnly FechaAgregado { get; set; }

        public Favorito(Usuario user, Producto prod, DateOnly fecha) 
        {
            Usuario = user;
            Producto = prod;
            FechaAgregado = fecha;
        }
    }
}
