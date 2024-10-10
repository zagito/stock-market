using MassTransit;
using MessageBroker.Events;

namespace Portfolio.API.Features.Orders
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine(context.Message.Id);
            return Task.CompletedTask;
        }
    }
}
