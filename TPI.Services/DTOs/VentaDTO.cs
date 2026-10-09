namespace TPI.Services.DTOs
{
    public class VentaDTO
    {
        public int NroVenta { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int IdMetodoPago { get; set; }
        public string NombreMetodoPago { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "NoPagado";
        public List<DetalleVentaDTO> Detalles { get; set; } = new();
    }
}