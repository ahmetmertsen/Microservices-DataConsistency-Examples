using Inventory.API.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;

namespace Inventory.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly InventoryApiDbContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(InventoryApiDbContext context, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            var stocks = await _context.Inventories.ToListAsync();

            foreach (var orderItem in context.Message.OrderItems)
            {
                bool hasStock = stocks.Any(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count);

                stockResult.Add(hasStock);
            }
            if (stockResult.TrueForAll(s => s.Equals(true)))
            {
                // Stock güncellenmesi
                foreach (var orderItem in context.Message.OrderItems)
                {
                    var stock = stocks.FirstOrDefault(s => s.ProductId == orderItem.ProductId);
                    if (stock != null)
                    {
                        stock.Count -= orderItem.Count;
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
