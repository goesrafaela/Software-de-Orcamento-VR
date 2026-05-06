using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OrcaPro.Data;
using OrcaPro.Services;
using OrcaPro.Models;


namespace OrcaPro
{
    public partial class OrcamentosWindow : Window
    {
        private List<OrcamentoItem> itens = new();
        private int ultimoOrcamentoId = 0;

        public OrcamentosWindow()
        {
            InitializeComponent();
            CarregarClientes();
            AtualizarGrid();
        }

        private void CarregarClientes()
        {
            using (var db = new AppDbContext())
            {
                var lista = db.Clientes.ToList();

                ClienteCombo.ItemsSource = lista;
                ClienteCombo.DisplayMemberPath = "Nome";
                ClienteCombo.SelectedValuePath = "Id";

                if (lista.Count > 0)
                    ClienteCombo.SelectedIndex = 0;
            }
        }
        private void ItensGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var item = e.Row.Item as OrcamentoItem;

            if (item == null) return;

            // exemplo de destaque
            if (item.Total > 1000)
            {
                e.Row.Background = Brushes.LightGreen;
            }
        }

        // 🔥 ADICIONAR ITEM
        private void AdicionarItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DescricaoBox.Text))
            {
                MessageBox.Show("Digite a descrição.");
                return;
            }

            if (!int.TryParse(QtdBox.Text, out int qtd))
            {
                MessageBox.Show("Quantidade inválida.");
                return;
            }

            if (!decimal.TryParse(ValorBox.Text, out decimal valor))
            {
                MessageBox.Show("Valor inválido.");
                return;
            }

            var item = new OrcamentoItem
            {
                Descricao = DescricaoBox.Text,
                Quantidade = qtd,
                ValorUnitario = valor,
                Total = qtd * valor
            };

            itens.Add(item);

            AtualizarGrid();
            LimparCampos();
        }

        // 🔄 GRID + TOTAL
        private void AtualizarGrid()
        {
            ItensGrid.ItemsSource = null;
            ItensGrid.ItemsSource = itens;

            decimal total = itens.Sum(i => i.Total);
            TotalText.Text = $"TOTAL: R$ {total:N2}";
        }

        private void LimparCampos()
        {
            DescricaoBox.Text = "";
            QtdBox.Text = "";
            ValorBox.Text = "";
        }

        // ❌ EXCLUIR ITEM
        private void ExcluirItem_Click(object sender, RoutedEventArgs e)
        {
            var item = ItensGrid.SelectedItem as OrcamentoItem;

            if (item == null)
            {
                MessageBox.Show("Selecione um item.");
                return;
            }

            itens.Remove(item);
            AtualizarGrid();
        }

        // 💾 SALVAR ORÇAMENTO
        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            if (ClienteCombo.SelectedValue == null)
            {
                MessageBox.Show("Selecione um cliente.");
                return;
            }

            if (itens.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um item.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var orc = new Orcamento
                {
                    ClienteId = (int)ClienteCombo.SelectedValue,
                    Total = itens.Sum(i => i.Total),
                    Status = "Em andamento"
                };

                db.Orcamentos.Add(orc);
                db.SaveChanges();

                foreach (var item in itens)
                {
                    item.OrcamentoId = orc.Id;
                    db.OrcamentoItens.Add(item);
                }

                db.SaveChanges();

                ultimoOrcamentoId = orc.Id;
            }

            MessageBox.Show("Orçamento salvo com sucesso!");

            itens.Clear();
            AtualizarGrid();

            AtualizarDashboard();
        }

        // 📄 GERAR PDF
        private void GerarPdf_Click(object sender, RoutedEventArgs e)
        {
            if (ultimoOrcamentoId == 0)
            {
                MessageBox.Show("Salve um orçamento antes de gerar PDF.");
                return;
            }

            PdfService.GerarOrcamento(ultimoOrcamentoId);

            MessageBox.Show("PDF gerado com sucesso!");
        }

        // ✅ APROVAR
        private void Aprovar_Click(object sender, RoutedEventArgs e)
        {
            AtualizarStatus("Aprovado");
        }

        // ✅ FINALIZAR
        private void Finalizar_Click(object sender, RoutedEventArgs e)
        {
            AtualizarStatus("Finalizado");
        }

        // 🔥 ALTERA STATUS
        private void AtualizarStatus(string status)
        {
            if (ultimoOrcamentoId == 0)
            {
                MessageBox.Show("Salve um orçamento primeiro.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var orc = db.Orcamentos.Find(ultimoOrcamentoId);

                if (orc != null)
                {
                    orc.Status = status;
                    db.SaveChanges();
                }
            }

            MessageBox.Show($"Orçamento {status}!");

            AtualizarDashboard();
        }

        // 🎨 COR AUTOMÁTICA (se usar grid de orçamentos)
        private void OrcamentosGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var orc = e.Row.Item as Orcamento;

            if (orc == null) return;

            switch (orc.Status)
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

        // 🔄 ATUALIZA DASHBOARD (CORRETO)
        private void AtualizarDashboard()
        {
            var dashboard = Application.Current.Windows
                .OfType<DashboardWindow>()
                .FirstOrDefault();

            dashboard?.CarregarDashboard(); // 🔥 agora sem reflection
        }
    }
}