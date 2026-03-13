
using Coordinator.Models;
using Coordinator.Models.Requests;
using Coordinator.Services;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHttpClient("Accounts.API", client => client.BaseAddress = new("https://localhost:7025"));
            builder.Services.AddHttpClient("Ledger.API", client => client.BaseAddress = new("https://localhost:7039"));

            builder.Services.AddTransient<ITransactionService, TransactionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapPost("create-transfer-transaction", async (CreateTransferRequest request, ITransactionService transactionService) =>
            {
                var transactionId = await transactionService.CreateTransactionAsync();
                await transactionService.PreapreServicesAsync(transactionId, request);
                bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

                if (transactionState) 
                {
                    await transactionService.CommitAsync(transactionId);
                    transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
                }
                if (!transactionState)
                {
                    await transactionService.RoolbackAsync(transactionId);
                }
            });

            app.Run();
        }
    }
}
