using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;

namespace TPI.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var entidad = await _context.Productos.FindAsync(id);
            if (entidad != null)
            {
                _context.Productos.Remove(entidad);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .ToListAsync();
        }
        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<bool> UpdateAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}