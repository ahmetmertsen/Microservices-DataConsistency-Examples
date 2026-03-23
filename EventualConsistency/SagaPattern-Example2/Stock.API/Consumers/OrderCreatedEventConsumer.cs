using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.OrderEvents;
using Shared.Settings;
using Shared.StockEvents;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly StockApiDbContext _contextDb;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderCreatedEventConsumer(StockApiDbContext contextDb, ISendEndpointProvider sendEndpointProvider)
        {
            _contextDb = contextDb;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            var stocks = await _contextDb.Stocks.ToListAsync();

            foreach (var orderItem in context.Message.OrderItems)
            {
                bool hasStock = stocks.Any(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count);

                stockResult.Add(hasStock);
            }

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
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
                await _contextDb.SaveChangesAsync();
                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Stok yetersiz..."
                };
                await sendEndpoint.Send(stockNotReservedEvent);
            }
        }
    }
}
