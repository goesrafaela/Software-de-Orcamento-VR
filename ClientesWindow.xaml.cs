using System.Linq;
using System.Windows;
using OrcaPro.Data;
using OrcaPro.Models;

namespace OrcaPro
{
    public partial class ClientesWindow : Window
    {
        private int clienteSelecionadoId = 0;

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
            if (string.IsNullOrEmpty(NomeBox.Text))
            {
                MessageBox.Show("Digite o nome do cliente.");
                return;
            }

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

            LimparCampos();
            CarregarClientes();
        }

        // 👇 QUANDO SELECIONA NA TABELA
        private void ClientesGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var cliente = ClientesGrid.SelectedItem as Cliente;

            if (cliente != null)
            {
                clienteSelecionadoId = cliente.Id;
                NomeBox.Text = cliente.Nome;
                EmailBox.Text = cliente.Email;
                TelefoneBox.Text = cliente.Telefone;
            }
        }

        // 👇 EDITAR
        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (clienteSelecionadoId == 0)
            {
                MessageBox.Show("Selecione um cliente para editar.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var cliente = db.Clientes.Find(clienteSelecionadoId);

                if (cliente != null)
                {
                    cliente.Nome = NomeBox.Text;
                    cliente.Email = EmailBox.Text;
                    cliente.Telefone = TelefoneBox.Text;

                    db.SaveChanges();
                }
            }

            LimparCampos();
            CarregarClientes();
        }

        // 👇 EXCLUIR
        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var clienteSelecionado = ClientesGrid.SelectedItem as Cliente;

            if (clienteSelecionado == null)
            {
                MessageBox.Show("Selecione um cliente para excluir.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Deseja excluir o cliente {clienteSelecionado.Nome}?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            using (var db = new AppDbContext())
            {
                var cliente = db.Clientes.Find(clienteSelecionado.Id);

                if (cliente != null)
                {
                    db.Clientes.Remove(cliente);
                    db.SaveChanges();
                }
            }

            LimparCampos();
            CarregarClientes();
        }

        private void LimparCampos()
        {
            NomeBox.Text = "";
            EmailBox.Text = "";
            TelefoneBox.Text = "";
            clienteSelecionadoId = 0;
        }
    }
}