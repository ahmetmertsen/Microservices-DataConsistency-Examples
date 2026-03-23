using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.StateDbContexts;
using SagaStateMachine.Service.StateIstances;
using SagaStateMachine.Service.StateMachines;
using Shared.Settings;

namespace SagaStateMachine.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            //builder.Services.AddHostedService<Worker>();

            builder.Services.AddMassTransit(configurator =>
            {
                configurator.AddSagaStateMachine<OrderStateMachine, OrderStateIstance>()
                    .EntityFrameworkRepository(options =>
                    {
                        options.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
                        {
                            _builder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                        });
                        options.UsePostgres();
                    });

                configurator.UsingRabbitMq((context, _configure) =>
                {
                    _configure.Host(builder.Configuration["RabbitMQ"]);

                    _configure.ReceiveEndpoint(RabbitMQSettings.StateMachineQueue, e => e.ConfigureSaga<OrderStateIstance>(context));
                });
            });

            var host = builder.Build();
            host.Run();
        }
    }
}