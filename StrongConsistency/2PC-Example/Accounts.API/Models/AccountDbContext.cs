using Accounts.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accounts.API.Models
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTransferIntent> AccountTransferIntents { get; set; }
    }
}
