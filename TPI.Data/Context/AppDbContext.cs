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

        internal AppDbContext()
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Categoria> Categorias => Set<Categoria>(); // ARRANCAMOS CON ESTAS PARA LA ENTREGA 2, LUEGO AGREGAMOS TODO EL RESTO

        public DbSet<Producto> Productos => Set<Producto>();

        public DbSet<Direccion> Direcciones => Set<Direccion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                entity.HasKey(e => e.Id);

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
                      .WithMany(c => c.Productos)
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
                      .WithMany(p => p.HistorialPrecios)
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Cascade);
            });

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

                entity.Property(e => e.ClaveHash)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Salt)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.Property(e => e.DNI)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasIndex(e => e.DNI)
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

                entity.HasOne(e => e.Direccion)
                      .WithOne(d => d.Usuario)
                      .HasForeignKey<Usuario>(u => u.IdDireccion)
                      .OnDelete(DeleteBehavior.Restrict);
            });

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
        }
    }
}