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

        // 🔥 ITEM PAGAMENTO
        public class PagamentoItem
        {
            public string Nome { get; set; } = "";

            public bool Pago { get; set; }
        }

        // 🔥 ITEM HISTÓRICO
        public class HistoricoItem
        {
            public int Id { get; set; }

            public string Cliente { get; set; } = "";

            public string Servicos { get; set; } = "";

            public decimal Total { get; set; }

            public string Status { get; set; } = "";

            public int Parcelas { get; set; }

            public List<PagamentoItem> Pagamentos { get; set; } = new();
        }

        // 🔥 CARREGAR DADOS
        private void CarregarDados()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var lista = new List<HistoricoItem>();

                    var orcamentos = db.Orcamentos.ToList();

                    foreach (var o in orcamentos)
                    {
                        var cliente = db.Clientes
                            .FirstOrDefault(c => c.Id == o.ClienteId);

                        var servicos = db.OrcamentoItens
                            .Where(i => i.OrcamentoId == o.Id)
                            .Select(i => i.Descricao)
                            .ToList();

                        var pagamentos = new List<PagamentoItem>();

                        for (int i = 1; i <= o.Parcelas; i++)
                        {
                            pagamentos.Add(new PagamentoItem
                            {
                                Nome = $"{i}ª Parcela",
                                Pago = false
                            });
                        }

                        lista.Add(new HistoricoItem
                        {
                            Id = o.Id,

                            Cliente = cliente?.Nome ?? "",

                            Servicos = string.Join(", ", servicos),

                            Total = o.Total,

                            Status = o.Status ?? "",

                            Parcelas = o.Parcelas,

                            Pagamentos = pagamentos
                        });
                    }

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
    }
}