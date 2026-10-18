using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.ApiClients
{
    public class VentaApiClient : BaseApiClient, IVentaApiClient
    {
        public VentaApiClient(HttpClient httpClient, ISessionService sesion) : base(httpClient, sesion) { }

        public async Task<VentaDTO> CreateAsync(VentaCreateDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PostAsJsonAsync("/ventas", dto);
            await EnsureSuccessOrThrowAsync(response);
            return (await response.Content.ReadFromJsonAsync<VentaDTO>())!;
        }

        public async Task<VentaDTO?> GetByNroVentaAsync(int nroVenta)
        {
            AgregarToken();
            var response = await HttpClient.GetAsync($"/ventas/{nroVenta}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<VentaDTO>();
        }

        public async Task<List<VentaDTO>> GetAllAsync(string? estado = null)
        {
            AgregarToken();
            var url = string.IsNullOrWhiteSpace(estado)
                ? "/ventas"
                : $"/ventas?estado={Uri.EscapeDataString(estado)}";

            var response = await HttpClient.GetAsync(url);
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<List<VentaDTO>>() ?? new();
        }

        public async Task<bool> UpdateAsync(VentaUpdateDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PutAsJsonAsync("/ventas", dto);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }

        public async Task<bool> DeleteAsync(int nroVenta)
        {
            AgregarToken();
            var response = await HttpClient.DeleteAsync($"/ventas/{nroVenta}");
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }
    }
}
