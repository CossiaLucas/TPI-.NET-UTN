using System.Net;
using System.Net.Http.Headers;
namespace TPI.ApiClients
{
    public class BaseApiClient
    {
        protected readonly HttpClient HttpClient;

        protected BaseApiClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        // Se llama al principio de cada método antes de cualquier request,
        // así cada llamada va con el token del usuario logueado en ese momento.
        protected void AgregarToken()
        {
            var token = SesionActual.Usuario?.Token;
            HttpClient.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new SesionExpiradaException();
            response.EnsureSuccessStatusCode();
        }

    }
}
