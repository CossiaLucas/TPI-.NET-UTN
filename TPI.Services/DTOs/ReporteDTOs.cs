namespace TPI.Services.DTOs
{
    /// <summary>Fila del reporte "Ventas por mes". Los meses sin ventas vienen con 0 para que el gráfico sea continuo.</summary>
    public class VentasPorMesDTO
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        /// <summary>Período en formato yyyy-MM (ordenable).</summary>
        public string Periodo { get; set; } = string.Empty;
        /// <summary>Etiqueta para mostrar en el eje del gráfico (MM/yyyy).</summary>
        public string Etiqueta { get; set; } = string.Empty;
        public int CantidadVentas { get; set; }
        public decimal Total { get; set; }
    }

    public class ProductoMasVendidoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int UnidadesVendidas { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class ProductoStockBajoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}
