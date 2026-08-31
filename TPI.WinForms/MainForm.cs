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

namespace TPI.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = $"Hola, {SesionActual.Usuario!.Nombre}";

            // Los CRUD de Producto/Categoria solo los veria un admin.
            // Si mas adelante seagrega algo para el usuario comun, va aca tambian,
            // condicionado a !SesionActual.Usuario.IsAdmin.


           // menuProductos.Visible = SesionActual.Usuario.IsAdmin; esta linea seria si en un futuro los menus de producto y categorias los ve solo el admin
           // menuCategorias.Visible = SesionActual.Usuario.IsAdmin;    por ahora para la entrega 2 asumo que no
        }

        private void menuProductos_Click(object sender, EventArgs e)
        {
            // ProductoListForm NO EXISTE TODAVIA, FALTA EL SERVICIO
            var form = Program.ServiceProvider.GetRequiredService<ProductoListForm>();
            form.ShowDialog(); // modal: vuelve aca al cerrarlo
        }

        private void menuCategorias_Click(object sender, EventArgs e)
        {
             // CateoriaListForm NO EXISTE TODAVIA, FALTA EL SERVICIO
            var form = Program.ServiceProvider.GetRequiredService<CategoriaListForm>();
            form.ShowDialog();
            //MessageBox.Show("Categorías menu ejemplo");
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            SesionActual.CerrarSesion();

            var loginForm = Program.ServiceProvider.GetRequiredService<LoginForm>();
            loginForm.Show();
            this.Close();
        }
    }
}
