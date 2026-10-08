using Dominio.Exceptions;

namespace Dominio.Entities
{
    public class Categoria
    {
        public const int NombreMaxLength = 100;

        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public Categoria() { }

        public void Validar()
        {
            Nombre = (Nombre ?? string.Empty).Trim();

            if (Nombre.Length == 0)
                throw new ValidacionDominioException("El nombre de la categoría es obligatorio.");

            if (Nombre.Length > NombreMaxLength)
                throw new ValidacionDominioException(
                    $"El nombre de la categoría no puede superar los {NombreMaxLength} caracteres.");
        }
    }
}
