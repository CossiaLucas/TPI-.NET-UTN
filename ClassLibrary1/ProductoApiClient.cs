using System.Net;
using System.Net.Http.Json;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    public class ProductoApiClient : BaseApiClient, IProductoService
    {
        public ProductoApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<ProductoDTO> CreateAsync(ProductoDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PostAsJsonAsync("/productos", dto);
            await EnsureSuccessOrThrowAsync(response);
            return (await response.Content.ReadFromJsonAsync<ProductoDTO>())!;
        }

        public async Task<ProductoDTO?> GetByIdAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.GetAsync($"/productos/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<ProductoDTO>();
        }

        public async Task<IEnumerable<ProductoDTO>> GetAllAsync()
        {
            AgregarToken();
            return await HttpClient.GetFromJsonAsync<List<ProductoDTO>>("/productos") ?? new();
        }

        public async Task<bool> UpdateAsync(ProductoDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PutAsJsonAsync("/productos", dto);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.DeleteAsync($"/productos/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }
    }
}
