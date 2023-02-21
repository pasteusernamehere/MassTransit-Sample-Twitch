namespace Sample.Components.StateMachines;

using Contracts;
using MassTransit;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));

        InstanceState(x => x.CurrentState);

        Initially(When(OrderSubmitted)
            .Then(context =>
            {
                context.Saga.CustomerNumber = context.Message.CustomerNumber;
                context.Saga.Updated ??= DateTime.UtcNow;
                context.Saga.SubmitDate ??= context.Message.Timestamp;
            })
            .TransitionTo(Submitted));

        During(Submitted, Ignore(OrderSubmitted));

        DuringAny(When(OrderSubmitted)
            .Then(context =>
            {
                context.Saga.CustomerNumber ??= context.Message.CustomerNumber;
                context.Saga.SubmitDate ??= context.Message.Timestamp;
            }));
    }

    public State Submitted { get; private set; }

    public Event<IOrderSubmitted> OrderSubmitted { get; private set; }
}