using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly IMetodoDePagoRepository _repository;

        public MetodoPagoService(IMetodoDePagoRepository repository) => _repository = repository;

        public async Task<List<MetodoPagoDTO>> GetAllAsync()
        {
            var metodos = await _repository.GetAllAsync();
            return metodos
                .Select(m => new MetodoPagoDTO { IdMetodoPago = m.IdMetodoPago, Nombre = m.Nombre })
                .ToList();
        }
    }
}
