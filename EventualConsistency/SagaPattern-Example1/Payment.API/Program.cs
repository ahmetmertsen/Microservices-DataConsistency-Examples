
using MassTransit;
using Payment.API.Consumers;
using Shared;

namespace Payment.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<StockReservedEventConsumer>();
                configurator.UsingRabbitMq((context, _configure) =>
                {
                    _configure.Host(builder.Configuration["RabbitMQ"]);
                    _configure.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue, e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
                });
            });

            var app = builder.Build();

            app.Run();
        }
    }
}
