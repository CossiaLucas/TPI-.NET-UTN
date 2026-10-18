using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.ApiClients
{
    public interface IVentaApiClient
    {
        Task<VentaDTO> CreateAsync(VentaCreateDTO dto);
        Task<VentaDTO?> GetByNroVentaAsync(int nroVenta);
        Task<List<VentaDTO>> GetAllAsync(string? estado = null);   // la Api filtra segun el rol del token
        Task<bool> UpdateAsync(VentaUpdateDTO dto);
        Task<bool> DeleteAsync(int nroVenta);
    }
}
