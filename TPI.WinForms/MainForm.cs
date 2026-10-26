using Dominio.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPI.ApiClients;
using TPI.Services.Interfaces;

namespace TPI.WinForms
{
    public partial class MainForm : Form
    {
        private readonly ISessionService _sesionService;
        public MainForm(ISessionService _sesion)
        {
            InitializeComponent();
            _sesionService = _sesion;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = $"Hola, {_sesionService.Usuario!.Nombre}";
            foreach (var item in menuStrip.Items.Find("menuReportes", false))
                item.Visible = _sesionService.Usuario!.IsAdmin;

            // Menús de administración: solo se agregan si el usuario logueado es admin.
            // (La API también lo exige con la policy SoloAdmin: ocultar el menú es solo comodidad.)
            if (_sesionService.Usuario.IsAdmin)
            {
                var menuUsuarios = new ToolStripMenuItem("Usuarios");
                menuUsuarios.Click += (_, _) =>
                    Program.ServiceProvider.GetRequiredService<UsuarioListForm>().ShowDialog();

                var menuMetodosPago = new ToolStripMenuItem("Métodos de pago");
                menuMetodosPago.Click += (_, _) =>
                    Program.ServiceProvider.GetRequiredService<MetodoPagoListForm>().ShowDialog();

                var posicion = menuStrip.Items.IndexOf(menuLogout);
                menuStrip.Items.Insert(posicion, menuUsuarios);
                menuStrip.Items.Insert(posicion + 1, menuMetodosPago);
            }

            // Los CRUD de Producto/Categoria solo los veria un admin.
            // Si mas adelante seagrega algo para el usuario comun, va aca tambian,
            // condicionado a !_sesionService.Usuario.IsAdmin.

            // menuProductos.Visible = _sesionService.Usuario.IsAdmin; esta linea seria si en un futuro los menus de producto y categorias los ve solo el admin
            // menuCategorias.Visible = _sesionService.Usuario.IsAdmin;    por ahora para la entrega 2 asumo que no
        }

        private void menuProductos_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<ProductoListForm>();
            form.ShowDialog(); // modal: vuelve aca al cerrarlo
        }

        private void menuCategorias_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<CategoriaListForm>();
            form.ShowDialog();
            //MessageBox.Show("Categorías menu ejemplo");
        }

        private void menuReportes_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<ReportesGraficosForm>();
            form.ShowDialog();
        }

        private void menuVentas_Click(object sender, EventArgs e)
        {
            var form = Program.ServiceProvider.GetRequiredService<Ventas>();
            form.ShowDialog();
        }

        public bool SalirDeLaApp { get; private set; } = true; // default: si cierran con la X, se sale
        private void menuLogout_Click(object sender, EventArgs e)
        {
            SalirDeLaApp = false;
            _sesionService.CerrarSesion();
            this.Close();
        }
    }
}
