namespace Sample.Contracts;

public interface IOrderSubmissionAccepted
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
    string CustomerNumber { get; }
}