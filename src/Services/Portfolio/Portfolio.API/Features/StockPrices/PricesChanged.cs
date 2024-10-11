using MassTransit;
using MessageBroker.Events;
using Microsoft.EntityFrameworkCore;
using Portfolio.API.Data;
using Portfolio.API.Data.Entities;

namespace Portfolio.API.Features.StockPrices
{
    public class PricesChanged : IConsumer<PricesChangedEvent>
    {

        private readonly ILogger<PricesChanged> _logger;
        private readonly PortfolioDbContext _portfolioDbContext;

        public PricesChanged(ILogger<PricesChanged> logger, PortfolioDbContext portfolioDbContext)
        {
            _logger = logger;
            _portfolioDbContext = portfolioDbContext;
        }

        public async Task Consume(ConsumeContext<PricesChangedEvent> context)
        {
            if (!context.Message.StockPrices.Any())
            {
                _logger.LogWarning("No prices recieved {date}", DateTime.UtcNow);
                return;
            }

            var stocks = new List<Stock>();

            foreach (var stockPrice in context.Message.StockPrices)
            {
                stocks.Add(new Stock
                {
                    Id = Guid.NewGuid(),
                    Price = stockPrice.Price,
                    Ticker = stockPrice.Ticker
                });
            }

            await _portfolioDbContext.BulkMergeAsync(stocks, options =>
            {
                options.ColumnPrimaryKeyExpression = x => new { x.Ticker };
                options.IgnoreOnMergeUpdateExpression = x => new { x.Id };
            });
        }
    }
}
