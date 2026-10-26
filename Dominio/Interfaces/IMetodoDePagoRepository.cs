using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IMetodoDePagoRepository
    {
        Task<IEnumerable<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);
        Task AddAsync(MetodoPago metodoPago);
        Task<bool> UpdateAsync(MetodoPago metodoPago);
        Task<bool> DeleteAsync(int id);
    }
}
