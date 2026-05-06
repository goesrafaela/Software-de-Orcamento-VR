using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OrcaPro.Data;

namespace OrcaPro
{
    public partial class HistoricoWindow : Window
    {
        public HistoricoWindow()
        {
            InitializeComponent();
            CarregarFiltros();
            CarregarDados();
        }

        private void CarregarFiltros()
        {
            using (var db = new AppDbContext())
            {
                var clientes = db.Clientes.ToList();

                ClienteFiltro.Items.Add("Todos");

                foreach (var c in clientes)
                    ClienteFiltro.Items.Add(c.Nome);

                ClienteFiltro.SelectedIndex = 0;
                StatusFiltro.SelectedIndex = 0;
            }
        }

        private void CarregarDados()
        {
            using (var db = new AppDbContext())
            {
                var lista = db.Orcamentos.ToList();
                HistoricoGrid.ItemsSource = lista;
            }
        }

        // 🔍 FILTRO PRINCIPAL
        private void Filtrar(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Orcamentos.AsQueryable();

                // Cliente
                if (ClienteFiltro.SelectedItem != null &&
                    ClienteFiltro.SelectedItem.ToString() != "Todos")
                {
                    var nome = ClienteFiltro.SelectedItem.ToString();
                    var cliente = db.Clientes.FirstOrDefault(c => c.Nome == nome);

                    if (cliente != null)
                        query = query.Where(o => o.ClienteId == cliente.Id);
                }

                // Status
                if (StatusFiltro.SelectedItem is ComboBoxItem statusItem &&
                    statusItem.Content.ToString() != "Todos")
                {
                    var status = statusItem.Content.ToString();
                    query = query.Where(o => o.Status == status);
                }

                // Busca
                if (!string.IsNullOrEmpty(BuscaBox.Text))
                {
                    var termo = BuscaBox.Text.ToLower();

                    query = query.Where(o =>
                        o.Id.ToString().Contains(termo) ||
                        o.Status.ToLower().Contains(termo));
                }

                HistoricoGrid.ItemsSource = query.ToList();
            }
        }

        // 🎨 CORES
        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dynamic item = e.Row.Item;

            if (item == null) return;

            string status = item.Status;

            switch (status)
            {
                case "Aprovado":
                    e.Row.Background = Brushes.LightGreen;
                    break;

                case "Finalizado":
                    e.Row.Background = Brushes.LightBlue;
                    break;

                case "Em andamento":
                    e.Row.Background = Brushes.LightYellow;
                    break;
            }
        }
    }
}