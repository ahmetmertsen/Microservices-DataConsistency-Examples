using MassTransit;
using Order.API.Models;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        private readonly OrderApiDbContext _contextDb;

        public OrderCompletedEventConsumer(OrderApiDbContext contextDb)
        {
            _contextDb = contextDb;
        }

        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            var order = await _contextDb.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatus = Models.Enums.OrderStatus.Completed;
                await _contextDb.SaveChangesAsync();
            }
        }
    }
}
