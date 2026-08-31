namespace TPI.Services.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
    }

    public class ProductoCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string FotoUrl { get; set; } = string.Empty;
    }

    public class ProductoUpdateDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string FotoUrl { get; set; } = string.Empty;
    }
}