namespace Sample.Api.Controllers;

using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IRequestClient<ISubmitOrder> _submitOrderRequestClient;

    public OrderController(ILogger<OrderController> logger, IRequestClient<ISubmitOrder> submitOrderRequestClient)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid id, string customerNumber)
    {
        //Uses request client to send the request and awaits the response
        //Sample.Api makes request (Publish)
        //  => Sample.Service (Subscribes) via SubmitOrderConsumer
        //     Sample.Service consumes the message published by Sample.Api
        //     it does some work, validation, lookups etc
        //     Sample.Service eventually responds with its own message be it accepted or rejected
        //     RequestClient waits for this response
        //     All channels/exchanges/queues are managed by GetResponse(IRequestClient) and RespondAsync(IConsumer)
        //and then responds with an appropriate message
        var (accepted, rejected) =
            await _submitOrderRequestClient.GetResponse<IOrderSubmissionAccepted, IOrderSubmissionRejected>(new
            {
                OrderId = id,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;
            return Accepted(response.Message);
        }
        else
        {
            var response = await rejected;
            return BadRequest(response.Message);
        }
    }
}