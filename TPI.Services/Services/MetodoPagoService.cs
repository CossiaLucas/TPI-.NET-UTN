using Dominio.Entities;
using Dominio.Interfaces;
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
            return metodos.Select(ToDto).ToList();
        }

        public async Task<MetodoPagoDTO?> GetByIdAsync(int id)
        {
            var metodo = await _repository.GetByIdAsync(id);
            return metodo is null ? null : ToDto(metodo);
        }

        public async Task<MetodoPagoDTO> CreateAsync(MetodoPagoDTO dto)
        {
            var metodo = new MetodoPago { Nombre = dto.Nombre };
            metodo.Validar();
            await ValidarNombreUnicoAsync(metodo.Nombre, idExcluido: null);

            await _repository.AddAsync(metodo);
            return ToDto(metodo);
        }

        public async Task<bool> UpdateAsync(MetodoPagoDTO dto)
        {
            var metodo = await _repository.GetByIdAsync(dto.IdMetodoPago);
            if (metodo is null)
                return false;

            metodo.Nombre = dto.Nombre;
            metodo.Validar();
            await ValidarNombreUnicoAsync(metodo.Nombre, idExcluido: metodo.IdMetodoPago);

            return await _repository.UpdateAsync(metodo);
        }

        public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

        private async Task ValidarNombreUnicoAsync(string nombre, int? idExcluido)
        {
            var existentes = await _repository.GetAllAsync();
            var duplicado = existentes.Any(m =>
                m.IdMetodoPago != idExcluido &&
                string.Equals(m.Nombre, nombre, StringComparison.OrdinalIgnoreCase));

            if (duplicado)
                throw new InvalidOperationException("Ya existe un método de pago con ese nombre.");
        }

        private static MetodoPagoDTO ToDto(MetodoPago m) =>
            new() { IdMetodoPago = m.IdMetodoPago, Nombre = m.Nombre };
    }
}
