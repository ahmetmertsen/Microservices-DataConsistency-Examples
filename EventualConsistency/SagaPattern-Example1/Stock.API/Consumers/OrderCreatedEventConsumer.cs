using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly StockApiDbContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(StockApiDbContext context, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            var stocks = await _context.Stocks.ToListAsync();

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

                // Payment'ı uyaracak event'in fırlatılması
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems
                };
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                // Stock kontrolü başarısız.
                // Order'ı uyaracak event fırlatılacak.
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Stok miktarı başarısız"
                };
                await _publishEndpoint.Publish(stockNotReservedEvent);
            }
        }
    }
}
