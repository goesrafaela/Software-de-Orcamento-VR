using System;
using System.ComponentModel.DataAnnotations;

namespace OrcaPro.Models
{
    public class Orcamento
    {
        [Key]
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string Descricao { get; set; } = "";

        public decimal Valor { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
        
        public decimal Total { get; set; }

        public List<OrcamentoItem> Itens { get; set; } = new();
        public string Status { get; set; } = "Em andamento";

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}