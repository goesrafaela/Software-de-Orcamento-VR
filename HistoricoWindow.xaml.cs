using System.Collections.Generic;
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

            CarregarDados();
        }

        // 🔥 MODEL DO GRID
        public class HistoricoItem
        {
            public int Id { get; set; }

            public string Cliente { get; set; } = "";

            public string Servicos { get; set; } = "";

            public decimal Total { get; set; }

            public string Status { get; set; } = "";

            public bool PrimeiraParcelaPaga { get; set; }

            public bool SegundaParcelaPaga { get; set; }

            public bool TerceiraParcelaPaga { get; set; }
        }

        // 🔥 CARREGAR DADOS
        private void CarregarDados()
        {
            try
            {
                 using (var db = new AppDbContext())
                {
                    var lista = db.Orcamentos
                        .ToList()
                        .Select(o => new
                        {
                            o.Id,

                            Cliente = db.Clientes
                                .FirstOrDefault(c => c.Id == o.ClienteId)?.Nome,

                            Servicos = string.Join(", ",
                                db.OrcamentoItens
                                .Where(i => i.OrcamentoId == o.Id)
                                .Select(i => i.Descricao)),

                            o.Total,
                            o.Status,
                            o.Parcelas,

                            Pagamentos = Enumerable.Range(1, o.Parcelas)
                                .Select(p => new
                                {
                                    Nome = $"{p}ª",
                                    Pago = false
                                })
                                .ToList()
                        })
                        .ToList();

                    HistoricoGrid.ItemsSource = lista;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar histórico:\n\n" + ex.Message
                );
            }
        }

        // 🎨 CORES
        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var item = e.Row.Item as HistoricoItem;

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
            }
        }

        // 💾 SALVAR PAGAMENTOS
        private void SalvarPagamentos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lista = HistoricoGrid.ItemsSource as List<HistoricoItem>;

                if (lista == null)
                    return;

                using (var db = new AppDbContext())
                {
                    foreach (var item in lista)
                    {
                        var orc = db.Orcamentos.Find(item.Id);

                        if (orc != null)
                        {
                            orc.PrimeiraParcelaPaga = item.PrimeiraParcelaPaga;

                            orc.SegundaParcelaPaga = item.SegundaParcelaPaga;

                            orc.TerceiraParcelaPaga = item.TerceiraParcelaPaga;
                        }
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Pagamentos atualizados!");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar pagamentos:\n\n" + ex.Message
                );
            }
        }
    }
}