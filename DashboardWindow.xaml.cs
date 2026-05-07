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
                    db.Database.EnsureCreated();

                    // CLIENTES
                    var totalClientes = db.Clientes?.Count() ?? 0;
                    ClientesCount.Text = totalClientes.ToString();

                    // ORÇAMENTOS
                    var orcamentos = db.Orcamentos?.ToList()
                        ?? new System.Collections.Generic.List<Models.Orcamento>();

                    // STATUS
                    EmAndamentoCount.Text = orcamentos
                        .Count(o => o.Status == "Em andamento")
                        .ToString();

                    AprovadosCount.Text = orcamentos
                        .Count(o => o.Status == "Aprovado")
                        .ToString();

                    // FATURAMENTO
                   var faturamento = orcamentos
                    .Where(o =>
                        (o.Status == "Aprovado" ||
                        o.Status == "Finalizado")
                        &&
                        o.DataCriacao.Month == DateTime.Now.Month
                        &&
                        o.DataCriacao.Year == DateTime.Now.Year)
                    .Sum(o => o.Total);

                    FaturamentoText.Text = $"R$ {faturamento:N2}";

                    // GRID
                    OrcamentosGrid.ItemsSource = orcamentos
                    .OrderByDescending(o => o.Id)
                    .Take(10)
                    .Select(o => new
                    {
                        o.Id,
                        Cliente = db.Clientes
                            .FirstOrDefault(c => c.Id == o.ClienteId)?.Nome,
                        o.Total,
                        o.Status
                    })
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

        // ✅ APROVAR
        private void Aprovar_Click(object sender, RoutedEventArgs e)
        {
            AlterarStatus("Aprovado");
        }

        // ✅ EM ANDAMENTO
        private void Andamento_Click(object sender, RoutedEventArgs e)
        {
            AlterarStatus("Em andamento");
        }

        // ✅ FINALIZAR
        private void Finalizar_Click(object sender, RoutedEventArgs e)
        {
            AlterarStatus("Finalizado");
        }

        // ✅ ALTERAR STATUS
        private void AlterarStatus(string novoStatus)
        {
            try
            {
                if (OrcamentosGrid.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um orçamento.");
                    return;
                }

                dynamic linha = OrcamentosGrid.SelectedItem;

                int id = linha.Id;

                using (var db = new AppDbContext())
                {
                    var orcamento = db.Orcamentos.Find(id);

                    if (orcamento != null)
                    {
                        orcamento.Status = novoStatus;

                        db.SaveChanges();
                    }
                }

                CarregarDashboard();

                MessageBox.Show($"Status alterado para: {novoStatus}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro:\n" + ex.Message);
            }
        }
    }
}