using System.Data;
using Dominio.Entities;
using Dominio.Interfaces;
using Dominio.Reportes;
using Microsoft.Data.SqlClient;

namespace TPI.Data.ADO
{
    /// <summary>
    /// Acceso a datos con ADO.NET puro (SqlConnection / SqlCommand / SqlDataReader) para los reportes.
    /// - Consultas 100% parametrizadas (nunca se concatenan valores en el SQL: no hay inyección SQL).
    /// - Una conexión por consulta, abierta y cerrada con "await using".
    /// - Se une detalleVenta con venta por detalleVenta.IdVenta = venta.NroVenta (la clave primaria de venta es NroVenta).
    /// Las mismas consultas están en Consultas_Reportes.sql para probarlas a mano contra la base.
    /// </summary>
    public class ReporteRepository : IReporteRepository
    {
        private readonly string _connectionString;

        public ReporteRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión es obligatoria.", nameof(connectionString));

            _connectionString = connectionString;
        }

        // ------------------------------------------------------------------ SQL
        private const string SqlVentasPorMes = """
            SELECT YEAR(v.Fecha)  AS Anio,
                   MONTH(v.Fecha) AS Mes,
                   COUNT(*)       AS CantidadVentas,
                   SUM(v.Total)   AS Total
            FROM venta v
            WHERE v.Fecha >= @Desde
              AND v.Fecha <  @HastaExclusivo
              AND ((@Estado IS NULL AND v.Estado <> @Cancelado) OR v.Estado = @Estado)
            GROUP BY YEAR(v.Fecha), MONTH(v.Fecha)
            ORDER BY Anio, Mes;
            """;

        private const string SqlProductosMasVendidos = """
            SELECT TOP (@Top)
                   p.Id           AS IdProducto,
                   p.Nombre       AS Nombre,
                   c.Nombre       AS Categoria,
                   SUM(d.Cantidad) AS Unidades,
                   SUM(d.Subtotal) AS Monto
            FROM detalleVenta d
            INNER JOIN venta     v ON v.NroVenta = d.IdVenta
            INNER JOIN producto  p ON p.Id       = d.IdProducto
            INNER JOIN categoria c ON c.Id       = p.IdCategoria
            WHERE v.Fecha >= @Desde
              AND v.Fecha <  @HastaExclusivo
              AND ((@Estado IS NULL AND v.Estado <> @Cancelado) OR v.Estado = @Estado)
              AND (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
            GROUP BY p.Id, p.Nombre, c.Nombre
            ORDER BY Unidades DESC, Monto DESC, p.Nombre;
            """;

        private const string SqlProductosStockBajo = """
            SELECT p.Id     AS IdProducto,
                   p.Nombre AS Nombre,
                   c.Nombre AS Categoria,
                   p.Stock  AS Stock
            FROM producto p
            INNER JOIN categoria c ON c.Id = p.IdCategoria
            WHERE p.Stock <= @Umbral
              AND (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
            ORDER BY p.Stock ASC, p.Nombre;
            """;

        // ------------------------------------------------------------------ Ventas por mes
        public async Task<List<VentasPorMes>> ObtenerVentasPorMesAsync(
            DateTime desde, DateTime hasta, string? estado, CancellationToken ct = default)
        {
            var resultado = new List<VentasPorMes>();

            await using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync(ct);

            await using var comando = new SqlCommand(SqlVentasPorMes, conexion);
            AgregarParametrosFechas(comando, desde, hasta);
            AgregarParametrosEstado(comando, estado);

            await using var lector = await comando.ExecuteReaderAsync(ct);
            var ordAnio = lector.GetOrdinal("Anio");
            var ordMes = lector.GetOrdinal("Mes");
            var ordCantidad = lector.GetOrdinal("CantidadVentas");
            var ordTotal = lector.GetOrdinal("Total");

            while (await lector.ReadAsync(ct))
            {
                resultado.Add(new VentasPorMes
                {
                    Anio = lector.GetInt32(ordAnio),
                    Mes = lector.GetInt32(ordMes),
                    CantidadVentas = lector.GetInt32(ordCantidad),
                    Total = lector.GetDecimal(ordTotal)
                });
            }

            return resultado;
        }

        // ------------------------------------------------------------------ Productos más vendidos
        public async Task<List<ProductoMasVendido>> ObtenerProductosMasVendidosAsync(
            DateTime desde, DateTime hasta, string? estado, int? idCategoria, int top, CancellationToken ct = default)
        {
            var resultado = new List<ProductoMasVendido>();

            await using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync(ct);

            await using var comando = new SqlCommand(SqlProductosMasVendidos, conexion);
            comando.Parameters.Add("@Top", SqlDbType.Int).Value = top;
            AgregarParametrosFechas(comando, desde, hasta);
            AgregarParametrosEstado(comando, estado);
            AgregarParametroCategoria(comando, idCategoria);

            await using var lector = await comando.ExecuteReaderAsync(ct);
            var ordId = lector.GetOrdinal("IdProducto");
            var ordNombre = lector.GetOrdinal("Nombre");
            var ordCategoria = lector.GetOrdinal("Categoria");
            var ordUnidades = lector.GetOrdinal("Unidades");
            var ordMonto = lector.GetOrdinal("Monto");

            while (await lector.ReadAsync(ct))
            {
                resultado.Add(new ProductoMasVendido
                {
                    IdProducto = lector.GetInt32(ordId),
                    Nombre = lector.GetString(ordNombre),
                    Categoria = lector.GetString(ordCategoria),
                    UnidadesVendidas = lector.GetInt32(ordUnidades),
                    MontoTotal = lector.GetDecimal(ordMonto)
                });
            }

            return resultado;
        }

        // ------------------------------------------------------------------ Stock bajo
        public async Task<List<ProductoStockBajo>> ObtenerProductosStockBajoAsync(
            int umbral, int? idCategoria, CancellationToken ct = default)
        {
            var resultado = new List<ProductoStockBajo>();

            await using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync(ct);

            await using var comando = new SqlCommand(SqlProductosStockBajo, conexion);
            comando.Parameters.Add("@Umbral", SqlDbType.Int).Value = umbral;
            AgregarParametroCategoria(comando, idCategoria);

            await using var lector = await comando.ExecuteReaderAsync(ct);
            var ordId = lector.GetOrdinal("IdProducto");
            var ordNombre = lector.GetOrdinal("Nombre");
            var ordCategoria = lector.GetOrdinal("Categoria");
            var ordStock = lector.GetOrdinal("Stock");

            while (await lector.ReadAsync(ct))
            {
                resultado.Add(new ProductoStockBajo
                {
                    IdProducto = lector.GetInt32(ordId),
                    Nombre = lector.GetString(ordNombre),
                    Categoria = lector.GetString(ordCategoria),
                    Stock = lector.GetInt32(ordStock)
                });
            }

            return resultado;
        }

        // ------------------------------------------------------------------ parámetros compartidos
        private static void AgregarParametrosFechas(SqlCommand comando, DateTime desde, DateTime hasta)
        {
            // Rango por día completo: [desde 00:00, día siguiente a "hasta" 00:00)
            comando.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde.Date;
            comando.Parameters.Add("@HastaExclusivo", SqlDbType.DateTime).Value = hasta.Date.AddDays(1);
        }

        private static void AgregarParametrosEstado(SqlCommand comando, string? estado)
        {
            // Estado null => se excluyen las ventas canceladas.
            comando.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = (object?)estado ?? DBNull.Value;
            comando.Parameters.Add("@Cancelado", SqlDbType.NVarChar, 50).Value = EstadoVenta.Cancelado;
        }

        private static void AgregarParametroCategoria(SqlCommand comando, int? idCategoria)
        {
            comando.Parameters.Add("@IdCategoria", SqlDbType.Int).Value = (object?)idCategoria ?? DBNull.Value;
        }
    }
}
