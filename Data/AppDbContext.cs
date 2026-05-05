using Microsoft.EntityFrameworkCore;
using OrcaPro.Models;

namespace OrcaPro.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=orcamento.db");
        }
    }
}