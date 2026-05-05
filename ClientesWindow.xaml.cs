using System.Linq;
using System.Windows;
using OrcaPro.Data;
using OrcaPro.Models;

namespace OrcaPro
{
    public partial class ClientesWindow : Window
    {
        public ClientesWindow()
        {
            InitializeComponent();
            CarregarClientes();
        }

        private void CarregarClientes()
        {
            using (var db = new AppDbContext())
            {
                ClientesGrid.ItemsSource = db.Clientes.ToList();
            }
        }

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var cliente = new Cliente
                {
                    Nome = NomeBox.Text,
                    Email = EmailBox.Text,
                    Telefone = TelefoneBox.Text
                };

                db.Clientes.Add(cliente);
                db.SaveChanges();
            }

            NomeBox.Text = "";
            EmailBox.Text = "";
            TelefoneBox.Text = "";

            CarregarClientes();
        }
    }
}