using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
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
    }
}
