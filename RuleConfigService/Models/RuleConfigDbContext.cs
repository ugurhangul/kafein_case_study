using Microsoft.EntityFrameworkCore;

namespace RuleConfigService.Models
{
    public class RuleConfigDbContext(DbContextOptions<RuleConfigDbContext> options) : DbContext(options)
    {
        public DbSet<Rule> Rules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rule>().HasData(
                new Rule { Id = 1, EventType = "SELECT", IsCritical = false },
                new Rule { Id = 2, EventType = "DELETE", IsCritical = true },
                new Rule { Id = 3, EventType = "UPDATE", IsCritical = true }
            );
        }
    }
}