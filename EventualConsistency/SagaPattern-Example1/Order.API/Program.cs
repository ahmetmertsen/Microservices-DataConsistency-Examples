
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Order.API.Models.Dtos;
using Order.API.Models.Entities;
using Shared;
using Shared.Events;

namespace Order.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<OrderApiDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<StockNotReservedEventConsumer>();
                configurator.AddConsumer<PaymentCompletedEventConsumer>();
                configurator.AddConsumer<PaymentFailedEventConsumer>();

                configurator.UsingRabbitMq((context, _configure) =>
                {
                    _configure.Host(builder.Configuration["RabbitMQ"]);
                    _configure.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue, e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
                    _configure.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue, e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
                    _configure.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapPost("/create-order", async (CreateOrderDto dto, OrderApiDbContext context, IPublishEndpoint publishEndpoint) =>
            {
                var productIds = dto.OrderItems.Select(i => i.ProductId).Distinct().ToList();

                var products = await context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                var priceMap = products.ToDictionary(p => p.Id, p => p.Price);

                var orderItems = dto.OrderItems.Select(oi => new OrderItem
                {
                    ProductId = oi.ProductId,
                    Count = oi.Count,
                    Price = priceMap[oi.ProductId]
                }).ToList();

                var order = new Models.Entities.Order
                {
                    BuyerId = dto.BuyerId,
                    OrderItems = orderItems,
                    OrderStatus = Models.Enums.OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    TotalPrice = orderItems.Sum(i => i.Price * i.Count)
                };
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();

                await publishEndpoint.Publish(new OrderCreatedEvent
                {
                    BuyerId = order.BuyerId,
                    OrderId = order.Id,
                    TotalPrice = order.TotalPrice,
                    OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage
                    {
                        Count = oi.Count,
                        Price = oi.Price,
                        ProductId = oi.ProductId
                    }).ToList()
                });
            });


            app.Run();
        }
    }
}
