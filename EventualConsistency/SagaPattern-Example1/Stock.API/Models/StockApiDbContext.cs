using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Stock.API.Models
{
    public class StockApiDbContext : DbContext
    {
        public StockApiDbContext(DbContextOptions<StockApiDbContext> options) : base(options) { }

        public DbSet<Entities.Stock> Stocks { get; set; }
    }
}
