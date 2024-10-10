namespace MessageBroker.Events
{
    public record OrderCreatedEvent
    {
        public Guid Id { get; set; }
    }
}
