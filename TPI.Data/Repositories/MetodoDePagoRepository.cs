using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;

namespace TPI.Data.Repositories
{
    public class MetodoDePagoRepository : IMetodoDePagoRepository
    {
        private readonly AppDbContext _context;

        public MetodoDePagoRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<MetodoPago>> GetAllAsync() =>
            await _context.MetodosPago.AsNoTracking().OrderBy(m => m.Nombre).ToListAsync();

        public async Task<MetodoPago?> GetByIdAsync(int id) =>
            await _context.MetodosPago.AsNoTracking().FirstOrDefaultAsync(m => m.IdMetodoPago == id);

        public async Task AddAsync(MetodoPago metodoPago)
        {
            _context.MetodosPago.Add(metodoPago);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(MetodoPago metodoPago)
        {
            _context.MetodosPago.Update(metodoPago);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entidad = await _context.MetodosPago.FindAsync(id);
            if (entidad is null)
                return false;

            _context.MetodosPago.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
