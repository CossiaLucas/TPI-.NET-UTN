using Dominio.Entities;
using Dominio.Interfaces;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        public ProductoService(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
        }
        public async Task<ProductoDTO> CreateAsync(ProductoCreateDTO dto)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(dto.IdCategoria);
            if (categoria is null)
                throw new InvalidOperationException("Categoria no encontrada.");
            var prod = new Producto
                {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Stock = dto.Stock,
                IdCategoria = dto.IdCategoria,
                FotoUrl = dto.FotoUrl
            };
            await _productoRepository.AddAsync(prod);
            return ToDto(prod);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _productoRepository.DeleteAsync(id);
        }
        public async Task<List<ProductoDTO>> GetAllAsync()
        {
            var items = await _productoRepository.GetAllAsync();
            return items.Select(ToDto).ToList();
        }
        public async Task<ProductoDTO?> GetByIdAsync(int id)
        {
            var entidad = await _productoRepository.GetByIdAsync(id);
            return entidad is null ? null : ToDto(entidad);
        }
        public async Task<bool> UpdateAsync(ProductoUpdateDTO dto)
        {
            var entidad = await _productoRepository.GetByIdAsync(dto.Id);
            if (entidad is null)
                return false;
            var categoria = await _categoriaRepository.GetByIdAsync(dto.IdCategoria);
            if (categoria is null)
                throw new InvalidOperationException("Categoria no encontrada.");
            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.Stock = dto.Stock;
            entidad.IdCategoria = dto.IdCategoria;
            entidad.FotoUrl = dto.FotoUrl;
            return await _productoRepository.UpdateAsync(entidad);
        }
        private static ProductoDTO ToDto(Producto p)
        {
            return new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Stock = p.Stock,
                IdCategoria = p.IdCategoria,
                CategoriaNombre = p.Categoria?.Nombre ?? string.Empty,
                FotoUrl = p.FotoUrl
            };
        }
    }
}