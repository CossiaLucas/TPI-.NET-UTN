using System.Globalization;
using System.Net.Http.Json;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    /// <summary>
    /// Cliente HTTP de los reportes (/reportes/*). Implementa IReporteService igual que el resto de los
    /// ApiClients implementan su servicio, así las UIs usan la misma interfaz que la API.
    /// </summary>
    public class ReporteApiClient : BaseApiClient, IReporteService
    {
        public ReporteApiClient(HttpClient httpClient, ISessionService sesion) : base(httpClient, sesion) { }

        public Task<IEnumerable<VentasPorMesDTO>> VentasPorMesAsync(
            DateTime? desde, DateTime? hasta, string? estado, CancellationToken ct = default)
        {
            var url = ConstruirUrl("/reportes/ventas-por-mes",
                ("desde", Fecha(desde)), ("hasta", Fecha(hasta)), ("estado", estado));
            return ObtenerAsync<VentasPorMesDTO>(url, ct);
        }

        public Task<IEnumerable<ProductoMasVendidoDTO>> ProductosMasVendidosAsync(
            DateTime? desde, DateTime? hasta, string? estado, int? idCategoria, int? top, CancellationToken ct = default)
        {
            var url = ConstruirUrl("/reportes/productos-mas-vendidos",
                ("desde", Fecha(desde)), ("hasta", Fecha(hasta)), ("estado", estado),
                ("idCategoria", Numero(idCategoria)), ("top", Numero(top)));
            return ObtenerAsync<ProductoMasVendidoDTO>(url, ct);
        }

        public Task<IEnumerable<ProductoStockBajoDTO>> ProductosStockBajoAsync(
            int? umbral, int? idCategoria, CancellationToken ct = default)
        {
            var url = ConstruirUrl("/reportes/productos-stock-bajo",
                ("umbral", Numero(umbral)), ("idCategoria", Numero(idCategoria)));
            return ObtenerAsync<ProductoStockBajoDTO>(url, ct);
        }

        private async Task<IEnumerable<T>> ObtenerAsync<T>(string url, CancellationToken ct)
        {
            AgregarToken();
            var response = await HttpClient.GetAsync(url, ct);
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken: ct) ?? new List<T>();
        }

        // Fechas siempre en formato ISO (yyyy-MM-dd) sin depender de la cultura de la máquina.
        private static string? Fecha(DateTime? fecha) =>
            fecha?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private static string? Numero(int? valor) =>
            valor?.ToString(CultureInfo.InvariantCulture);

        // Solo incluye los parámetros con valor, y los codifica para la URL.
        private static string ConstruirUrl(string ruta, params (string Nombre, string? Valor)[] parametros)
        {
            var partes = parametros
                .Where(p => !string.IsNullOrWhiteSpace(p.Valor))
                .Select(p => $"{p.Nombre}={Uri.EscapeDataString(p.Valor!)}");

            var queryString = string.Join("&", partes);
            return queryString.Length == 0 ? ruta : $"{ruta}?{queryString}";
        }
    }
}
