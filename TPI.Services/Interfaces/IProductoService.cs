using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IProductoService
    {
        Task<ProductoDTO> CreateAsync(ProductoDTO dto);
        Task<ProductoDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ProductoDTO>> GetAllAsync();
        Task<bool> UpdateAsync(ProductoDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}