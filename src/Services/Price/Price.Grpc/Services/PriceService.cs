using Grpc.Core;
using Price.Grpc.PriceGeneration;

namespace Price.Grpc.Services
{
    public class PriceService : StockPriceProtoService.StockPriceProtoServiceBase
    {
        private readonly ILogger<PriceService> _logger;
        private readonly IPriceGeneratorService _priceGeneratorService;
        public PriceService(ILogger<PriceService> logger, IPriceGeneratorService priceGeneratorService)
        {
            _logger = logger;
            _priceGeneratorService = priceGeneratorService;
        }

        public override Task<GettStockPriceResponse> GetStockPrice(GettStockPriceRequest request, ServerCallContext context)
        {
            decimal price = _priceGeneratorService.GetPrice(request.Ticker);

            return Task.FromResult(new GettStockPriceResponse
            {
                Ticker = request.Ticker,
                Price = price
            });
        }
    }
}
