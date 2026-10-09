using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;

namespace TPI.Data.Repositories
{
    public class DetalleVentaRepository : IDetalleVentaRepository
    {
        private readonly AppDbContext _context;

        public DetalleVentaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DetalleVenta detalle)
        {
            _context.DetallesVenta.Add(detalle);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int idDetalle)
        {
            var detalle = await _context.DetallesVenta.FindAsync(idDetalle);
            if (detalle is null)
                return false;

            _context.DetallesVenta.Remove(detalle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DetalleVenta?> GetByIdAsync(int idDetalle)
        {
            return await Query()
                .FirstOrDefaultAsync(d => d.IdDetalle == idDetalle);
        }

        public async Task<List<DetalleVenta>> GetByVentaAsync(int nroVenta)
        {
            return await Query()
                .AsNoTracking()
                .Where(d => d.IdVenta == nroVenta)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(DetalleVenta detalle)
        {
            if (_context.Entry(detalle).State == EntityState.Detached)
                _context.DetallesVenta.Update(detalle);

            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<DetalleVenta> Query()
        {
            return _context.DetallesVenta
                .Include(d => d.Producto)
                .Include(d => d.Venta);
        }
    }
}