using System;
using System.IO;
using System.Linq;
using OrcaPro.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OrcaPro.Services
{
    public static class PdfService
    {
        public static void GerarOrcamento(
            int orcamentoId,
            string responsavel,
            int parcelas,
            DateTime primeiroVencimento)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            using (var db = new AppDbContext())
            {
                var orc = db.Orcamentos
                    .FirstOrDefault(o => o.Id == orcamentoId);

                if (orc == null)
                    return;

                var cliente = db.Clientes
                    .FirstOrDefault(c => c.Id == orc.ClienteId);

                var itens = db.OrcamentoItens
                    .Where(i => i.OrcamentoId == orc.Id)
                    .ToList();

                var pasta = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop),
                    "Orcamentos");

                Directory.CreateDirectory(pasta);

                var caminho = Path.Combine(
                    pasta,
                    $"Orcamento_{orc.Id}.pdf");

                decimal valorParcela = orc.Total / parcelas;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        // CABEÇALHO
                        page.Header()
                            .Text("ORÇAMENTO")
                            .FontSize(24)
                            .Bold();

                        // CONTEÚDO
                        page.Content().Column(col =>
                        {
                            col.Spacing(10);

                            col.Item().Text($"Cliente: {cliente?.Nome}");

                            col.Item().Text($"Status: {orc.Status}");

                            col.Item().Text($"Valor Total: R$ {orc.Total:N2}");

                            col.Item().LineHorizontal(1);

                            // ITENS
                            foreach (var item in itens)
                            {
                                col.Item().Text(
                                    $"{item.Descricao} | " +
                                    $"Qtd: {item.Quantidade} | " +
                                    $"Unit: R$ {item.ValorUnitario:N2} | " +
                                    $"Total: R$ {item.Total:N2}");
                            }

                            // 🔥 PARCELAMENTO
                            col.Item().PaddingTop(20);

                            col.Item()
                                .Text("Parcelamento")
                                .FontSize(18)
                                .Bold();

                            for (int i = 0; i < parcelas; i++)
                            {
                                var data = primeiroVencimento.AddMonths(i);

                                col.Item().Text(
                                    $"{i + 1}ª parcela - " +
                                    $"R$ {valorParcela:N2} - " +
                                    $"Vencimento: {data:dd/MM/yyyy}");
                            }

                            // ASSINATURA
                            col.Item().PaddingTop(40);

                            col.Item().Text("________________________________");

                            col.Item().Text(responsavel);

                            col.Item().Text("Assinatura Responsável");
                        });

                        // RODAPÉ
                        page.Footer()
                            .AlignCenter()
                            .Text("VR Reservatórios");
                    });
                })
                .GeneratePdf(caminho);

                // ABRE PDF
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = caminho,
                        UseShellExecute = true
                    });
            }
        }
    }
}