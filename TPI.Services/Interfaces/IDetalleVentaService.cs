using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IDetalleVentaService
    {
        Task<DetalleVentaDTO> CreateAsync(DetalleVentaDTO dto);
        Task<DetalleVentaDTO?> GetByIdAsync(int idDetalle);
        Task<IEnumerable<DetalleVentaDTO>> GetByVentaAsync(int nroVenta);
        Task<bool> UpdateAsync(DetalleVentaDTO dto);
        Task<bool> DeleteAsync(int idDetalle);
    }
}