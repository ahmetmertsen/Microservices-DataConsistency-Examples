using MassTransit;
using Order.API.Models;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderFailedEventConsumer : IConsumer<OrderFailedEvent>
    {
        private readonly OrderApiDbContext _contextDb;

        public OrderFailedEventConsumer(OrderApiDbContext contextDb)
        {
            _contextDb = contextDb;
        }

        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
            var order = await _contextDb.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatus = Models.Enums.OrderStatus.Failed;
                await _contextDb.SaveChangesAsync();
            }
        }
    }
}
