using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IVentaService
    {
        Task<VentaDTO> CreateAsync(int idUsuario, VentaCreateDTO dto);
        Task<VentaDTO?> GetByNroVentaAsync(int nroVenta);
        Task<IEnumerable<VentaDTO>> GetByUsuarioAsync(int idUsuario, string? estado = null);
        Task<IEnumerable<VentaDTO>> GetAllAsync(string? estado = null);
        Task<bool> UpdateAsync(VentaUpdateDTO dto);
        Task<bool> DeleteAsync(int nroVenta);
    }
}