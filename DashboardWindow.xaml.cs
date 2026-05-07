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

                    // 🔥 CLIENTES
                    int totalClientes = db.Clientes?.Count() ?? 0;

                    ClientesCount.Text = totalClientes.ToString();

                    // 🔥 LISTA DE ORÇAMENTOS
                    var orcamentos = db.Orcamentos?
                        .ToList()
                        ?? new System.Collections.Generic.List<Models.Orcamento>();

                    // 🔥 STATUS
                    EmAndamentoCount.Text = orcamentos
                        .Count(o => o.Status == "Em andamento")
                        .ToString();

                    AprovadosCount.Text = orcamentos
                        .Count(o =>
                            o.Status == "Aprovado" ||
                            o.Status == "Finalizado")
                        .ToString();

                    // 🔥 FATURAMENTO TOTAL
                    decimal faturamentoTotal = orcamentos
                        .Where(o =>
                            o.Status == "Aprovado" ||
                            o.Status == "Finalizado")
                        .Sum(o => o.Total);

                    FaturamentoText.Text =
                        $"R$ {faturamentoTotal:N2}";

                    // 🔥 FATURAMENTO DO MÊS
                    decimal faturamentoMes = orcamentos
                        .Where(o =>
                            (o.Status == "Aprovado" ||
                             o.Status == "Finalizado")
                            &&
                            o.DataCriacao.Month == DateTime.Now.Month
                            &&
                            o.DataCriacao.Year == DateTime.Now.Year)
                        .Sum(o => o.Total);

                    FaturamentoMesText.Text =
                        $"R$ {faturamentoMes:N2}";

                    // 🔥 LUCRO BRUTO
                    decimal lucroBruto = faturamentoMes;

                    LucroBrutoText.Text =
                        $"R$ {lucroBruto:N2}";

                    // 🔥 GRID
                    var listaGrid = orcamentos
                        .OrderByDescending(o => o.Id)
                        .Select(o => new
                        {
                            o.Id,

                            Cliente = db.Clientes
                                .FirstOrDefault(c => c.Id == o.ClienteId)?.Nome,

                            Total = $"R$ {o.Total:N2}",

                            o.Status
                        })
                        .Take(10)
                        .ToList();

                    OrcamentosGrid.ItemsSource = listaGrid;
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

        // 🔥 ORÇAMENTOS
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
                MessageBox.Show(
                    "Erro ao abrir orçamentos:\n" + ex.Message);
            }
        }

        // 🔥 CLIENTES
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
                MessageBox.Show(
                    "Erro ao abrir clientes:\n" + ex.Message);
            }
        }

        // 🔥 HISTÓRICO
        private void AbrirHistorico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tela = new HistoricoWindow();

                tela.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao abrir histórico:\n" + ex.Message);
            }
        }

        // 🔥 RELATÓRIO
        private void AbrirRelatorio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tela = new RelatorioWindow();

                tela.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao abrir relatório:\n" + ex.Message);
            }
        }

        // 🔥 SAIR
        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // 🔥 APROVAR
        private void Aprovar_Click(object sender, RoutedEventArgs e)
        {
            AlterarStatus("Aprovado");
        }

        // 🔥 EM ANDAMENTO
        private void Andamento_Click(object sender, RoutedEventArgs e)
        {
            AlterarStatus("Em andamento");
        }

        // 🔥 FINALIZAR
        private void Finalizar_Click(object sender, RoutedEventArgs e)
        {
            AlterarStatus("Finalizado");
        }

        // 🔥 ALTERAR STATUS
        private void AlterarStatus(string novoStatus)
        {
            try
            {
                dynamic selecionado = OrcamentosGrid.SelectedItem;

                if (selecionado == null)
                {
                    MessageBox.Show("Selecione um orçamento.");

                    return;
                }

                int id = selecionado.Id;

                using (var db = new AppDbContext())
                {
                    var item = db.Orcamentos
                        .FirstOrDefault(o => o.Id == id);

                    if (item != null)
                    {
                        item.Status = novoStatus;

                        db.SaveChanges();
                    }
                }

                CarregarDashboard();

                MessageBox.Show(
                    "Status atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao alterar status:\n" + ex.Message);
            }
        }
    }
}