using Dominio.Entities;
using Dominio.Interfaces;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<CategoriaDTO> CreateAsync(CategoriaDTO dto)
        {
            var categoria = new Categoria { Nombre = dto.Nombre };
            categoria.Validar();
            await ValidarNombreUnicoAsync(categoria.Nombre, idExcluido: null);

            await _categoriaRepository.AddAsync(categoria);
            return ToDto(categoria);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoriaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
        {
            var items = await _categoriaRepository.GetAllAsync();
            return items.Select(ToDto).ToList();
        }

        public async Task<CategoriaDTO?> GetByIdAsync(int id)
        {
            var entidad = await _categoriaRepository.GetByIdAsync(id);
            return entidad is null ? null : ToDto(entidad);
        }

        public async Task<bool> UpdateAsync(CategoriaDTO dto)
        {
            var entidad = await _categoriaRepository.GetByIdAsync(dto.Id);
            if (entidad is null)
                return false;

            entidad.Nombre = dto.Nombre;
            entidad.Validar();
            await ValidarNombreUnicoAsync(entidad.Nombre, idExcluido: entidad.Id);

            return await _categoriaRepository.UpdateAsync(entidad);
        }

        private async Task ValidarNombreUnicoAsync(string nombre, int? idExcluido)
        {
            var existentes = await _categoriaRepository.GetAllAsync();
            var duplicada = existentes.Any(c =>
                c.Id != idExcluido &&
                string.Equals(c.Nombre, nombre, StringComparison.OrdinalIgnoreCase));

            if (duplicada)
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
        }

        private static CategoriaDTO ToDto(Categoria c)
        {
            return new CategoriaDTO { Id = c.Id, Nombre = c.Nombre };
        }
    }
}
