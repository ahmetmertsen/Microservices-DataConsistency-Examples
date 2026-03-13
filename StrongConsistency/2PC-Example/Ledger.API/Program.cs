
using Ledger.API.Models;
using Ledger.API.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace Ledger.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<LedgerDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            app.MapPost("/ready", async (PrepareTransferRequest request, LedgerDbContext _context) =>
            {
                if (request.Amount <= 0) return false;

                var existing = await _context.Ledgers.FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId);
                if (existing != null)
                {
                    return true;
                }

                await _context.Ledgers.AddAsync(new Models.Entities.Ledger
                {
                    TransactionId = request.TransactionId,
                    FromAccountId = request.FromAccountId,
                    ToAccountId = request.ToAccountId,
                    Amount = request.Amount,
                    Status = Models.Enums.Status.Ready,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return true;
            });

            app.MapGet("/commit/{transactionId}", async (string transactionId, LedgerDbContext _context) =>
            {
                var intent = await _context.Ledgers.FirstOrDefaultAsync(x => x.TransactionId == transactionId);

                if (intent == null)
                {
                    return false;
                }

                intent.Status = Models.Enums.Status.Committed;

                await _context.SaveChangesAsync();
                return true;
            });

            app.MapGet("/rollback/{transactionId}", async (string transactionId, LedgerDbContext _context) =>
            {
                var intent = await _context.Ledgers.FirstOrDefaultAsync(x => x.TransactionId == transactionId);

                if (intent == null) return false;

                intent.Status = Models.Enums.Status.Rolledback;
                await _context.SaveChangesAsync();
                return true;
            });


            app.Run();
        }
    }
}
