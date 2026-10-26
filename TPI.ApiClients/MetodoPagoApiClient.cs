using System.Net;
using System.Net.Http.Json;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    public class MetodoPagoApiClient : BaseApiClient, IMetodoPagoService
    {
        public MetodoPagoApiClient(HttpClient httpClient, ISessionService sesion) : base(httpClient, sesion) { }

        public async Task<List<MetodoPagoDTO>> GetAllAsync()
        {
            AgregarToken();
            var response = await HttpClient.GetAsync("/metodos-pago");
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<List<MetodoPagoDTO>>() ?? new();
        }

        public async Task<MetodoPagoDTO?> GetByIdAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.GetAsync($"/metodos-pago/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<MetodoPagoDTO>();
        }

        public async Task<MetodoPagoDTO> CreateAsync(MetodoPagoDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PostAsJsonAsync("/metodos-pago", dto);
            await EnsureSuccessOrThrowAsync(response);
            return (await response.Content.ReadFromJsonAsync<MetodoPagoDTO>())!;
        }

        public async Task<bool> UpdateAsync(MetodoPagoDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PutAsJsonAsync("/metodos-pago", dto);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.DeleteAsync($"/metodos-pago/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }
    }
}
