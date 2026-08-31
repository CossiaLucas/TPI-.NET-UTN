using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IProductoRepository
    {
        Task AddAsync(Producto producto);
        Task<bool> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
        Task<Producto?> GetByIdAsync(int id);
        Task<List<Producto>> GetAllAsync();
    }
}