using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Components.Consumers;
using Sample.Components.StateMachines;

var isService = !(Debugger.IsAttached || args.Contains("--console"));

var builder = new HostBuilder().ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", true);
        config.AddEnvironmentVariables();

        if (args != null)
        {
            config.AddCommandLine(args);
        }
    })
    .ConfigureServices((hostingContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
            x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .RedisRepository("127.0.0.1");

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        services.AddHostedService<MassTransitHostedService>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

if (isService)
{
    await builder.UseWindowsService()
        .Build()
        .RunAsync();
}
else
{
    await builder.RunConsoleAsync();
}