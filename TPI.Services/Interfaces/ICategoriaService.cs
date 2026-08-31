using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<CategoriaDTO> CreateAsync(CategoriaDTO dto);
        Task<CategoriaDTO?> GetByIdAsync(int id);
        Task<IEnumerable<CategoriaDTO>> GetAllAsync();
        Task<bool> UpdateAsync(CategoriaDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}