using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Direccion
    {
        public int IdDireccion { get; set; }
        public Usuario Usuario { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public int? Piso { get; set; }
        public string? Departamento { get; set; }
        public string CodigoPostal { get; set; }

        public Direccion() { }
    }
}
