using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TPI.Data.Context;
using TPI.Data.Repositories;
using TPI.Services.Interfaces;
using TPI.Services.Services;
namespace TPI.WinForms
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ICategoriaService, CategoriaService>();

            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IProductoService, ProductoService>();

            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();       // pantalla comun / admin

            services.AddTransient<CategoriaListForm>();
            //services.AddTransient<CategoriaForm>();
            //services.AddTransient<ProductoListForm>();
            //services.AddTransient<ProductoForm>();
            ServiceProvider = services.BuildServiceProvider();

            // Asegurar migraciones y seed local cuando se ejecuta el WinForms
            using (var scope = ServiceProvider.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                    var usuarioRepo = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
                    var categoriaRepo = scope.ServiceProvider.GetService<ICategoriaRepository>();
                    var productoRepo = scope.ServiceProvider.GetService<IProductoRepository>();
                    var usuarios = context.Usuarios.AsNoTracking().ToList();
                    if (usuarios.Count == 0)
                    {
                        var admin = new Dominio.Entities.Usuario(
                            nombre: "Admin",
                            apellido: "Capo",
                            username: "admin",
                            email: "admin@gmail.com",
                            password: "1234",
                            dni: "12345678",
                            fechaNacimiento: DateTime.Now,
                            telefono: "000000000",
                            isAdmin: true);
                        usuarioRepo.AddAsync(admin).GetAwaiter().GetResult();
                    }
                    if (categoriaRepo is not null)
                    {
                        var cats = context.Categorias.AsNoTracking().ToList();
                        if (cats.Count == 0)
                        {
                            var cat = new Dominio.Entities.Categoria { Nombre = "General" };
                            categoriaRepo.AddAsync(cat).GetAwaiter().GetResult();
                            if (productoRepo is not null)
                            {
                                var prod = new Dominio.Entities.Producto
                                {
                                    Nombre = "Producto Demo",
                                    Descripcion = "Producto de ejemplo",
                                    Stock = 10,
                                    IdCategoria = cat.Id,
                                    FotoUrl = string.Empty
                                };
                                productoRepo.AddAsync(prod).GetAwaiter().GetResult();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Seed error (WinForms): {ex.Message}");
                }
            }
            Application.Run(ServiceProvider.GetRequiredService<LoginForm>());
        }
    }
}