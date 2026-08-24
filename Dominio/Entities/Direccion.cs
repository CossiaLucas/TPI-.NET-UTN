using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class Direccion
    {
        public Usuario User { get; set; } //ID
        public string Calle { get; set; } //ID
        public string Numero { get; set; } //ID
        public int Piso { get; set; }
        public string Departamento { get; set; }
        public string CP { get; set; }

        public Direccion(Usuario user, string calle, string num, int piso, string depto, string cp)
        {
            User = user;
            Calle = calle;
            Numero = num;
            Piso = piso;
            Departamento = depto;
            CP = cp;
        }
    }
}
