using System.Globalization;
using System.Net.Http.Json;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    /// <summary>Cliente HTTP de los reportes (GET /reportes/...). Solo los administradores tienen acceso.</summary>
    public class ReporteApiClient : BaseApiClient, IReporteService
    {
        public ReporteApiClient(HttpClient httpClient, ISessionService sesion) : base(httpClient, sesion) { }

        public Task<IEnumerable<VentasPorMesDTO>> VentasPorMesAsync(
            DateTime? desde, DateTime? hasta, string? estado, CancellationToken ct = default) =>
            GetListaAsync<VentasPorMesDTO>("/reportes/ventas-por-mes",
                new() { ["desde"] = Fecha(desde), ["hasta"] = Fecha(hasta), ["estado"] = estado }, ct);

        public Task<IEnumerable<ProductoMasVendidoDTO>> ProductosMasVendidosAsync(
            DateTime? desde, DateTime? hasta, string? estado, int? idCategoria, int? top, CancellationToken ct = default) =>
            GetListaAsync<ProductoMasVendidoDTO>("/reportes/productos-mas-vendidos",
                new()
                {
                    ["desde"] = Fecha(desde),
                    ["hasta"] = Fecha(hasta),
                    ["estado"] = estado,
                    ["idCategoria"] = Numero(idCategoria),
                    ["top"] = Numero(top)
                }, ct);

        public Task<IEnumerable<ProductoStockBajoDTO>> ProductosStockBajoAsync(
            int? umbral, int? idCategoria, CancellationToken ct = default) =>
            GetListaAsync<ProductoStockBajoDTO>("/reportes/productos-stock-bajo",
                new() { ["umbral"] = Numero(umbral), ["idCategoria"] = Numero(idCategoria) }, ct);

        private async Task<IEnumerable<T>> GetListaAsync<T>(
            string ruta, Dictionary<string, string?> parametros, CancellationToken ct)
        {
            AgregarToken();

            // Solo se envían los parámetros con valor; la API aplica sus valores por defecto al resto.
            var query = string.Join("&", parametros
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!)}"));

            var response = await HttpClient.GetAsync(query.Length == 0 ? ruta : $"{ruta}?{query}", ct);
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken: ct) ?? new List<T>();
        }

        private static string? Fecha(DateTime? valor) =>
            valor?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private static string? Numero(int? valor) =>
            valor?.ToString(CultureInfo.InvariantCulture);
    }
}
