using Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IMetodoDePagoRepository
    {
        Task<IEnumerable<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);

    }
}
