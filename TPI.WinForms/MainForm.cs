using Microsoft.Extensions.DependencyInjection;
using TPI.Services.Interfaces;
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

        public bool SalirDeLaApp { get; private set; } = true; // default: si cierran con la X, se sale
        private void menuLogout_Click(object sender, EventArgs e)
        {
            SalirDeLaApp = false;
            _sesionService.CerrarSesion();
            this.Close();
        }
    }
}
