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
        Task<IEnumerable<CategoriaDto>> ObtenerTodasAsync();

        Task<CategoriaDto?> ObtenerPorIdAsync(int id);

        Task<CategoriaDto> CrearAsync(CategoriaDto dto);

        Task<bool> ModificarAsync(int id, CategoriaDto dto);

        Task<bool> EliminarAsync(int id);
    }
}