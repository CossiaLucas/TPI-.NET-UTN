
namespace TPI.Services.DTOs
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
    public class CategoriaCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class CategoriaUpdateDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}