using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Messages;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class StockRollbackMessageConsumer : IConsumer<StockRollbackMessage>
    {
        private readonly StockApiDbContext _contextDb;

        public StockRollbackMessageConsumer(StockApiDbContext contextDb)
        {
            _contextDb = contextDb;
        }

        public async Task Consume(ConsumeContext<StockRollbackMessage> context)
        {
            var stocks = await _contextDb.Stocks.ToListAsync();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = stocks.FirstOrDefault(s => s.ProductId == orderItem.ProductId);
                if (stock != null)
                {
                    stock.Count += orderItem.Count;
                }   
            }
            await _contextDb.SaveChangesAsync();
        }
    }
}
