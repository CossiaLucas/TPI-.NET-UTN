using Dominio.Entities;
using Dominio.Interfaces;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly IDetalleVentaRepository _detalleRepository;
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;

        public DetalleVentaService(
            IDetalleVentaRepository detalleRepository,
            IVentaRepository ventaRepository,
            IProductoRepository productoRepository)
        {
            _detalleRepository = detalleRepository;
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<DetalleVentaDTO> CreateAsync(DetalleVentaDTO dto)
        {
            Validar(dto);

            var venta = await _ventaRepository.GetByNroVentaAsync(dto.IdVenta);
            if (venta is null)
                throw new InvalidOperationException("Venta no encontrada.");

            var detalle = await CrearEntidadAsync(dto);
            await _detalleRepository.AddAsync(detalle);
            venta.Total = venta.Detalles.Sum(d => d.Subtotal) + detalle.Subtotal;
            await _ventaRepository.UpdateAsync(venta);

            return ToDto(detalle);
        }

        public async Task<bool> DeleteAsync(int idDetalle)
        {
            var detalle = await _detalleRepository.GetByIdAsync(idDetalle);
            if (detalle is null)
                return false;

            var deleted = await _detalleRepository.DeleteAsync(idDetalle);
            if (deleted && detalle.Venta is not null)
            {
                detalle.Venta.Total = detalle.Venta.Detalles
                    .Where(d => d.IdDetalle != idDetalle)
                    .Sum(d => d.Subtotal);
                await _ventaRepository.UpdateAsync(detalle.Venta);
            }

            return deleted;
        }

        public async Task<DetalleVentaDTO?> GetByIdAsync(int idDetalle)
        {
            var detalle = await _detalleRepository.GetByIdAsync(idDetalle);
            return detalle is null ? null : ToDto(detalle);
        }

        public async Task<IEnumerable<DetalleVentaDTO>> GetByVentaAsync(int nroVenta)
        {
            var detalles = await _detalleRepository.GetByVentaAsync(nroVenta);
            return detalles.Select(ToDto).ToList();
        }

        public async Task<bool> UpdateAsync(DetalleVentaDTO dto)
        {
            Validar(dto);

            var detalle = await _detalleRepository.GetByIdAsync(dto.IdDetalle);
            if (detalle is null)
                return false;

            if (detalle.IdVenta != dto.IdVenta)
                throw new InvalidOperationException("No se puede mover un detalle a otra venta.");

            var actualizado = await CrearEntidadAsync(dto);
            detalle.IdProducto = actualizado.IdProducto;
            detalle.Cantidad = actualizado.Cantidad;
            detalle.PrecioUnitario = actualizado.PrecioUnitario;
            detalle.Subtotal = actualizado.Subtotal;

            var updated = await _detalleRepository.UpdateAsync(detalle);
            if (updated && detalle.Venta is not null)
            {
                detalle.Venta.Total = detalle.Venta.Detalles.Sum(d => d.Subtotal);
                await _ventaRepository.UpdateAsync(detalle.Venta);
            }

            return updated;
        }

        private async Task<DetalleVenta> CrearEntidadAsync(DetalleVentaDTO dto)
        {
            var producto = await _productoRepository.GetByIdAsync(dto.IdProducto);
            if (producto is null)
                throw new InvalidOperationException("Producto no encontrado.");

            var precio = dto.PrecioUnitario > 0
                ? dto.PrecioUnitario
                : producto.ObtenerPrecioActual() ?? 0;

            if (precio <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor que cero.", nameof(dto.PrecioUnitario));

            return new DetalleVenta
            {
                IdDetalle = dto.IdDetalle,
                IdVenta = dto.IdVenta,
                IdProducto = dto.IdProducto,
                Cantidad = dto.Cantidad,
                PrecioUnitario = Math.Round(precio, 2),
                Subtotal = Math.Round(dto.Cantidad * precio, 2)
            };
        }

        private static void Validar(DetalleVentaDTO dto)
        {
            if (dto.IdVenta <= 0)
                throw new ArgumentException("La venta debe ser válida.", nameof(dto.IdVenta));
            if (dto.IdProducto <= 0)
                throw new ArgumentException("El producto debe ser válido.", nameof(dto.IdProducto));
            if (dto.Cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(dto.Cantidad));
        }

        private static DetalleVentaDTO ToDto(DetalleVenta detalle)
        {
            return new DetalleVentaDTO
            {
                IdDetalle = detalle.IdDetalle,
                IdVenta = detalle.IdVenta,
                IdProducto = detalle.IdProducto,
                NombreProducto = detalle.Producto?.Nombre ?? string.Empty,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = detalle.Subtotal
            };
        }
    }
}