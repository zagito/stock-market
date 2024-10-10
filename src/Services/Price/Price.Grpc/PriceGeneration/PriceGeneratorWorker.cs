using MassTransit;
using MessageBroker.Events;

namespace Price.Grpc.PriceGeneration
{
    public class PriceGeneratorWorker : BackgroundService
    {
        private readonly IPriceGeneratorService _priceGeneratorService;
        private readonly IBus _bus;

        public PriceGeneratorWorker(IPriceGeneratorService priceGeneratorService, IBus bus)
        {
            _priceGeneratorService = priceGeneratorService;
            _bus = bus;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var prices = _priceGeneratorService.GeneratePrices();

                await _bus.Publish(new PricesChangedEvent(prices));

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
