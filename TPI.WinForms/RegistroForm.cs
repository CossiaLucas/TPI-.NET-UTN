using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms;

public sealed class RegistroForm : Form
{
    private readonly IUsuarioService _usuarioService;
    private readonly TextBox _nombre = Campo();
    private readonly TextBox _apellido = Campo();
    private readonly TextBox _username = Campo();
    private readonly TextBox _email = Campo();
    private readonly TextBox _clave = Campo();
    private readonly TextBox _dni = Campo();
    private readonly DateTimePicker _fechaNacimiento = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(-18) };
    private readonly TextBox _telefono = Campo();
    private readonly Button _registrar = new() { Text = "Registrarme", AutoSize = true };

    public RegistroForm(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
        Text = "Crear cuenta";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(430, 430);
        _clave.PasswordChar = '*';

        var campos = new (string, Control)[]
        {
            ("Nombre", _nombre), ("Apellido", _apellido), ("Usuario", _username),
            ("Email", _email), ("Contraseña", _clave), ("DNI", _dni),
            ("Fecha de nacimiento", _fechaNacimiento), ("Teléfono", _telefono)
        };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), ColumnCount = 2, RowCount = campos.Length + 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < campos.Length; i++)
        {
            layout.Controls.Add(new Label { Text = campos[i].Item1, AutoSize = true, Anchor = AnchorStyles.Left }, 0, i);
            campos[i].Item2.Dock = DockStyle.Fill;
            layout.Controls.Add(campos[i].Item2, 1, i);
        }
        _registrar.Click += async (_, _) => await RegistrarAsync();
        layout.Controls.Add(_registrar, 1, campos.Length);
        Controls.Add(layout);
    }

    private async Task RegistrarAsync()
    {
        if (new[] { _nombre, _apellido, _username, _email, _clave, _dni, _telefono }.Any(t => string.IsNullOrWhiteSpace(t.Text)))
        {
            MessageBox.Show("Completá todos los campos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            await _usuarioService.CreateAsync(new UsuarioCreateDTO
            {
                Nombre = _nombre.Text.Trim(), Apellido = _apellido.Text.Trim(), Username = _username.Text.Trim(),
                Email = _email.Text.Trim(), Clave = _clave.Text, DNI = _dni.Text.Trim(),
                FechaNacimiento = _fechaNacimiento.Value.Date, Telefono = _telefono.Text.Trim()
            });
            MessageBox.Show("Usuario registrado correctamente. Ya podés iniciar sesión.", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (ApiErrorException ex)
        {
            MessageBox.Show(ex.Message, "No se pudo registrar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("No se pudo conectar con el servidor.", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static TextBox Campo() => new();
}
