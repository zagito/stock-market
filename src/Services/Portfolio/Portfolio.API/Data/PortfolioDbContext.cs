using Microsoft.EntityFrameworkCore;
using Portfolio.API.Data.Entities;

namespace Portfolio.API.Data
{
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions options)
       : base(options)
        {
        }

        public DbSet<Entities.Portfolio> Portfolios { get; set; }

        public DbSet<Stock> Stocks { get; set; }

        public DbSet<PortfolioStock> PortfolioStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Stock>(entity => {
                entity.HasIndex(e => e.Ticker).IsUnique();
            });
        }
    }
}
