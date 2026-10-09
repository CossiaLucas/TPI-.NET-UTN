using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IVentaRepository
    {
        Task AddAsync(Venta venta);
        Task<bool> UpdateAsync(Venta venta);
        Task<bool> DeleteAsync(int nroVenta);
        Task<Venta?> GetByNroVentaAsync(int nroVenta);
        Task<List<Venta>> GetAllAsync();
    }
}