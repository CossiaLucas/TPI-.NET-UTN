using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms;

/// <summary>ABM de métodos de pago (solo administradores). Formulario construido por código, sin Designer.</summary>
public sealed class MetodoPagoListForm : Form
{
    private readonly IMetodoPagoService _metodoPagoService;
    private readonly DataGridView _grilla = new();
    private readonly ToolStrip _barra = new();

    public MetodoPagoListForm(IMetodoPagoService metodoPagoService)
    {
        _metodoPagoService = metodoPagoService;

        Text = "Métodos de pago";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 420);

        ConfigurarBarra();
        ConfigurarGrilla();

        Controls.Add(_grilla);
        Controls.Add(_barra);

        Load += async (_, _) => await ListarAsync();
    }

    private void ConfigurarBarra()
    {
        var nuevo = new ToolStripButton("Nuevo");
        var editar = new ToolStripButton("Editar");
        var eliminar = new ToolStripButton("Eliminar");
        var actualizar = new ToolStripButton("Actualizar");

        nuevo.Click += (_, _) => AbrirDetalle(null);
        editar.Click += (_, _) => EditarSeleccionado();
        eliminar.Click += async (_, _) => await EliminarAsync();
        actualizar.Click += async (_, _) => await ListarAsync();

        _barra.Items.AddRange(new ToolStripItem[] { nuevo, editar, eliminar, new ToolStripSeparator(), actualizar });
    }

    private void ConfigurarGrilla()
    {
        _grilla.Dock = DockStyle.Fill;
        _grilla.AutoGenerateColumns = false;
        _grilla.ReadOnly = true;
        _grilla.AllowUserToAddRows = false;
        _grilla.AllowUserToDeleteRows = false;
        _grilla.MultiSelect = false;
        _grilla.RowHeadersVisible = false;
        _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = nameof(MetodoPagoDTO.IdMetodoPago), FillWeight = 25 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "nombre", HeaderText = "Nombre", DataPropertyName = nameof(MetodoPagoDTO.Nombre), FillWeight = 100 });
    }

    private async Task ListarAsync()
    {
        try
        {
            _grilla.DataSource = await _metodoPagoService.GetAllAsync();
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"No se pudieron cargar los métodos de pago: {ex.Message}", "Métodos de pago",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AbrirDetalle(MetodoPagoDTO? metodo)
    {
        var form = Program.ServiceProvider.GetRequiredService<MetodoPagoDetalleForm>();
        form.Inicializar(metodo);
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = ListarAsync();
    }

    private void EditarSeleccionado()
    {
        if (_grilla.CurrentRow?.DataBoundItem is not MetodoPagoDTO seleccionado)
        {
            MessageBox.Show("Seleccioná un método de pago de la grilla.");
            return;
        }
        AbrirDetalle(seleccionado);
    }

    private async Task EliminarAsync()
    {
        if (_grilla.CurrentRow?.DataBoundItem is not MetodoPagoDTO seleccionado)
        {
            MessageBox.Show("Seleccioná un método de pago de la grilla.");
            return;
        }

        if (MessageBox.Show($"¿Eliminar el método de pago '{seleccionado.Nombre}'?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            await _metodoPagoService.DeleteAsync(seleccionado.IdMetodoPago);
            await ListarAsync();
        }
        catch (ApiErrorException ex)
        {
            MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            MessageBox.Show("No se puede eliminar: el método de pago tiene ventas asociadas.", "No se pudo eliminar",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"No se pudo eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
