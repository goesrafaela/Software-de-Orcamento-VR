using System;

namespace OrcaPro.Models
{
    public class Orcamento
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = "Em andamento";

        // 🔥 NOVOS CAMPOS
        public int Parcelas { get; set; }

        public string Responsavel { get; set; } = "";

        public DateTime PrimeiroVencimento { get; set; }

        public bool PrimeiraParcelaPaga { get; set; }

        public bool SegundaParcelaPaga { get; set; }

        public bool TerceiraParcelaPaga { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}