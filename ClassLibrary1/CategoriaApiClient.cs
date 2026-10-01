using System.Net;
using System.Net.Http.Json;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    public class CategoriaApiClient : BaseApiClient, ICategoriaService
    {
        public CategoriaApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<CategoriaDTO> CreateAsync(CategoriaDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PostAsJsonAsync("/categorias", dto);
            await EnsureSuccessOrThrowAsync(response);
            return (await response.Content.ReadFromJsonAsync<CategoriaDTO>())!;
        }

        public async Task<CategoriaDTO?> GetByIdAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.GetAsync($"/categorias/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<CategoriaDTO>();
        }

        public async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
        {
            AgregarToken();
            return await HttpClient.GetFromJsonAsync<List<CategoriaDTO>>("/categorias") ?? new();
        }

        public async Task<bool> UpdateAsync(CategoriaDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PutAsJsonAsync("/categorias", dto);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.DeleteAsync($"/categorias/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }
    }
}