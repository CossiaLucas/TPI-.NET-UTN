using Microsoft.Extensions.DependencyInjection;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

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
            var dto = new LoginRequestDTO
            {
                Email = txtEmail.Text,
                Password = txtPassword.Text
            };

            var resultado = await _authService.LoginAsync(dto);

            if (resultado is null)
            {
                MessageBox.Show("Email o contraseña incorrectos.", "Error de login",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SesionActual.Iniciar(resultado); // guardamos el usuario logueado, ver clase abajo

            var mainForm = Program.ServiceProvider.GetRequiredService<MainForm>();
            mainForm.Show();
            this.Hide();
        }
    }
}
