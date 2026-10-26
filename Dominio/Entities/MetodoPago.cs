using Dominio.Exceptions;

namespace Dominio.Entities
{
    public class MetodoPago
    {
        public const int NombreMaxLength = 100;

        public int IdMetodoPago { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public MetodoPago() { }

        public void Validar()
        {
            Nombre = (Nombre ?? string.Empty).Trim();

            if (Nombre.Length == 0)
                throw new ValidacionDominioException("El nombre del método de pago es obligatorio.");

            if (Nombre.Length > NombreMaxLength)
                throw new ValidacionDominioException(
                    $"El nombre del método de pago no puede superar los {NombreMaxLength} caracteres.");
        }
    }
}
