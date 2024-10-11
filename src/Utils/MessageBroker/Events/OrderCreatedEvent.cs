namespace MessageBroker.Events
{
    public record OrderCreatedEvent(Guid OrderId, Guid UserId, int Quantity, string Ticker, decimal Price, bool IsSell);
}
