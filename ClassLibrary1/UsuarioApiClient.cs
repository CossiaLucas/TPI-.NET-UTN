using System.Net;
using System.Net.Http.Json;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    public class UsuarioApiClient : BaseApiClient, IUsuarioService
    {
        public UsuarioApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<UsuarioDTO> CreateAsync(UsuarioCreateDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PostAsJsonAsync("/usuarios", dto);
            await EnsureSuccessOrThrowAsync(response);
            return (await response.Content.ReadFromJsonAsync<UsuarioDTO>())!;
        }

        public async Task<UsuarioDTO?> GetByIdAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.GetAsync($"/usuarios/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessOrThrowAsync(response);
            return await response.Content.ReadFromJsonAsync<UsuarioDTO>();
        }

        public async Task<List<UsuarioDTO>> GetAllAsync()
        {
            AgregarToken();
            return await HttpClient.GetFromJsonAsync<List<UsuarioDTO>>("/usuarios") ?? new();
        }

        public async Task<bool> UpdateAsync(UsuarioUpdateDTO dto)
        {
            AgregarToken();
            var response = await HttpClient.PutAsJsonAsync("/usuarios", dto);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            AgregarToken();
            var response = await HttpClient.DeleteAsync($"/usuarios/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessOrThrowAsync(response);
            return true;
        }
    }
}
