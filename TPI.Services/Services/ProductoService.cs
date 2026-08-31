using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerTodosAsync()
        {
            DateTime ahora = DateTime.Now;

            return await _context.Productos
                .AsNoTracking()
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.Nombre,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Stock = p.Stock,
                    FotoUrl = p.FotoUrl,

                    PrecioActual = p.Precios
                        .Where(pr => pr.FechaDesde <= ahora)
                        .OrderByDescending(pr => pr.FechaDesde)
                        .Select(pr => (decimal?)pr.Valor)
                        .FirstOrDefault()
                })
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
        {
            DateTime ahora = DateTime.Now;

            return await _context.Productos
                .AsNoTracking()
                .Where(p => p.IdProducto == id)
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.Nombre,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Stock = p.Stock,
                    FotoUrl = p.FotoUrl,

                    PrecioActual = p.Precios
                        .Where(pr => pr.FechaDesde <= ahora)
                        .OrderByDescending(pr => pr.FechaDesde)
                        .Select(pr => (decimal?)pr.Valor)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerPorCategoriaAsync(int idCategoria)
        {
            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == idCategoria);

            if (!categoriaExiste)
                throw new KeyNotFoundException(
                    "La categoría indicada no existe.");

            DateTime ahora = DateTime.Now;

            return await _context.Productos
                .AsNoTracking()
                .Where(p => p.IdCategoria == idCategoria)
                .Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    IdCategoria = p.IdCategoria,
                    NombreCategoria = p.Categoria.Nombre,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Stock = p.Stock,
                    FotoUrl = p.FotoUrl,

                    PrecioActual = p.Precios
                        .Where(pr => pr.FechaDesde <= ahora)
                        .OrderByDescending(pr => pr.FechaDesde)
                        .Select(pr => (decimal?)pr.Valor)
                        .FirstOrDefault()
                })
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<ProductoDto> CrearAsync(ProductoDto dto)
        {
            ValidarProducto(dto);

            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == dto.IdCategoria);

            if (!categoriaExiste)
                throw new KeyNotFoundException(
                    "La categoría indicada no existe.");

            var producto = new Producto
            {
                IdCategoria = dto.IdCategoria,
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion.Trim(),
                Stock = dto.Stock,
                FotoUrl = string.IsNullOrWhiteSpace(dto.FotoUrl)
                    ? null
                    : dto.FotoUrl.Trim()
            };

            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            dto.IdProducto = producto.IdProducto;

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstAsync(c => c.IdCategoria == producto.IdCategoria);

            dto.NombreCategoria = categoria.Nombre;
            dto.Nombre = producto.Nombre;
            dto.Descripcion = producto.Descripcion;
            dto.Stock = producto.Stock;
            dto.FotoUrl = producto.FotoUrl;
            dto.PrecioActual = null;

            return dto;
        }

        public async Task<bool> ModificarAsync(int id, ProductoDto dto)
        {
            ValidarProducto(dto);

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
                return false;

            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == dto.IdCategoria);

            if (!categoriaExiste)
                throw new KeyNotFoundException(
                    "La categoría indicada no existe.");

            producto.IdCategoria = dto.IdCategoria;
            producto.Nombre = dto.Nombre.Trim();
            producto.Descripcion = dto.Descripcion.Trim();
            producto.Stock = dto.Stock;
            producto.FotoUrl = string.IsNullOrWhiteSpace(dto.FotoUrl)
                ? null
                : dto.FotoUrl.Trim();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
                return false;

            bool tieneCarrito = await _context.ItemsCarrito
                .AnyAsync(i => i.IdProducto == id);

            if (tieneCarrito)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el producto porque está presente en un carrito.");
            }

            bool tieneVentas = await _context.DetallesVenta
                .AnyAsync(d => d.IdProducto == id);

            if (tieneVentas)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el producto porque forma parte de una venta.");
            }

            _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();

            return true;
        }

        private static void ValidarProducto(ProductoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.IdCategoria <= 0)
                throw new ArgumentException(
                    "Debe indicar una categoría válida.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException(
                    "El nombre del producto es obligatorio.");

            if (dto.Nombre.Trim().Length > 150)
                throw new ArgumentException(
                    "El nombre del producto no puede superar los 150 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                throw new ArgumentException(
                    "La descripción del producto es obligatoria.");

            if (dto.Stock < 0)
                throw new ArgumentException(
                    "El stock no puede ser negativo.");

            if (!string.IsNullOrWhiteSpace(dto.FotoUrl) &&
                dto.FotoUrl.Trim().Length > 500)
            {
                throw new ArgumentException(
                    "La URL de la foto no puede superar los 500 caracteres.");
            }
        }
    }
}