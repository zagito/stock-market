using MassTransit;
using MessageBroker.Events;

namespace Portfolio.API.Features.StockPrices
{
    public class PricesChangedConsumer : IConsumer<PricesChangedEvent>
    {
        public Task Consume(ConsumeContext<PricesChangedEvent> context)
        {
            string s = context.Message.StockPrices.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2);
            Console.WriteLine(s);
            return Task.CompletedTask;
        }
    }
}
