namespace Sample.Api.Controllers;

using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IRequestClient<ICheckOrder> _checkOrderClient;
    private readonly ILogger<OrderController> _logger;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IRequestClient<ISubmitOrder> _submitOrderRequestClient;

    public OrderController(ILogger<OrderController> logger, IRequestClient<ISubmitOrder> submitOrderRequestClient,
        ISendEndpointProvider sendEndpointProvider, IRequestClient<ICheckOrder> checkOrderClient)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
        _checkOrderClient = checkOrderClient;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await _checkOrderClient.GetResponse<IOrderStatus>(new { OrderId = id });

        return Ok(response.Message);
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

    [HttpPut]
    public async Task<IActionResult> Put(Guid id, string customerNumber)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:submit-order"));

        await endpoint.Send<ISubmitOrder>(new
        {
            OrderId = id,
            InVar.Timestamp,
            CustomerNumber = customerNumber
        });

        return Accepted();
    }
}