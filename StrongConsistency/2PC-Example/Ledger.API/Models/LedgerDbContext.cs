using Microsoft.EntityFrameworkCore;

namespace Ledger.API.Models
{
    public class LedgerDbContext : DbContext
    {
        public LedgerDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Entities.Ledger> Ledgers { get; set; }
    }
}
