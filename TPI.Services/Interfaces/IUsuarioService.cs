using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;

namespace TPI.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> CreateAsync(UsuarioCreateDTO dto);
        Task<UsuarioDTO?> GetByIdAsync(int id);
        Task<List<UsuarioDTO>> GetAllAsync();
        Task<bool> UpdateAsync(UsuarioUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
