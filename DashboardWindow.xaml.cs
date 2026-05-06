using System;
using System.Linq;
using System.Windows;
using OrcaPro.Data;

namespace OrcaPro
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            this.Loaded += DashboardWindow_Loaded;
        }

        private void DashboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarDashboard();
        }

        public void CarregarDashboard()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // 🔥 garante que banco existe
                    db.Database.EnsureCreated();

                    // CLIENTES
                    var totalClientes = db.Clientes?.Count() ?? 0;
                    ClientesCount.Text = totalClientes.ToString();

                    // ORÇAMENTOS
                    var orcamentos = db.Orcamentos?.ToList() ?? new System.Collections.Generic.List<Models.Orcamento>();

                    // STATUS
                    EmAndamentoCount.Text = orcamentos
                        .Count(o => o.Status == "Em andamento")
                        .ToString();

                    AprovadosCount.Text = orcamentos
                        .Count(o => o.Status == "Aprovado")
                        .ToString();

                    // FATURAMENTO
                    var faturamento = orcamentos
                        .Where(o => o.Status == "Aprovado")
                        .Sum(o => o.Total);

                    FaturamentoText.Text = $"R$ {faturamento:N2}";

                    // GRID
                    OrcamentosGrid.ItemsSource = orcamentos
                        .OrderByDescending(o => o.Id)
                        .Take(10)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar dashboard:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void AbrirOrcamentos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tela = new OrcamentosWindow();
                tela.ShowDialog();

                CarregarDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir orçamentos:\n" + ex.Message);
            }
        }

        private void AbrirClientes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tela = new ClientesWindow();
                tela.ShowDialog();

                CarregarDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir clientes:\n" + ex.Message);
            }
        }

        private void AbrirHistorico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tela = new HistoricoWindow();
                tela.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro:\n" + ex.Message);
            }
        }

        private void AbrirRelatorio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tela = new RelatorioWindow();
                tela.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro:\n" + ex.Message);
            }
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}