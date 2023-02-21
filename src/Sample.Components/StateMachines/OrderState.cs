namespace Sample.Components.StateMachines;

using MassTransit;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public string CurrentState { get; set; }
    public DateTime? OrderDate { get; set; }
    public string CustomerNumber { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? Updated { get; set; }
    public int Version { get; set; }
    public Guid CorrelationId { get; set; }
}