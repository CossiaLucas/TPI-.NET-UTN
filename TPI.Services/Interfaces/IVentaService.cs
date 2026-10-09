using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IVentaService
    {
        Task<VentaDTO> CreateAsync(VentaDTO dto);
        Task<VentaDTO?> GetByNroVentaAsync(int nroVenta);
        Task<IEnumerable<VentaDTO>> GetAllAsync();
        Task<bool> UpdateAsync(VentaDTO dto);
        Task<bool> DeleteAsync(int nroVenta);
    }
}