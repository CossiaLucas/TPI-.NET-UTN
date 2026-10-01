using Microsoft.Extensions.DependencyInjection;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;
using TPI.ApiClients;

namespace TPI.WinForms
{
    public partial class LoginForm : Form
    {
        private readonly IAuthService _authService;

        public LoginForm(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new LoginRequestDTO { Email = txtEmail.Text, Password = txtPassword.Text };
                var resultado = await _authService.LoginAsync(dto);

                if (resultado is null)
                {
                    MessageBox.Show("Email o contraseña incorrectos.", "Error de login",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SesionActual.Iniciar(resultado);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("No se pudo conectar con el servidor. Verifica que estas corriendo TPI.Api",
                    "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
