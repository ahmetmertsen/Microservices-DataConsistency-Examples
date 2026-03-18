using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly StockApiDbContext _context;

        public PaymentFailedEventConsumer(StockApiDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var stocks = await _context.Stocks.ToListAsync();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = stocks.FirstOrDefault(s => s.ProductId == orderItem.ProductId);
                if (stock != null)
                {
                    stock.Count += orderItem.Count;
                }  
            }
            await _context.SaveChangesAsync();
        }
    }
}
