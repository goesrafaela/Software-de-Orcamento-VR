using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OrcaPro.Data;
using OrcaPro.Models;

namespace OrcaPro
{
    public partial class HistoricoWindow : Window
    {
        public HistoricoWindow()
        {
            InitializeComponent();

            Loaded += HistoricoWindow_Loaded;
        }

        private void HistoricoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarFiltros();
                CarregarDados();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao abrir histórico:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CarregarFiltros()
        {
            using (var db = new AppDbContext())
            {
                ClienteFiltro.Items.Clear();

                var clientes = db.Clientes.ToList();

                ClienteFiltro.Items.Add("Todos");

                foreach (var c in clientes)
                {
                    ClienteFiltro.Items.Add(c.Nome);
                }

                ClienteFiltro.SelectedIndex = 0;

                if (StatusFiltro.Items.Count > 0)
                    StatusFiltro.SelectedIndex = 0;
            }
        }

        private void CarregarDados()
        {
            using (var db = new AppDbContext())
            {
                var lista = db.Orcamentos
                    .OrderByDescending(o => o.Id)
                    .ToList();

                HistoricoGrid.ItemsSource = lista;
            }
        }

        // 🔍 FILTRAR
        private void Filtrar(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var query = db.Orcamentos.AsQueryable();

                    // CLIENTE
                    if (ClienteFiltro.SelectedItem != null &&
                        ClienteFiltro.SelectedItem.ToString() != "Todos")
                    {
                        string nomeCliente = ClienteFiltro.SelectedItem.ToString();

                        var cliente = db.Clientes
                            .FirstOrDefault(c => c.Nome == nomeCliente);

                        if (cliente != null)
                        {
                            query = query.Where(o => o.ClienteId == cliente.Id);
                        }
                    }

                    // STATUS
                    if (StatusFiltro.SelectedItem is ComboBoxItem statusItem)
                    {
                        string status = statusItem.Content.ToString();

                        if (status != "Todos")
                        {
                            query = query.Where(o => o.Status == status);
                        }
                    }

                    // BUSCA
                    if (!string.IsNullOrWhiteSpace(BuscaBox.Text))
                    {
                        string termo = BuscaBox.Text.ToLower();

                        query = query.Where(o =>
                            o.Id.ToString().Contains(termo) ||
                            (o.Status != null &&
                             o.Status.ToLower().Contains(termo)));
                    }

                    HistoricoGrid.ItemsSource = query
                        .OrderByDescending(o => o.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao filtrar:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 🎨 CORES AUTOMÁTICAS
        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                var item = e.Row.Item as Orcamento;

                if (item == null)
                    return;

                switch (item.Status)
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

                    default:
                        e.Row.Background = Brushes.White;
                        break;
                }
            }
            catch
            {
                // evita crash visual
            }
        }
    }
}