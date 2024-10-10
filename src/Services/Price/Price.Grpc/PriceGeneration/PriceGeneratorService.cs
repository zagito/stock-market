using MassTransit.Mediator;
using System.Collections.Concurrent;

namespace Price.Grpc.PriceGeneration
{
    public class PriceGeneratorService : IPriceGeneratorService
    {
        private static ConcurrentDictionary<string, decimal> stocks = new();

        private string[] stockNames = ["ACRE", "ACT", "AE", "AER", "CDT", "CEAD", "CENX", "DNMR", "DOCN", "FTS"];

        public Dictionary<string, decimal> GeneratePrices()
        {
            if (!stocks.Any())
            {
                SetPrices();
            }
            else 
            {
                ChangePrices();
            }

            return stocks.ToDictionary(stock => stock.Key, stock => stock.Value);
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

            var intResult = random.Next(firstAsInt, secondInt);
            return  (decimal)intResult / 100;
        }
    }
}


