using System.ComponentModel.DataAnnotations;

namespace OrcaPro.Models
{
    public class Usuario
    {
         [Key] // 👈 ISSO RESOLVE
        public int Id { get; set; }
       public string Nome { get; set; } = "";
       public string Email { get; set; } = "";
       public string Senha { get; set; } = "";
    }
}