using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var paymentOk = Random.Shared.Next(1, 21) > 10;
            if (paymentOk)
            {
                // Ödeme başarılı...
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };
                await _publishEndpoint.Publish(paymentCompletedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarılı...");
            }
            else
            {
                // Ödeme başarısız...
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Yetersiz bakiye....",
                    OrderItems = context.Message.OrderItems
                };
                await _publishEndpoint.Publish(paymentFailedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarısız...");
            }
        }
    }
}
