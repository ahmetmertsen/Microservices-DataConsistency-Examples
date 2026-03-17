using Inventory.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Models
{
    public class InventoryApiDbContext : DbContext
    {
        public InventoryApiDbContext(DbContextOptions<InventoryApiDbContext> options) : base(options) { }

        public DbSet<Entities.Inventory> Inventories { get; set; }
    }
}
