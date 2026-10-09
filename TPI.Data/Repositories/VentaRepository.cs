using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;

namespace TPI.Data.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly AppDbContext _context;

        public VentaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Venta venta)
        {
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int nroVenta)
        {
            var venta = await _context.Ventas.FindAsync(nroVenta);
            if (venta is null)
                return false;

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Venta>> GetAllAsync()
        {
            return await Query()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Venta?> GetByNroVentaAsync(int nroVenta)
        {
            return await Query()
                .FirstOrDefaultAsync(v => v.NroVenta == nroVenta);
        }

        public async Task<bool> UpdateAsync(Venta venta)
        {
            if (_context.Entry(venta).State == EntityState.Detached)
                _context.Ventas.Update(venta);

            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<Venta> Query()
        {
            return _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.MetodoPago)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto);
        }
    }
}