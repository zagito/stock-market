namespace Price.Grpc.PriceGeneration
{
    public interface IPriceGeneratorService
    {
        public decimal GetPrice(string ticker);

        public Dictionary<string, decimal> GeneratePrices();
    }
}
