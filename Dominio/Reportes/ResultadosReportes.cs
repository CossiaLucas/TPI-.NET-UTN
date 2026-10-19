namespace Dominio.Reportes
{
    /// <summary>Una fila del reporte "Ventas por mes".</summary>
    public class VentasPorMes
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int CantidadVentas { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Una fila del reporte "Productos más vendidos".</summary>
    public class ProductoMasVendido
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int UnidadesVendidas { get; set; }
        public decimal MontoTotal { get; set; }
    }

    /// <summary>Una fila del reporte "Productos con stock bajo".</summary>
    public class ProductoStockBajo
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}
