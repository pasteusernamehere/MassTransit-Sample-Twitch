namespace Sample.Contracts
{
    using System;

    public interface IOrderSubmissionAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }
}