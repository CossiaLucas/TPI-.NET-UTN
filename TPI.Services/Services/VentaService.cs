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
        private readonly IMetodoDePagoRepository _metodoPagoRepository;

        public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository, IMetodoDePagoRepository metodoPagoRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _metodoPagoRepository = metodoPagoRepository;
        }

        public async Task<VentaDTO> CreateAsync(int idUsuario, VentaCreateDTO dto)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El usuario debe ser válido.");
            await ValidarMetodoPagoAsync(dto.IdMetodoPago);


            var lineas = AgruparLineas(dto.Detalles);

            var productos = new Dictionary<int, Producto>();
            foreach (var (idProducto, cantidad) in lineas)
            {
                var producto = await ObtenerProductoAsync(idProducto);
                ValidarStock(producto, cantidad);
                productos[idProducto] = producto;
            }


            var venta = new Venta
            {
                Fecha = DateTime.Now,
                IdUsuario = idUsuario,
                IdMetodoPago = dto.IdMetodoPago,
                Estado = EstadoVenta.NoPagado
            };

            foreach (var (idProducto, cantidad) in lineas)
            {
                var producto = productos[idProducto];
                producto.Stock -= cantidad;
                venta.Detalles.Add(CrearDetalle(producto, cantidad, PrecioVigente(producto)));
            }

            venta.Total = venta.Detalles.Sum(d => d.Subtotal);
            await _ventaRepository.AddAsync(venta);
            var creada = await _ventaRepository.GetByNroVentaAsync(venta.NroVenta);
            return ToDto(creada ?? venta);
        }

        public async Task<bool> DeleteAsync(int nroVenta)
        {
            var venta = await _ventaRepository.GetByNroVentaAsync(nroVenta);
            if (venta is null)
                return false;

            if (venta.Estado != EstadoVenta.Cancelado)
                await DevolverStockAsync(venta);

            return await _ventaRepository.DeleteAsync(nroVenta);
        }

        public async Task<IEnumerable<VentaDTO>> GetAllAsync(string? estado = null)
        {
            var ventas = await _ventaRepository.GetAllAsync();
            return FiltrarPorEstado(ventas, estado).Select(ToDto).ToList();
        }

        public async Task<VentaDTO?> GetByNroVentaAsync(int nroVenta)
        {
            var venta = await _ventaRepository.GetByNroVentaAsync(nroVenta);
            return venta is null ? null : ToDto(venta);
        }

        public async Task<IEnumerable<VentaDTO>> GetByUsuarioAsync(int idUsuario, string? estado = null)
        {
            var ventas = await _ventaRepository.GetAllAsync();
            return FiltrarPorEstado(ventas.Where(v => v.IdUsuario == idUsuario), estado).Select(ToDto).ToList();
        }

        public async Task<bool> UpdateAsync(VentaUpdateDTO dto)
        {
            if (!EstadoVenta.EsValido(dto.Estado))
                throw new ArgumentException($"Estado inválido. Valores permitidos: {string.Join(", ", new[] { EstadoVenta.Pagado, EstadoVenta.NoPagado, EstadoVenta.Cancelado })}.");
            await ValidarMetodoPagoAsync(dto.IdMetodoPago);

            var venta = await _ventaRepository.GetByNroVentaAsync(dto.NroVenta);
            if (venta is null)
                return false;

            if (venta.Estado == EstadoVenta.Cancelado)
                throw new InvalidOperationException("Una venta cancelada no se puede modificar.");

            venta.IdMetodoPago = dto.IdMetodoPago;

            if (dto.Estado == EstadoVenta.Cancelado)
            {
                await DevolverStockAsync(venta);
                venta.Estado = EstadoVenta.Cancelado;
                return await _ventaRepository.UpdateAsync(venta);
            }

            var nuevas = AgruparLineas(dto.Detalles);
            var actuales = venta.Detalles
                .GroupBy(d => d.IdProducto)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

            var hayCambios = actuales.Count != nuevas.Count
                            || nuevas.Any(n => !actuales.TryGetValue(n.Key, out var c) || c != n.Value);

            if (hayCambios)
            {
                if (venta.Estado != EstadoVenta.NoPagado)
                    throw new InvalidOperationException(
                        "Solo se pueden modificar los productos de una venta NoPagado.");

                await ReemplazarLineasAsync(venta, actuales, nuevas);
            }

            venta.Estado = dto.Estado;
            return await _ventaRepository.UpdateAsync(venta);
        }

        //-------------------------------------------------

        private static IEnumerable<Venta> FiltrarPorEstado(IEnumerable<Venta> ventas, string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return ventas;

            if (!EstadoVenta.EsValido(estado))
                throw new ArgumentException(
                    $"Estado inválido. Valores permitidos: {string.Join(", ", EstadoVenta.Todos)}.");

            return ventas.Where(v => v.Estado == estado);
        }



        private async Task ReemplazarLineasAsync(Venta venta, Dictionary<int, int> actuales, Dictionary<int, int> nuevas)
        {
            // Las lineas que ya existian conservan el precio con el que se vendieron
            var preciosPrevios = venta.Detalles
                .GroupBy(d => d.IdProducto)
                .ToDictionary(g => g.Key, g => g.First().PrecioUnitario);

            var productos = new Dictionary<int, Producto>();
            foreach (var id in actuales.Keys.Union(nuevas.Keys))
                productos[id] = await ObtenerProductoAsync(id);

            // Validar primero: solo hace falta stock para el aumento de cantidad
            foreach (var (id, cantidadNueva) in nuevas)
            {
                var aumento = cantidadNueva - actuales.GetValueOrDefault(id);
                if (aumento > productos[id].Stock)
                    throw new InvalidOperationException(
                        $"Stock insuficiente de '{productos[id].Nombre}'. Disponible: {productos[id].Stock}.");
            }

            // Aplicar: si la cantidad baja o la línea se quita, el stock vuelve
            foreach (var (id, producto) in productos)
                producto.Stock -= nuevas.GetValueOrDefault(id) - actuales.GetValueOrDefault(id);

            venta.Detalles.Clear();
            foreach (var (id, cantidad) in nuevas)
            {
                var precio = preciosPrevios.TryGetValue(id, out var previo)
                    ? previo
                    : PrecioVigente(productos[id]);
                venta.Detalles.Add(CrearDetalle(productos[id], cantidad, precio));
            }

            venta.Total = venta.Detalles.Sum(d => d.Subtotal);
        }

        private async Task DevolverStockAsync(Venta venta)
        {
            foreach (var detalle in venta.Detalles)
            {
                var producto = await ObtenerProductoAsync(detalle.IdProducto);
                producto.Stock += detalle.Cantidad;
            }
        }

        private async Task<Producto> ObtenerProductoAsync(int idProducto)
        {
            var producto = await _productoRepository.GetByIdAsync(idProducto);
            return producto ?? throw new InvalidOperationException($"El producto {idProducto} no existe.");
        }

        private async Task ValidarMetodoPagoAsync(int idMetodoPago)
        {
            if (idMetodoPago <= 0 || await _metodoPagoRepository.GetByIdAsync(idMetodoPago) is null)
                throw new ArgumentException("El método de pago no es válido.");
        }

        private static void ValidarStock(Producto producto, int cantidad)
        {
            if (cantidad > producto.Stock)
                throw new InvalidOperationException(
                    $"Stock insuficiente de '{producto.Nombre}'. Disponible: {producto.Stock}.");
        }

        // Si el mismo producto viene en dos lineas, se suman las cantidades
        private static Dictionary<int, int> AgruparLineas(IEnumerable<DetalleVentaCreateDTO>? detalles)
        {
            var lista = detalles?.ToList() ?? new List<DetalleVentaCreateDTO>();
            if (lista.Count == 0)
                throw new ArgumentException("La venta debe tener al menos un producto.");

            foreach (var d in lista)
            {
                if (d.IdProducto <= 0)
                    throw new ArgumentException("El producto debe ser válido.");
                if (d.Cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }

            return lista.GroupBy(d => d.IdProducto)
                        .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));
        }

        private static decimal PrecioVigente(Producto producto)
        {
            var precio = producto.ObtenerPrecioActual() ?? 0;
            if (precio <= 0)
                throw new InvalidOperationException($"El producto '{producto.Nombre}' no tiene un precio vigente.");
            return precio;
        }

        private static DetalleVenta CrearDetalle(Producto producto, int cantidad, decimal precio) => new()
        {
            IdProducto = producto.Id,
            Cantidad = cantidad,
            PrecioUnitario = Math.Round(precio, 2),
            Subtotal = Math.Round(cantidad * precio, 2)
        };


        private static VentaDTO ToDto(Venta venta)
        {
            return new VentaDTO
            {
                NroVenta = venta.NroVenta,
                Fecha = venta.Fecha,
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