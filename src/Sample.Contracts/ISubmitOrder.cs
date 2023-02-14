namespace Sample.Contracts
{
    using System;

    public interface ISubmitOrder
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }
}