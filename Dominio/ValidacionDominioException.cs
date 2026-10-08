namespace Dominio.Exceptions
{

    public class ValidacionDominioException : ArgumentException
    {
        public ValidacionDominioException(string mensaje) : base(mensaje) { }
    }
}
