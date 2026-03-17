
using Inventory.API.Consumers;
using Inventory.API.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Inventory.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<InventoryApiDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<OrderCreatedEventConsumer>();

                configurator.UsingRabbitMq((context, _configure) =>
                {
                    _configure.Host(builder.Configuration["RabbitMQ"]);
                    _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
                });
            });

            var app = builder.Build();


            app.Run();
        }
    }
}
