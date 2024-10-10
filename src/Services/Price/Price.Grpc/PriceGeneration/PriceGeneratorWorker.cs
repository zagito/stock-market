using MassTransit;
using MessageBroker.Events;
using System.Threading;

namespace Price.Grpc.PriceGeneration
{
    public class PriceGeneratorWorker : BackgroundService
    {
        private readonly ILogger<PriceGeneratorWorker> _logger;
        private readonly IPriceGeneratorService _priceGeneratorService;
        //private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBus _bus;

        public PriceGeneratorWorker(ILogger<PriceGeneratorWorker> logger, IPriceGeneratorService priceGeneratorService, IBus bus)
        {
            _logger = logger;
            _priceGeneratorService = priceGeneratorService;
            //_publishEndpoint = publishEndpoint;
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
