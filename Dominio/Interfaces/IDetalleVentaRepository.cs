using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IDetalleVentaRepository
    {
        Task AddAsync(DetalleVenta detalle);
        Task<bool> UpdateAsync(DetalleVenta detalle);
        Task<bool> DeleteAsync(int idDetalle);
        Task<DetalleVenta?> GetByIdAsync(int idDetalle);
        Task<List<DetalleVenta>> GetByVentaAsync(int nroVenta);
    }
}