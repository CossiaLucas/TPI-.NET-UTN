using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TPI.Api;
using TPI.Data.Context;
using TPI.Data.Repositories;
using TPI.Services.Interfaces;
using TPI.Services.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresar el token con el formato: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

var app = builder.Build();

//AUTOGENERA BD
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //context.Database.Migrate();
    context.Database.EnsureCreated();
    var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    var categoriaRepository = scope.ServiceProvider.GetRequiredService<ICategoriaRepository>();
    var productoRepository = scope.ServiceProvider.GetRequiredService<IProductoRepository>();

    if (!await context.Usuarios.AnyAsync())
    {
        var admin = new Usuario(
            nombre: "Admin",
            apellido: "Capo",
            username: "admin",
            email: "admin@gmail.com",
            password: "1234567",
            dni: "12345678",
            fechaNacimiento: DateTime.Now,
            telefono: "000000000",
            isAdmin: true);

        await usuarioRepository.AddAsync(admin);
    }

    if (!await context.Categorias.AnyAsync())
    {
        var categoria = new Categoria { Nombre = "General" };
        await categoriaRepository.AddAsync(categoria);

        var producto = new Producto
        {
            Nombre = "Producto Demo",
            Descripcion = "Producto de ejemplo",
            Stock = 10,
            IdCategoria = categoria.Id,
            FotoUrl = null,
            Precios = { new Precio { FechaDesde = DateTime.Now, Valor = 1500m } }
        };
        await productoRepository.AddAsync(producto);
    }
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapUsuarioEndpoints();

app.MapAuthEndpoints();

app.MapCategoriaEndpoints();

app.MapProductoEndpoints();

app.MapControllers();

app.Run();
