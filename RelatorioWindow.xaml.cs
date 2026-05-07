using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OrcaPro.Data;

namespace OrcaPro
{
    public partial class RelatorioWindow : Window
    {
        public RelatorioWindow()
        {
            InitializeComponent();
            CarregarAnos();
        }

        private void CarregarAnos()
        {
            var anoAtual = DateTime.Now.Year;

            for (int i = anoAtual; i >= anoAtual - 5; i--)
            {
                AnoFiltro.Items.Add(i);
            }

            AnoFiltro.SelectedIndex = 0;
        }

        private void CarregarDados(object sender, RoutedEventArgs e)
        {
            if (AnoFiltro.SelectedItem == null) return;

            int ano = (int)AnoFiltro.SelectedItem;

            using (var db = new AppDbContext())
            {
                var dados = db.Orcamentos
                    .Where(o => o.Status == "Finalizado"  && o.DataCriacao.Year == ano)
                    .ToList()
                    .GroupBy(o => o.DataCriacao.Month)
                    .Select(g => new
                    {
                        Mes = g.Key,
                        Total = g.Sum(x => x.Total)
                    })
                    .OrderBy(x => x.Mes)
                    .ToList();

                RelatorioGrid.ItemsSource = dados;

                // TOTAL ANO
                var totalAno = dados.Sum(x => x.Total);
                TotalAnoText.Text = $"Total do ano: R$ {totalAno:N2}";

                // GRÁFICO
                GerarGrafico(dados);
            }
        }

        private void GerarGrafico(dynamic dados)
        {
            GraficoPanel.Items.Clear();

            foreach (var item in dados)
            {
                var barra = new Border
                {
                    Height = 30,
                    Margin = new Thickness(0, 5, 0, 5),
                    Background = Brushes.SteelBlue,
                    Width = (double)item.Total / 10 // escala simples
                };

                var texto = new TextBlock
                {
                    Text = $"Mês {item.Mes} - R$ {item.Total:N2}",
                    Margin = new Thickness(5)
                };

                var stack = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                stack.Children.Add(barra);
                stack.Children.Add(texto);

                GraficoPanel.Items.Add(stack);
            }
        }
    }
}