using MassTransit;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly OrderApiDbContext _context;

        public StockNotReservedEventConsumer(OrderApiDbContext context) { _context = context; }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);
            if (order == null)
            {
                throw new NullReferenceException();
            }
            order.OrderStatus = Models.Enums.OrderStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}
