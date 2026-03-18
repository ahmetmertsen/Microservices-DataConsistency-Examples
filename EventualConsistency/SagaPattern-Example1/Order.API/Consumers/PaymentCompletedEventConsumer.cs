using MassTransit;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly OrderApiDbContext _context;

        public PaymentCompletedEventConsumer(OrderApiDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);
            if (order == null)
            {
                throw new NullReferenceException();
            }
            order.OrderStatus = Models.Enums.OrderStatus.Completed;
            await _context.SaveChangesAsync();
        }
    }
}
