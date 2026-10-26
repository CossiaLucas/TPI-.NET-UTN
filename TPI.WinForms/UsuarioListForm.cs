using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TPI.ApiClients;
using TPI.Services.DTOs;
using TPI.Services.Interfaces;

namespace TPI.WinForms;

/// <summary>ABM de usuarios (solo administradores). Formulario construido por código, sin Designer.</summary>
public sealed class UsuarioListForm : Form
{
    private readonly IUsuarioService _usuarioService;
    private readonly ISessionService _sesion;
    private readonly DataGridView _grilla = new();
    private readonly ToolStrip _barra = new();

    public UsuarioListForm(IUsuarioService usuarioService, ISessionService sesion)
    {
        _usuarioService = usuarioService;
        _sesion = sesion;

        Text = "Usuarios";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(960, 480);

        ConfigurarBarra();
        ConfigurarGrilla();

        // Primero el control que ocupa el centro (Fill) y después la barra (Top), igual que hace el Designer.
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

        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = nameof(UsuarioDTO.Id), FillWeight = 35 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "nombre", HeaderText = "Nombre", DataPropertyName = nameof(UsuarioDTO.Nombre), FillWeight = 80 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "apellido", HeaderText = "Apellido", DataPropertyName = nameof(UsuarioDTO.Apellido), FillWeight = 80 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "username", HeaderText = "Usuario", DataPropertyName = nameof(UsuarioDTO.Username), FillWeight = 80 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "email", HeaderText = "Email", DataPropertyName = nameof(UsuarioDTO.Email), FillWeight = 130 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "dni", HeaderText = "DNI", DataPropertyName = nameof(UsuarioDTO.DNI), FillWeight = 70 });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "telefono", HeaderText = "Teléfono", DataPropertyName = nameof(UsuarioDTO.Telefono), FillWeight = 80 });
        _grilla.Columns.Add(new DataGridViewCheckBoxColumn { Name = "admin", HeaderText = "Admin", DataPropertyName = nameof(UsuarioDTO.IsAdmin), FillWeight = 40 });
    }

    private async Task ListarAsync()
    {
        try
        {
            var usuarios = await _usuarioService.GetAllAsync();
            _grilla.DataSource = usuarios.OrderBy(u => u.Id).ToList();
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"No se pudieron cargar los usuarios: {ex.Message}", "Usuarios",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AbrirDetalle(UsuarioDTO? usuario)
    {
        var form = Program.ServiceProvider.GetRequiredService<UsuarioDetalleForm>();
        form.Inicializar(usuario);
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = ListarAsync();
    }

    private void EditarSeleccionado()
    {
        if (_grilla.CurrentRow?.DataBoundItem is not UsuarioDTO seleccionado)
        {
            MessageBox.Show("Seleccioná un usuario de la grilla.");
            return;
        }
        AbrirDetalle(seleccionado);
    }

    private async Task EliminarAsync()
    {
        if (_grilla.CurrentRow?.DataBoundItem is not UsuarioDTO seleccionado)
        {
            MessageBox.Show("Seleccioná un usuario de la grilla.");
            return;
        }

        if (seleccionado.Id == _sesion.Usuario?.Id)
        {
            MessageBox.Show("No podés eliminar tu propio usuario.");
            return;
        }

        if (MessageBox.Show($"¿Eliminar al usuario '{seleccionado.Username}'?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            await _usuarioService.DeleteAsync(seleccionado.Id);
            await ListarAsync();
        }
        catch (ApiErrorException ex)
        {
            MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            MessageBox.Show("No se puede eliminar: el usuario tiene ventas asociadas.", "No se pudo eliminar",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"No se pudo eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
