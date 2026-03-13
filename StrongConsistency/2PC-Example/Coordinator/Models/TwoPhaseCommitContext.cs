using Coordinator.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models
{
    public class TwoPhaseCommitContext : DbContext
    {
        public TwoPhaseCommitContext(DbContextOptions options) : base(options) { }
        
        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>()
                .HasData(
                    new Node("Accounts.API") { Id = Guid.NewGuid() },
                    new Node("Ledger.API") { Id = Guid.NewGuid() }
                );
        }
    }
}
