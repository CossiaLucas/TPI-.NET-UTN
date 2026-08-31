using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;

namespace TPI.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;
        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var entidad = await _context.Categorias.FindAsync(id);
            if (entidad != null)
            {
                _context.Categorias.Remove(entidad);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _context.Categorias.AsNoTracking().ToListAsync();
        }
        public async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}