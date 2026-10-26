using System.Text.RegularExpressions;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms;

/// <summary>
/// Alta y edición de un usuario. Alta: crea un usuario con rol Cliente (la API nunca asigna admin desde acá).
/// Edición: solo se pueden cambiar nombre, apellido, teléfono y (opcionalmente) la contraseña.
/// </summary>
public sealed class UsuarioDetalleForm : Form
{
    private readonly IUsuarioService _usuarioService;
    private UsuarioDTO? _usuarioActual;

    private readonly TextBox _nombre = new();
    private readonly TextBox _apellido = new();
    private readonly TextBox _username = new();
    private readonly TextBox _email = new();
    private readonly TextBox _clave = new() { PasswordChar = '*' };
    private readonly TextBox _dni = new();
    private readonly DateTimePicker _fechaNacimiento = new() { Format = DateTimePickerFormat.Short };
    private readonly TextBox _telefono = new();
    private readonly Label _lblClave = new() { AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly Button _guardar = new() { Text = "Guardar", AutoSize = true };
    private readonly Button _cancelar = new() { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };

    public UsuarioDetalleForm(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(460, 380);
        AcceptButton = _guardar;
        CancelButton = _cancelar;

        var campos = new (Control Etiqueta, Control Campo)[]
        {
            (Etiqueta("Nombre"), _nombre),
            (Etiqueta("Apellido"), _apellido),
            (Etiqueta("Usuario"), _username),
            (Etiqueta("Email"), _email),
            (_lblClave, _clave),
            (Etiqueta("DNI"), _dni),
            (Etiqueta("Fecha de nacimiento"), _fechaNacimiento),
            (Etiqueta("Teléfono"), _telefono)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = campos.Length + 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (var i = 0; i < campos.Length; i++)
        {
            layout.Controls.Add(campos[i].Etiqueta, 0, i);
            campos[i].Campo.Dock = DockStyle.Fill;
            layout.Controls.Add(campos[i].Campo, 1, i);
        }

        var botones = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        botones.Controls.Add(_guardar);
        botones.Controls.Add(_cancelar);
        layout.Controls.Add(botones, 1, campos.Length);

        Controls.Add(layout);

        _guardar.Click += async (_, _) => await GuardarAsync();
    }

    private static Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Anchor = AnchorStyles.Left };

    /// <summary>null = alta; un usuario = edición.</summary>
    public void Inicializar(UsuarioDTO? usuario)
    {
        _usuarioActual = usuario;
        var esEdicion = usuario is not null;

        Text = esEdicion ? "Editar usuario" : "Nuevo usuario";
        _lblClave.Text = esEdicion ? "Nueva contraseña (opcional)" : "Contraseña";

        _nombre.Text = usuario?.Nombre ?? string.Empty;
        _apellido.Text = usuario?.Apellido ?? string.Empty;
        _username.Text = usuario?.Username ?? string.Empty;
        _email.Text = usuario?.Email ?? string.Empty;
        _dni.Text = usuario?.DNI ?? string.Empty;
        _telefono.Text = usuario?.Telefono ?? string.Empty;
        _clave.Text = string.Empty;

        var fecha = usuario?.FechaNacimiento.Date ?? DateTime.Today.AddYears(-18);
        _fechaNacimiento.Value = fecha < _fechaNacimiento.MinDate || fecha > _fechaNacimiento.MaxDate
            ? DateTime.Today
            : fecha;

        // En edición, estos datos de identidad no se modifican.
        _username.Enabled = !esEdicion;
        _email.Enabled = !esEdicion;
        _dni.Enabled = !esEdicion;
        _fechaNacimiento.Enabled = !esEdicion;
    }

    private string? Validar()
    {
        if (string.IsNullOrWhiteSpace(_nombre.Text)) return "El nombre es obligatorio.";
        if (string.IsNullOrWhiteSpace(_apellido.Text)) return "El apellido es obligatorio.";
        if (string.IsNullOrWhiteSpace(_telefono.Text)) return "El teléfono es obligatorio.";

        if (_usuarioActual is null)
        {
            var username = _username.Text.Trim();
            if (username.Length is < 4 or > 50) return "El usuario debe tener entre 4 y 50 caracteres.";
            if (!Regex.IsMatch(_email.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return "El email no tiene un formato válido.";
            if (string.IsNullOrWhiteSpace(_dni.Text)) return "El DNI es obligatorio.";
            if (_clave.Text.Length < 6) return "La contraseña debe tener al menos 6 caracteres.";
            if (_fechaNacimiento.Value.Date > DateTime.Today) return "La fecha de nacimiento no puede ser futura.";
        }
        else if (_clave.Text.Length > 0 && _clave.Text.Length < 6)
        {
            return "La contraseña debe tener al menos 6 caracteres.";
        }

        return null;
    }

    private async Task GuardarAsync()
    {
        var error = Validar();
        if (error is not null)
        {
            MessageBox.Show(error, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _guardar.Enabled = false;
        try
        {
            if (_usuarioActual is null)
            {
                await _usuarioService.CreateAsync(new UsuarioCreateDTO
                {
                    Nombre = _nombre.Text.Trim(),
                    Apellido = _apellido.Text.Trim(),
                    Username = _username.Text.Trim(),
                    Email = _email.Text.Trim(),
                    Clave = _clave.Text,
                    DNI = _dni.Text.Trim(),
                    FechaNacimiento = _fechaNacimiento.Value.Date,
                    Telefono = _telefono.Text.Trim()
                });
            }
            else
            {
                var existe = await _usuarioService.UpdateAsync(new UsuarioUpdateDTO
                {
                    Id = _usuarioActual.Id,
                    Nombre = _nombre.Text.Trim(),
                    Apellido = _apellido.Text.Trim(),
                    Telefono = _telefono.Text.Trim(),
                    Clave = string.IsNullOrWhiteSpace(_clave.Text) ? null : _clave.Text
                });

                if (!existe)
                {
                    MessageBox.Show("El usuario ya no existe.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (ApiErrorException ex)
        {
            MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("No se pudo conectar con el servidor.", "Error de conexión",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _guardar.Enabled = true;
        }
    }
}
