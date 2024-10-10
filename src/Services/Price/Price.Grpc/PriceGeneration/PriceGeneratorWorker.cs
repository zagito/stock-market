namespace Price.Grpc.PriceGeneration
{
    public class PriceGeneratorWorker : BackgroundService
    {
        private readonly ILogger<PriceGeneratorWorker> _logger;
        private readonly IPriceGeneratorService _priceGeneratorService;

        public PriceGeneratorWorker(ILogger<PriceGeneratorWorker> logger, IPriceGeneratorService priceGeneratorService)
        {
            _logger = logger;
            _priceGeneratorService = priceGeneratorService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var prices = _priceGeneratorService.GeneratePrices();

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
