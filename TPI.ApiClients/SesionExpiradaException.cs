using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPI.ApiClients
{
    public class SesionExpiradaException : Exception
    {
        public SesionExpiradaException() : base("La sesión expiro. Es necesario volver a iniciar sesion.") { }
    }
}
