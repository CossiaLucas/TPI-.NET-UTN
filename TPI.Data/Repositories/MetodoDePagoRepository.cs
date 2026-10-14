using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
