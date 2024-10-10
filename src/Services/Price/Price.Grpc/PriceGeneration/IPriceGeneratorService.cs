using MessageBroker.Events;

namespace Price.Grpc.PriceGeneration
{
    public interface IPriceGeneratorService
    {
        public decimal GetPrice(string ticker);

        public StockPrice[] GeneratePrices();
    }
}
