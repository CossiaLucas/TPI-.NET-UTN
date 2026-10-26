using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms;

/// <summary>Alta y edición de un método de pago.</summary>
public sealed class MetodoPagoDetalleForm : Form
{
    private readonly IMetodoPagoService _metodoPagoService;
    private MetodoPagoDTO? _metodoActual;

    private readonly TextBox _nombre = new() { MaxLength = 100 };
    private readonly Button _guardar = new() { Text = "Guardar", AutoSize = true };
    private readonly Button _cancelar = new() { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };

    public MetodoPagoDetalleForm(IMetodoPagoService metodoPagoService)
    {
        _metodoPagoService = metodoPagoService;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(400, 120);
        AcceptButton = _guardar;
        CancelButton = _cancelar;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), ColumnCount = 2, RowCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Nombre", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _nombre.Dock = DockStyle.Fill;
        layout.Controls.Add(_nombre, 1, 0);

        var botones = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        botones.Controls.Add(_guardar);
        botones.Controls.Add(_cancelar);
        layout.Controls.Add(botones, 1, 1);

        Controls.Add(layout);

        _guardar.Click += async (_, _) => await GuardarAsync();
    }

    /// <summary>null = alta; un método de pago = edición.</summary>
    public void Inicializar(MetodoPagoDTO? metodo)
    {
        _metodoActual = metodo;
        _nombre.Text = metodo?.Nombre ?? string.Empty;
        Text = metodo is null ? "Nuevo método de pago" : "Editar método de pago";
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_nombre.Text))
        {
            MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _guardar.Enabled = false;
        try
        {
            if (_metodoActual is null)
            {
                await _metodoPagoService.CreateAsync(new MetodoPagoDTO { Nombre = _nombre.Text.Trim() });
            }
            else
            {
                var existe = await _metodoPagoService.UpdateAsync(new MetodoPagoDTO
                {
                    IdMetodoPago = _metodoActual.IdMetodoPago,
                    Nombre = _nombre.Text.Trim()
                });

                if (!existe)
                {
                    MessageBox.Show("El método de pago ya no existe.", "Métodos de pago",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
