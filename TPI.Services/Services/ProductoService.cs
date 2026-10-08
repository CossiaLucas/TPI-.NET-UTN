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

        public async Task<ProductoDTO> CreateAsync(ProductoDTO dto)
        {
            var prod = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Stock = dto.Stock,
                IdCategoria = dto.IdCategoria,
                FotoUrl = dto.FotoUrl
            };
            prod.Validar();
            Producto.ValidarPrecio(dto.PrecioActual);

            var categoria = await _categoriaRepository.GetByIdAsync(dto.IdCategoria);
            if (categoria is null)
                throw new InvalidOperationException("Categoria no encontrada.");

            // UN PRECIO DE >0 SIGNIFICA "todavía sin precio": no se registra.
            if (dto.PrecioActual is > 0)
                prod.Precios.Add(NuevoPrecio(dto.PrecioActual.Value));

            await _productoRepository.AddAsync(prod);
            return ToDto(prod);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productoRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductoDTO>> GetAllAsync()
        {
            var items = await _productoRepository.GetAllAsync();
            return items.Select(ToDto).ToList();
        }

        public async Task<ProductoDTO?> GetByIdAsync(int id)
        {
            var entidad = await _productoRepository.GetByIdAsync(id);
            return entidad is null ? null : ToDto(entidad);
        }

        public async Task<bool> UpdateAsync(ProductoDTO dto)
        {
            var entidad = await _productoRepository.GetByIdAsync(dto.Id);
            if (entidad is null)
                return false;

            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.Stock = dto.Stock;
            entidad.IdCategoria = dto.IdCategoria;
            entidad.FotoUrl = dto.FotoUrl;
            entidad.Validar();
            Producto.ValidarPrecio(dto.PrecioActual);

            var categoria = await _categoriaRepository.GetByIdAsync(dto.IdCategoria);
            if (categoria is null)
                throw new InvalidOperationException("Categoria no encontrada.");

            if (dto.PrecioActual is > 0 && dto.PrecioActual != entidad.ObtenerPrecioActual())
                entidad.Precios.Add(NuevoPrecio(dto.PrecioActual.Value));

            return await _productoRepository.UpdateAsync(entidad);
        }

        private static Precio NuevoPrecio(decimal valor)
        {
            return new Precio
            {
                FechaDesde = DateTime.Now,
                Valor = Math.Round(valor, 2)
            };
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
                NombreCategoria = p.Categoria?.Nombre ?? string.Empty,
                FotoUrl = p.FotoUrl,
                PrecioActual = p.ObtenerPrecioActual()
            };
        }
    }
}
