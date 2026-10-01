using Dominio.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TPI.Services.Interfaces;
using TPI.ApiClients;
namespace TPI.WinForms
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var apiBaseUrl = configuration["ApiBaseUrl"]!;
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);


            services.AddHttpClient<IAuthService, AuthApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddHttpClient<IUsuarioService, UsuarioApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddHttpClient<ICategoriaService, CategoriaApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddHttpClient<IProductoService, ProductoApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl));

            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();
            services.AddTransient<CategoriaListForm>();
            services.AddTransient<CategoriaDetalleForm>();
            services.AddTransient<ProductoListForm>();
            services.AddTransient<ProductoDetalleForm>();
            ServiceProvider = services.BuildServiceProvider();

            while (true)
            {
                if (!SesionActual.EstaLogueado)
                {
                    var loginForm = ServiceProvider.GetRequiredService<LoginForm>();
                    if (loginForm.ShowDialog() != DialogResult.OK)
                        return;
                }

                var mainForm = ServiceProvider.GetRequiredService<MainForm>();

                try
                {
                    Application.Run(mainForm);
                }
                catch (SesionExpiradaException)
                {
                    SesionActual.CerrarSesion();
                    MessageBox.Show("Tu sesion expiro. Volve a iniciar sesion.", "Sesion expirada",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                if (mainForm.SalirDeLaApp)
                    break;
            }
        }
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception is SesionExpiradaException)
            {
                SesionActual.CerrarSesion();
                MessageBox.Show("Tu sesion expiro. La aplicacion se va a reiniciar.", "Sesion expirada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Restart();
            }
            else
            {
                MessageBox.Show($"Error inesperado: {e.Exception.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}