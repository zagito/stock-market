namespace MessageBroker.Events
{
    public record OrderExecutedEvent(Guid OrderId, DateTime ExecutedDate, bool IsSuccess, string FailureReason);
}
