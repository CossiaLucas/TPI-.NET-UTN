using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using TPI.Data.Context;

namespace TPI.Api
{
    /// <summary>
    /// Datos de demostración para poder probar los reportes: categorías, productos con precio,
    /// 2 clientes y ~40 ventas repartidas en los últimos 8 meses, en distintos estados.
    /// Solo se ejecuta si la base NO tiene ventas (nunca pisa datos cargados por el usuario).
    /// El stock de los productos demo es el stock "final" (no se descuenta por las ventas de ejemplo).
    /// </summary>
    public static class DemoDataSeeder
    {
        private record ClienteDemo(string Nombre, string Apellido, string Username, string Email, string Dni, string Telefono);
        private record ProductoDemo(string Nombre, string Descripcion, string Categoria, int Stock, decimal Precio);

        private static readonly ClienteDemo[] Clientes =
        {
            new("Lucía", "Fernández", "cliente1", "cliente1@gmail.com", "30111222", "1155550001"),
            new("Martín", "Gómez", "cliente2", "cliente2@gmail.com", "30333444", "1155550002"),
        };

        private const string PasswordClientesDemo = "cliente123";

        private static readonly ProductoDemo[] Productos =
        {
            new("Auricular Bluetooth", "Auricular inalámbrico con estuche de carga", "Electrónica", 25, 18500m),
            new("Mouse inalámbrico", "Mouse óptico 2.4 GHz", "Electrónica", 3, 7200m),
            new("Teclado mecánico", "Teclado mecánico retroiluminado", "Electrónica", 12, 32000m),
            new("Monitor 24 pulgadas", "Monitor Full HD IPS", "Electrónica", 4, 145000m),
            new("Lámpara LED de escritorio", "Lámpara regulable con puerto USB", "Hogar", 30, 9800m),
            new("Cafetera de filtro", "Cafetera eléctrica de 1,2 litros", "Hogar", 8, 38500m),
            new("Juego de sábanas", "Sábanas de algodón de dos plazas", "Hogar", 2, 22000m),
            new("Cuaderno A4", "Cuaderno tapa dura de 96 hojas", "Librería", 80, 3200m),
            new("Mochila urbana", "Mochila con compartimento para notebook", "Librería", 15, 28000m),
            new("Pack de biromes", "Pack de 12 biromes de colores", "Librería", 120, 1800m),
        };

        public static async Task SeedAsync(AppDbContext context)
        {
            // Si ya hay ventas, no se toca nada.
            if (await context.Ventas.AnyAsync())
                return;

            var metodos = await context.MetodosPago.ToListAsync();
            if (metodos.Count == 0)
                return;

            var compradores = await ObtenerOCrearUsuariosAsync(context);
            var productos = await ObtenerOCrearProductosAsync(context);

            var vendibles = productos.Where(p => p.ObtenerPrecioActual() is > 0).ToList();
            if (compradores.Count == 0 || vendibles.Count == 0)
                return;

            GenerarVentas(context, compradores, vendibles, metodos);
            await context.SaveChangesAsync();
        }

        private static async Task<List<Usuario>> ObtenerOCrearUsuariosAsync(AppDbContext context)
        {
            var usuarios = await context.Usuarios.ToListAsync();

            foreach (var c in Clientes)
            {
                var existe = usuarios.Any(u => u.Username == c.Username || u.Email == c.Email || u.DNI == c.Dni);
                if (existe) continue;

                var nuevo = new Usuario(
                    nombre: c.Nombre,
                    apellido: c.Apellido,
                    username: c.Username,
                    email: c.Email,
                    password: PasswordClientesDemo,
                    dni: c.Dni,
                    fechaNacimiento: new DateTime(1995, 3, 10),
                    telefono: c.Telefono,
                    isAdmin: false);

                context.Usuarios.Add(nuevo);
                usuarios.Add(nuevo);
            }

            return usuarios;
        }

        private static async Task<List<Producto>> ObtenerOCrearProductosAsync(AppDbContext context)
        {
            var categorias = await context.Categorias.ToListAsync();
            var productos = await context.Productos.Include(p => p.Precios).ToListAsync();

            foreach (var spec in Productos)
            {
                if (productos.Any(p => string.Equals(p.Nombre, spec.Nombre, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var categoria = categorias.FirstOrDefault(c =>
                    string.Equals(c.Nombre, spec.Categoria, StringComparison.OrdinalIgnoreCase));
                if (categoria is null)
                {
                    categoria = new Categoria { Nombre = spec.Categoria };
                    context.Categorias.Add(categoria);
                    categorias.Add(categoria);
                }

                var producto = new Producto
                {
                    Nombre = spec.Nombre,
                    Descripcion = spec.Descripcion,
                    Stock = spec.Stock,
                    Categoria = categoria,
                    Precios = { new Precio { FechaDesde = DateTime.Now.AddMonths(-10), Valor = spec.Precio } }
                };

                context.Productos.Add(producto);
                productos.Add(producto);
            }

            return productos;
        }

        private static void GenerarVentas(
            AppDbContext context, List<Usuario> compradores, List<Producto> productos, List<MetodoPago> metodos)
        {
            var rnd = new Random(2026); // semilla fija: los datos de ejemplo son siempre los mismos
            var ahora = DateTime.Now;

            for (var i = 0; i < 42; i++)
            {
                var fecha = ahora.Date.AddDays(-rnd.Next(0, 240)).AddHours(rnd.Next(9, 21)).AddMinutes(rnd.Next(0, 60));
                if (fecha > ahora) fecha = ahora.AddMinutes(-5);

                var sorteo = rnd.Next(100);
                var estado = sorteo < 60 ? EstadoVenta.Pagado
                           : sorteo < 85 ? EstadoVenta.NoPagado
                           : EstadoVenta.Cancelado;

                var venta = new Venta
                {
                    Fecha = fecha,
                    Usuario = compradores[rnd.Next(compradores.Count)],
                    MetodoPago = metodos[rnd.Next(metodos.Count)],
                    Estado = estado
                };

                var elegidos = productos.OrderBy(_ => rnd.Next()).Take(rnd.Next(1, 4));
                foreach (var producto in elegidos)
                {
                    var cantidad = rnd.Next(1, 4);
                    var precio = producto.ObtenerPrecioActual()!.Value;
                    venta.Detalles.Add(new DetalleVenta
                    {
                        Producto = producto,
                        Cantidad = cantidad,
                        PrecioUnitario = precio,
                        Subtotal = cantidad * precio
                    });
                }

                venta.Total = venta.Detalles.Sum(d => d.Subtotal);
                context.Ventas.Add(venta);
            }
        }
    }
}
