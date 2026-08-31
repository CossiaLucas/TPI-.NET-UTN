using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaDto>> ObtenerTodasAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaDto
                {
                    Id = c.IdCategoria,
                    Nombre = c.Nombre
                })
                .ToListAsync();
        }

        public async Task<CategoriaDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.IdCategoria == id)
                .Select(c => new CategoriaDto
                {
                    Id = c.IdCategoria,
                    Nombre = c.Nombre
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoriaDto> CrearAsync(CategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la categoría es obligatorio.");

            string nombre = dto.Nombre.Trim();

            bool existe = await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());

            if (existe)
                throw new InvalidOperationException(
                    "Ya existe una categoría con ese nombre.");

            var categoria = new Categoria
            {
                Nombre = nombre
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            dto.Id = categoria.IdCategoria;
            dto.Nombre = categoria.Nombre;

            return dto;
        }

        public async Task<bool> ModificarAsync(int id, CategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la categoría es obligatorio.");

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return false;

            string nombre = dto.Nombre.Trim();

            bool existe = await _context.Categorias
                .AnyAsync(c =>
                    c.IdCategoria != id &&
                    c.Nombre.ToLower() == nombre.ToLower());

            if (existe)
                throw new InvalidOperationException(
                    "Ya existe otra categoría con ese nombre.");

            categoria.Nombre = nombre;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return false;

            bool tieneProductos = await _context.Productos
                .AnyAsync(p => p.IdCategoria == id);

            if (tieneProductos)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar la categoría porque tiene productos asociados.");
            }

            _context.Categorias.Remove(categoria);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}