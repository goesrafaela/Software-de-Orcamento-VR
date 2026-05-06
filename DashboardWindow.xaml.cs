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
            CarregarDashboard();
        }

        public void CarregarDashboard()
        {
            using (var db = new AppDbContext())
            {
                // Clientes
                ClientesCount.Text = db.Clientes.Count().ToString();

                // Status
                EmAndamentoCount.Text = db.Orcamentos
                    .Count(o => o.Status == "Em andamento")
                    .ToString();

                AprovadosCount.Text = db.Orcamentos
                    .Count(o => o.Status == "Aprovado")
                    .ToString();

                // 💰 Faturamento (somente aprovados)
                var faturamento = db.Orcamentos
                    .Where(o => o.Status == "Aprovado")
                    .Sum(o => (decimal?)o.Total) ?? 0;

                FaturamentoText.Text = $"R$ {faturamento:N2}";

                // 📊 Lista
                OrcamentosGrid.ItemsSource = db.Orcamentos
                    .OrderByDescending(o => o.Id)
                    .Take(10)
                    .ToList();
            }
        }

        private void AbrirOrcamentos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    if (!db.Clientes.Any())
                    {
                        MessageBox.Show("Cadastre um cliente antes.");
                        return;
                    }
                }

                var tela = new OrcamentosWindow();
                tela.ShowDialog();

                CarregarDashboard(); // 🔥 atualiza ao fechar
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro:\n" + ex.Message);
            }
        }

        private void AbrirHistorico_Click(object sender, RoutedEventArgs e)
        {
            var tela = new HistoricoWindow();
            tela.ShowDialog();
        }

        private void AbrirClientes_Click(object sender, RoutedEventArgs e)
        {
            var tela = new ClientesWindow();
            tela.ShowDialog();

            CarregarDashboard(); // 🔥 atualiza ao fechar
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}