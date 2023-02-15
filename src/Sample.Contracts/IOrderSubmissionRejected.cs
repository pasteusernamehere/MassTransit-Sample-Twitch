namespace Sample.Contracts;

using System;

public interface IOrderSubmissionRejected
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
    string CustomerNumber { get; }
    string Reason { get; }
}