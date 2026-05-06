using System.IO;
using System.Linq;
using OrcaPro.Models;
using OrcaPro.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace OrcaPro.Services
{
    public class PdfService
    {
        public static void GerarOrcamento(int orcamentoId)
        {
            using (var db = new AppDbContext())
            {
                var orc = db.Orcamentos.First(o => o.Id == orcamentoId);
                var itens = db.OrcamentoItens.Where(i => i.OrcamentoId == orcamentoId).ToList();
                var cliente = db.Clientes.First(c => c.Id == orc.ClienteId);

                string caminho = $"Orcamento_{orc.Id}.pdf";

                var writer = new PdfWriter(caminho);
                var pdf = new PdfDocument(writer);
                var doc = new Document(pdf);

                doc.Add(new Paragraph("ORÇAMENTO")
                    .SetFontSize(20));

                doc.Add(new Paragraph($"Cliente: {cliente.Nome}"));
                doc.Add(new Paragraph($"Data: {orc.Data}"));

                doc.Add(new Paragraph(" "));

                foreach (var item in itens)
                {
                    doc.Add(new Paragraph(
                        $"{item.Descricao} - Qtd: {item.Quantidade} - R$ {item.Total}"
                    ));
                }

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph($"TOTAL: R$ {orc.Total}")
                    .SetFontSize(16));

                doc.Close();
            }
        }
    }
}