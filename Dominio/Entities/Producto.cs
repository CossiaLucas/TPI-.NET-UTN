using Dominio.Exceptions;

namespace Dominio.Entities
{
    public class Producto
    {
        public const int NombreMaxLength = 150;
        public const int FotoUrlMaxLength = 500;
        public const decimal PrecioMaximo = 99_999_999.99m; // decimal(10,2)

        public int Id { get; set; }
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string? FotoUrl { get; set; }

        public ICollection<Precio> Precios { get; set; } = new List<Precio>();
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        public ICollection<ItemCarrito> ItemsCarrito { get; set; } = new List<ItemCarrito>();
        public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
        public List<Precio> HistorialPrecios { get; set; } = new();

        public Producto() { }

        public void Validar()
        {
            Nombre = (Nombre ?? string.Empty).Trim();
            Descripcion = (Descripcion ?? string.Empty).Trim();
            FotoUrl = string.IsNullOrWhiteSpace(FotoUrl) ? null : FotoUrl.Trim();

            if (Nombre.Length == 0)
                throw new ValidacionDominioException("El nombre del producto es obligatorio.");

            if (Nombre.Length > NombreMaxLength)
                throw new ValidacionDominioException(
                    $"El nombre del producto no puede superar los {NombreMaxLength} caracteres.");

            if (Stock < 0)
                throw new ValidacionDominioException("El stock no puede ser negativo.");

            if (IdCategoria <= 0)
                throw new ValidacionDominioException("Debe seleccionar una categoría.");

            if (FotoUrl is not null)
            {
                if (FotoUrl.Length > FotoUrlMaxLength)
                    throw new ValidacionDominioException(
                        $"La URL de la foto no puede superar los {FotoUrlMaxLength} caracteres.");

                if (!Uri.TryCreate(FotoUrl, UriKind.Absolute, out var uri)
                    || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    throw new ValidacionDominioException(
                        "La URL de la foto debe comenzar con http:// o https://.");
            }
        }

        public static void ValidarPrecio(decimal? precio)
        {
            if (precio is null) return;

            if (precio < 0)
                throw new ValidacionDominioException("El precio no puede ser negativo.");

            if (precio > PrecioMaximo)
                throw new ValidacionDominioException($"El precio no puede superar {PrecioMaximo:N2}.");
        }

        public decimal? ObtenerPrecioActual()
        {
            var ahora = DateTime.Now;
            return Precios
                .Where(p => p.FechaDesde <= ahora)
                .OrderByDescending(p => p.FechaDesde)
                .Select(p => (decimal?)p.Valor)
                .FirstOrDefault();
        }
    }
}
