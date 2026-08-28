using Dominio.Entities;
using Microsoft.EntityFrameworkCore;

namespace TPI.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Direccion> Direcciones => Set<Direccion>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Precio> Precios => Set<Precio>();
        public DbSet<Carrito> Carritos => Set<Carrito>();
        public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>();
        public DbSet<Favorito> Favoritos => Set<Favorito>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // Direccion
            // ==========================================
            modelBuilder.Entity<Direccion>(entity =>
            {
                entity.ToTable("direccion");

                entity.HasKey(e => e.IdDireccion);

                entity.Property(e => e.Calle)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Numero)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Piso)
                      .HasMaxLength(20);

                entity.Property(e => e.Departamento)
                      .HasMaxLength(20);

                entity.Property(e => e.CodigoPostal)
                      .HasMaxLength(10)
                      .IsRequired();

                // Relación 1:1 con Usuario
                entity.HasOne(e => e.Usuario)
                      .WithOne(u => u.Direccion)
                      .HasForeignKey<Usuario>(u => u.IdDireccion)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // Usuario
            // ==========================================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Apellido)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Username)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.Username)
                      .IsUnique();

                entity.Property(e => e.Clave)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.Property(e => e.Dni)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasIndex(e => e.Dni)
                      .IsUnique();

                entity.Property(e => e.FechaAlta)
                      .HasColumnType("datetime")
                      .IsRequired();

                entity.Property(e => e.Telefono)
                      .HasMaxLength(30);

                entity.Property(e => e.FechaNacimiento)
                      .HasColumnType("date");

                entity.Property(e => e.IsAdmin)
                      .HasDefaultValue(false);
            });

            // ==========================================
            // Categoria
            // ==========================================
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("categoria");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(e => e.Nombre)
                      .IsUnique();
            });

            // ==========================================
            // Producto
            // ==========================================
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("producto");

                entity.HasKey(e => e.IdProducto);

                entity.Property(e => e.Nombre)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnType("text");

                entity.Property(e => e.Stock)
                      .IsRequired();

                entity.Property(e => e.FotoUrl)
                      .HasMaxLength(500);

                entity.HasOne(e => e.Categoria)
                      .WithMany()
                      .HasForeignKey(e => e.IdCategoria)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // Precio
            // ==========================================
            modelBuilder.Entity<Precio>(entity =>
            {
                entity.ToTable("precio");

                entity.HasKey(e => new { e.IdProducto, e.FechaDesde });

                entity.Property(e => e.FechaDesde)
                      .HasColumnType("datetime");

                entity.Property(e => e.Valor)
                      .HasColumnName("precio")
                      .HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Producto)
                      .WithMany(p => p.Precios)
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==========================================
            // Carrito
            // ==========================================
            modelBuilder.Entity<Carrito>(entity =>
            {
                entity.ToTable("carrito");

                entity.HasKey(e => e.Id);

                // Usuario 1 : 1 Carrito
                entity.HasOne(e => e.Usuario)
                      .WithOne(u => u.Carrito)
                      .HasForeignKey<Carrito>(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);

                // Un usuario no puede tener más de un carrito
                entity.HasIndex(e => e.IdUsuario)
                      .IsUnique();

                entity.Property(e => e.Subtotal)
                      .HasColumnType("decimal(10,2)");
            });

            // ==========================================
            // ItemCarrito
            // ==========================================
            modelBuilder.Entity<ItemCarrito>(entity =>
            {
                entity.ToTable("itemCarrito");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Cantidad)
                      .IsRequired();

                entity.Property(e => e.Subtotal)
                      .HasColumnType("decimal(10,2)");

                // Carrito 1 : N ItemCarrito
                entity.HasOne(e => e.Carrito)
                      .WithMany(c => c.Items)
                      .HasForeignKey(e => e.IdCarrito)
                      .OnDelete(DeleteBehavior.Cascade);

                // Producto 1 : N ItemCarrito
                entity.HasOne(e => e.Producto)
                      .WithMany(p => p.ItemsCarrito)
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);

                // Un mismo producto no se repite dentro del mismo carrito
                entity.HasIndex(e => new { e.IdCarrito, e.IdProducto })
                      .IsUnique();
            });

            // ==========================================
            // Favorito
            // ==========================================
            modelBuilder.Entity<Favorito>(entity =>
            {
                entity.ToTable("favoritos");

                entity.HasKey(e => new { e.IdUsuario, e.IdProducto });

                entity.Property(e => e.FechaAgregado)
                      .HasColumnType("datetime")
                      .IsRequired();

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Favoritos)
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                      .WithMany(p => p.Favoritos)
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==========================================
            // MetodoPago
            // ==========================================
            modelBuilder.Entity<MetodoPago>(entity =>
            {
                entity.ToTable("metodoPago");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(e => e.Nombre)
                      .IsUnique();
            });

            // ==========================================
            // Venta
            // ==========================================
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("venta");

                entity.HasKey(e => e.NroVenta);

                entity.Property(e => e.Total)
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Estado)
                      .HasMaxLength(50)
                      .IsRequired();

                // Usuario 1 : N Venta
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Ventas)
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);

                // MetodoPago 1 : N Venta
                entity.HasOne(e => e.MetodoPago)
                      .WithMany()
                      .HasForeignKey(e => e.IdMetodoPago)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // DetalleVenta
            // ==========================================
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("detalleVenta");

                entity.HasKey(e => e.IdDetalle);

                entity.Property(e => e.Cantidad)
                      .IsRequired();

                entity.Property(e => e.PrecioUnitario)
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Subtotal)
                      .HasColumnType("decimal(10,2)");

                // Venta 1 : N DetalleVenta
                entity.HasOne(e => e.Venta)
                      .WithMany(v => v.Detalles)
                      .HasForeignKey(e => e.IdVenta)
                      .OnDelete(DeleteBehavior.Cascade);

                // Producto 1 : N DetalleVenta
                entity.HasOne(e => e.Producto)
                      .WithMany(p => p.DetallesVenta)
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}