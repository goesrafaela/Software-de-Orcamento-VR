using System.ComponentModel.DataAnnotations;

namespace OrcaPro.Models
{
    public class OrcamentoItem
    {
        [Key]
        public int Id { get; set; }

        public int OrcamentoId { get; set; }

        public string Descricao { get; set; } = "";

        public int Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal Total { get; set; }
    }
}