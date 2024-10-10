using Microsoft.EntityFrameworkCore;

namespace Order.API.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions options)
       : base(options)
        {
        }

        public DbSet<Entities.Order> Orders { get; set; }
    }
}
