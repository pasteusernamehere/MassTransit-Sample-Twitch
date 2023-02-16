namespace Sample.Components.Consumers;

using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

public class SubmitOrderConsumer : IConsumer<ISubmitOrder>
{
    private readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ISubmitOrder> context)
    {
        _logger.Log(LogLevel.Debug, "{Consumer}: {CustomerNumber}", nameof(SubmitOrderConsumer),
            context.Message.CustomerNumber);

        //Implement validation etc... reach out to repositories...  

        if (context.Message.CustomerNumber.Contains("TEST"))
        {
            await context.RespondAsync<IOrderSubmissionRejected>(new
            {
                context.Message.OrderId,
                InVar.Timestamp,
                context.Message.CustomerNumber,
                Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
            });

            return;
        }

        await context.RespondAsync<IOrderSubmissionAccepted>(new
        {
            context.Message.OrderId,
            InVar.Timestamp,
            context.Message.CustomerNumber
        });
    }
}