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

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Categoria> Categorias => Set<Categoria>(); // ARRANCAMOS CON ESTAS PARA LA ENTREGA 2, LUEGO AGREGAMOS TODO EL RESTO

        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>()
                .Property(categoria => categoria.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Producto>()
                .Property(producto => producto.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Producto>()
                .HasOne(producto => producto.Categoria)
                .WithMany()
                .HasForeignKey(producto => producto.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}