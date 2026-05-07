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

                // 🔥 PASTA PDF
                var pasta = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop),
                    "Orcamentos");

                Directory.CreateDirectory(pasta);

                var caminho = Path.Combine(
                    pasta,
                    $"Orcamento_{orc.Id}.pdf");

                // 🔥 CAMINHO DA LOGO
                var logoPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "logo.png");

                decimal valorParcela = orc.Total / parcelas;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        // 🔥 HEADER
                        page.Header().Row(row =>
                        {
                            // LOGO
                            if (File.Exists(logoPath))
                            {
                                row.ConstantItem(120)
                                   .Height(70)
                                   .Image(logoPath);
                            }

                            // TEXTO
                            row.RelativeItem().Column(col =>
                            {
                                col.Item()
                                   .AlignRight()
                                   .Text("ORÇAMENTO")
                                   .FontSize(24)
                                   .Bold();

                                col.Item()
                                   .AlignRight()
                                   .Text($"Data: {DateTime.Now:dd/MM/yyyy}");

                                col.Item()
                                   .AlignRight()
                                   .Text($"Orçamento Nº {orc.Id}");
                            });
                        });

                        // 🔥 CONTEÚDO
                        page.Content().PaddingVertical(20).Column(col =>
                        {
                            col.Spacing(12);

                            // CLIENTE
                            col.Item().Text($"Cliente: {cliente?.Nome}")
                                .FontSize(14);

                            col.Item().Text($"Responsável: {responsavel}");

                            col.Item().Text($"Status: {orc.Status}");

                            col.Item().Text($"Valor Total: R$ {orc.Total:N2}")
                                .Bold()
                                .FontSize(16);

                            col.Item().LineHorizontal(1);

                            // 🔥 SERVIÇOS
                            col.Item()
                                .PaddingTop(10)
                                .Text("Serviços")
                                .Bold()
                                .FontSize(18);

                            foreach (var item in itens)
                            {
                                col.Item().Border(1).Padding(8).Column(itemCol =>
                                {
                                    itemCol.Item().Text(item.Descricao).Bold();

                                    itemCol.Item().Text(
                                        $"Quantidade: {item.Quantidade}");

                                    itemCol.Item().Text(
                                        $"Valor Unitário: R$ {item.ValorUnitario:N2}");

                                    itemCol.Item().Text(
                                        $"Total: R$ {item.Total:N2}");
                                });
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
                                    $"{i + 1}ª parcela — " +
                                    $"R$ {valorParcela:N2} — " +
                                    $"Vencimento: {data:dd/MM/yyyy}");
                            }

                            // 🔥 ASSINATURA
                            col.Item().PaddingTop(50);

                            col.Item()
                                .AlignCenter()
                                .Text("________________________________");

                            col.Item()
                                .AlignCenter()
                                .Text(responsavel);

                            col.Item()
                                .AlignCenter()
                                .Text("Responsável Técnico");
                        });

                        // 🔥 FOOTER
                        page.Footer()
                            .AlignCenter()
                            .Text("VR Reservatórios")
                            .FontSize(10);
                    });
                })
                .GeneratePdf(caminho);

                // 🔥 ABRE PDF
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