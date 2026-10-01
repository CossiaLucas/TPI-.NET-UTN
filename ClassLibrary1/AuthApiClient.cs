using System.Net;
using System.Net.Http.Json;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.ApiClients
{
    public class AuthApiClient : BaseApiClient, IAuthService
    {
        public AuthApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
        {
            var response = await HttpClient.PostAsJsonAsync("/auth/login", dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null; // mismo contrato que antes: null = credenciales inválidas

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
        }
    }
}
