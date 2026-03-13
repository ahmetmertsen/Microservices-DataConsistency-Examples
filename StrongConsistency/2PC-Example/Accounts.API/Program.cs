
using Accounts.API.Models;
using Accounts.API.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Accounts.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.MapGet("/ready", async (PrepareTransferRequest request, AccountDbContext _context) =>
            {
                if (request.Amount <= 0) return false;

                var existing = await _context.AccountTransferIntents.FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId);
                if (existing == null)
                {
                    return false;
                }

                var from = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId);
                var to = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId);
                if (from == null || to == null) return false;
                if (from.Balance < request.Amount) return false;

                await _context.AccountTransferIntents.AddAsync(new Models.Entities.AccountTransferIntent
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

            app.MapGet("/commit/{transactionId}", async (string transactionId, AccountDbContext _context) =>
            {
                var intent = await _context.AccountTransferIntents.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
                if (intent == null)
                {
                    return false;
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                var from = await _context.Accounts.FirstAsync(a => a.Id == intent.FromAccountId);
                var to = await _context.Accounts.FirstAsync(a => a.Id == intent.ToAccountId);

                if (from.Balance < intent.Amount)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                from.Balance -= intent.Amount;
                to.Balance += intent.Amount;
                intent.Status = Models.Enums.Status.Committed;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            });

            app.MapGet("/rollback/{transactionId}", async (string transactionId, AccountDbContext _context) =>
            {
                var intent = await _context.AccountTransferIntents.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
                if(intent == null)
                {
                    return false;
                }

                intent.Status = Models.Enums.Status.Rolledback;
                await _context.SaveChangesAsync();
                return true;
            });

            app.Run();
        }
    }
}
