using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<CategoriaDTO> CreateAsync(CategoriaCreateDTO dto);
        Task<CategoriaDTO?> GetByIdAsync(int id);
        Task<List<CategoriaDTO>> GetAllAsync();
        Task<bool> UpdateAsync(CategoriaUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}