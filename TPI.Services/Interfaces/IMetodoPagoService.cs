using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IMetodoPagoService
    {
        Task<List<MetodoPagoDTO>> GetAllAsync();
        Task<MetodoPagoDTO?> GetByIdAsync(int id);
        Task<MetodoPagoDTO> CreateAsync(MetodoPagoDTO dto);
        Task<bool> UpdateAsync(MetodoPagoDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
