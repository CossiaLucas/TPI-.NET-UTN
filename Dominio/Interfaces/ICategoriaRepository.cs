using Dominio.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Dominio.Interfaces
{
    public interface ICategoriaRepository
    {
        Task AddAsync(Categoria categoria);
        Task<bool> UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(int id);
        Task<Categoria?> GetByIdAsync(int id);
        Task<List<Categoria>> GetAllAsync();
    }
}