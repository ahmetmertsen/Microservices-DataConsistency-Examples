using Microsoft.EntityFrameworkCore;
using Order.API.Models.Entities;

namespace Order.API.Models
{
    public class OrderApiDbContext : DbContext
    {
        public OrderApiDbContext(DbContextOptions<OrderApiDbContext> options) : base(options) { }

        public DbSet<Entities.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(32);

            base.OnModelCreating(modelBuilder);
        }
    }
}
