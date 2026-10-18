using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IVentaService
    {
        Task<VentaDTO> CreateAsync(int idUsuario, VentaCreateDTO dto);
        Task<VentaDTO?> GetByNroVentaAsync(int nroVenta);
        Task<IEnumerable<VentaDTO>> GetByUsuarioAsync(int idUsuario);
        Task<IEnumerable<VentaDTO>> GetAllAsync();
        Task<bool> UpdateAsync(VentaUpdateDTO dto);
        Task<bool> DeleteAsync(int nroVenta);
    }
}