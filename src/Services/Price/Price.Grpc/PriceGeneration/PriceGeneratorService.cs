using MassTransit.Mediator;
using MessageBroker.Events;
using System.Collections.Concurrent;

namespace Price.Grpc.PriceGeneration
{
    public class PriceGeneratorService : IPriceGeneratorService
    {
        private readonly ILogger<PriceGeneratorService> _logger;

        public PriceGeneratorService(ILogger<PriceGeneratorService> logger)
        {
            _logger = logger;
        }

        private static ConcurrentDictionary<string, decimal> stocks = new();

        private string[] stockNames = ["ACRE", "ACT", "AE", "AER", "CDT", "CEAD", "CENX", "DNMR", "DOCN", "FTS"];

        public  StockPrice[] GeneratePrices()
        {
            if (!stocks.Any())
            {
                SetPrices();
            }
            else 
            {
                ChangePrices();
            }

            return stocks.Select(pair => new StockPrice(pair.Key, pair.Value)).ToArray();

        }

        public decimal GetPrice(string ticker)
        {
            stocks.TryGetValue(ticker, out decimal price);
            return price;
        }

        private void SetPrices() 
        {
            Random rnd = new Random();
            Parallel.ForEach(stockNames, (i, token) =>
            {
                stocks.TryAdd(i, rnd.NextDeciaml(5, 10));
            });
        }

        private void ChangePrices() 
        {
            Random rnd = new Random();
            Parallel.ForEach(stockNames, (i, token) =>
            {
                if( stocks.TryGetValue(i, out decimal value))
                {
                    var t = rnd.NextDeciaml(0.9m, 1.11m);
                    _logger.LogInformation("{t}", t);
                    stocks[i] = value * rnd.NextDeciaml(0.9m, 1.1m);
                }
            });
        }
    }

    public static class RandomExtension 
    {
        public static decimal NextDeciaml(this Random random, decimal first, decimal second) 
        {
            int firstAsInt = (int)(first * 100);
            int secondInt = (int)(second * 100);

            int intResult = random.Next(firstAsInt, secondInt);
            return (decimal)intResult / 100;
        }
    }
}


