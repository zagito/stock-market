using MassTransit;
using MessageBroker.Events;

namespace Portfolio.API.Features.StockPrices
{
    public class PricesChangedConsumer : IConsumer<PricesChangedEvent>
    {

        private readonly ILogger<PricesChangedConsumer> _logger;

        public PricesChangedConsumer(ILogger<PricesChangedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PricesChangedEvent> context)
        {
            string s = context.Message.StockPrices.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2);
            _logger.LogInformation("New prices incoming {Prices}", s);
            return Task.CompletedTask;
        }
    }
}
