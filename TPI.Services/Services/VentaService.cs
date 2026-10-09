using Dominio.Entities;
using Dominio.Interfaces;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.Services.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;

        public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<VentaDTO> CreateAsync(VentaDTO dto)
        {
            Validar(dto);

            var venta = new Venta
            {
                NroVenta = dto.NroVenta,
                IdUsuario = dto.IdUsuario,
                IdMetodoPago = dto.IdMetodoPago,
                Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "NoPagado" : dto.Estado
            };

            foreach (var detalleDto in dto.Detalles)
                venta.Detalles.Add(await CrearDetalleAsync(dto.NroVenta, detalleDto));

            venta.Total = venta.Detalles.Sum(d => d.Subtotal);
            await _ventaRepository.AddAsync(venta);
            return ToDto(venta);
        }

        public async Task<bool> DeleteAsync(int nroVenta)
        {
            return await _ventaRepository.DeleteAsync(nroVenta);
        }

        public async Task<IEnumerable<VentaDTO>> GetAllAsync()
        {
            var ventas = await _ventaRepository.GetAllAsync();
            return ventas.Select(ToDto).ToList();
        }

        public async Task<VentaDTO?> GetByNroVentaAsync(int nroVenta)
        {
            var venta = await _ventaRepository.GetByNroVentaAsync(nroVenta);
            return venta is null ? null : ToDto(venta);
        }

        public async Task<bool> UpdateAsync(VentaDTO dto)
        {
            Validar(dto);

            var venta = await _ventaRepository.GetByNroVentaAsync(dto.NroVenta);
            if (venta is null)
                return false;

            venta.IdUsuario = dto.IdUsuario;
            venta.IdMetodoPago = dto.IdMetodoPago;
            venta.Estado = string.IsNullOrWhiteSpace(dto.Estado) ? venta.Estado : dto.Estado;

            var detallesNuevos = new List<DetalleVenta>();
            foreach (var detalleDto in dto.Detalles)
                detallesNuevos.Add(await CrearDetalleAsync(dto.NroVenta, detalleDto));

            venta.Detalles.Clear();
            foreach (var detalle in detallesNuevos)
                venta.Detalles.Add(detalle);

            venta.Total = venta.Detalles.Sum(d => d.Subtotal);
            return await _ventaRepository.UpdateAsync(venta);
        }

        private async Task<DetalleVenta> CrearDetalleAsync(int nroVenta, DetalleVentaDTO dto)
        {
            if (dto.IdProducto <= 0)
                throw new ArgumentException("El producto debe ser válido.", nameof(dto.IdProducto));
            if (dto.Cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.", nameof(dto.Cantidad));

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
                IdVenta = nroVenta,
                IdProducto = dto.IdProducto,
                Cantidad = dto.Cantidad,
                PrecioUnitario = Math.Round(precio, 2),
                Subtotal = Math.Round(dto.Cantidad * precio, 2)
            };
        }

        private static void Validar(VentaDTO dto)
        {
            if (dto.NroVenta <= 0)
                throw new ArgumentException("El número de venta debe ser mayor que cero.", nameof(dto.NroVenta));
            if (dto.IdUsuario <= 0)
                throw new ArgumentException("El usuario debe ser válido.", nameof(dto.IdUsuario));
            if (dto.IdMetodoPago <= 0)
                throw new ArgumentException("El método de pago debe ser válido.", nameof(dto.IdMetodoPago));
            if (dto.Detalles.Count == 0)
                throw new ArgumentException("La venta debe tener al menos un detalle.", nameof(dto.Detalles));
        }

        private static VentaDTO ToDto(Venta venta)
        {
            return new VentaDTO
            {
                NroVenta = venta.NroVenta,
                IdUsuario = venta.IdUsuario,
                NombreUsuario = venta.Usuario is null
                    ? string.Empty
                    : $"{venta.Usuario.Nombre} {venta.Usuario.Apellido}",
                IdMetodoPago = venta.IdMetodoPago,
                NombreMetodoPago = venta.MetodoPago?.Nombre ?? string.Empty,
                Total = venta.Total,
                Estado = venta.Estado,
                Detalles = venta.Detalles.Select(d => new DetalleVentaDTO
                {
                    IdDetalle = d.IdDetalle,
                    IdVenta = d.IdVenta,
                    IdProducto = d.IdProducto,
                    NombreProducto = d.Producto?.Nombre ?? string.Empty,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }
    }
}