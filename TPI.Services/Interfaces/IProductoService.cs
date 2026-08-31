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
        Task<IEnumerable<ProductoDto>> ObtenerTodosAsync();

        Task<ProductoDto?> ObtenerPorIdAsync(int id);

        Task<IEnumerable<ProductoDto>> ObtenerPorCategoriaAsync(int idCategoria);

        Task<ProductoDto> CrearAsync(ProductoDto dto);

        Task<bool> ModificarAsync(int id, ProductoDto dto);

        Task<bool> EliminarAsync(int id);
    }
}