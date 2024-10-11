using MassTransit;
using MassTransit.Transports;
using MessageBroker.Events;
using Microsoft.EntityFrameworkCore;
using Portfolio.API.Data;
using Portfolio.API.Data.Entities;
using Shared.Exceptions;
using System.Threading;

namespace Portfolio.API.Features.Orders
{
    public class OrderCreated : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreated> _logger;
        private readonly PortfolioDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreated(ILogger<OrderCreated> logger, PortfolioDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var order = context.Message;

            if (order == null) 
            {
                _logger.LogError("Empty order recieved at date: {date}", DateTime.UtcNow);
                return;
            }

            var userPortfolio = await _dbContext.Portfolios
                .Include(x => x.PortfolioStocks.Where(ps => ps.Stock.Ticker == order.Ticker))
                .FirstOrDefaultAsync(p => p.UserId == order.UserId);

            bool isNewUser = false;
            if (userPortfolio == null) 
            {
                userPortfolio = GenerateNewUserPortfolio(order.UserId);
                isNewUser = true;
            }


            //IF isNewUser and is sell -> error no stocks for sell - just create the Portfolio
            //IF is sell and is not a new user -> validate do user has enough quantity and sell.
            //IF is buy -> validate do user has enough cash


            var reslut = order.IsSell ? await PerformSellOperation(userPortfolio, order, isNewUser)
                        : await PerformBuyOperation(userPortfolio, order, isNewUser);

            await _publishEndpoint.Publish(
                    new OrderExecutedEvent(order.OrderId, DateTime.UtcNow, reslut.IsSuccess, reslut.ErrorReason ?? string.Empty));
        }

        

        private async Task<Result> PerformSellOperation(Data.Entities.Portfolio userPortfolio, OrderCreatedEvent order, bool isNewUser) 
        {
            if (isNewUser) 
            {
                _dbContext.Add(userPortfolio);
                await _dbContext.SaveChangesAsync();
                return new Result(false, "Insufficient stock quantity");
            }

            var portfolioStock = userPortfolio.PortfolioStocks.SingleOrDefault();
            if(portfolioStock == null || portfolioStock.Quantity < order.Quantity)
                return new Result(false, "Insufficient stock quantity");

            //Let's delete this portfolio stock if empty
            if (portfolioStock.Quantity == order.Quantity)
            {
                _dbContext.PortfolioStocks.Remove(portfolioStock);
            }
            else 
            {
                portfolioStock.Quantity -= order.Quantity;
            }

            userPortfolio.Cash += order.Quantity * order.Price;
            await _dbContext.SaveChangesAsync();

            return new Result(true, null);
        }

        private async Task<Result> PerformBuyOperation(Data.Entities.Portfolio userPortfolio, OrderCreatedEvent order, bool isNewUser)
        {
            decimal totalPrice = order.Quantity * order.Price;

            if (userPortfolio.Cash < totalPrice)
            {
                if (isNewUser)
                {
                    _dbContext.Add(userPortfolio);
                    await _dbContext.SaveChangesAsync();
                }
                return new Result(false, "Insufficient cach in portfolio");
            }

            var stock = await _dbContext.Stocks.FirstOrDefaultAsync(st => st.Ticker == order.Ticker);
            if (stock == null)
                throw new StockNotSyncedEcpetion(order.Ticker);

            AppPortfolioStock(userPortfolio, order, isNewUser, stock);

            userPortfolio.Cash -= totalPrice;

            if (isNewUser)
            {
                _dbContext.Portfolios.Add(userPortfolio);
            }

            await _dbContext.SaveChangesAsync();

            return new Result(true, null);
        }

        private void AppPortfolioStock(Data.Entities.Portfolio userPortfolio, OrderCreatedEvent order, bool isNewUser, Stock stock)
        {
            var portfolioStock = userPortfolio.PortfolioStocks.SingleOrDefault();
            if (portfolioStock == null)
            {
                if (isNewUser)
                {
                    userPortfolio.AddStock(order.Quantity, stock.Id);
                }
                else
                {
                    _dbContext.PortfolioStocks.Add(new Data.Entities.PortfolioStock
                    {
                        Id = Guid.NewGuid(),
                        PortfolioId = userPortfolio.Id,
                        Quantity = order.Quantity,
                        StockId = stock.Id
                    });

                }
            }
            else
            {
                portfolioStock.Quantity += order.Quantity;
            }
        }

        private Data.Entities.Portfolio GenerateNewUserPortfolio(Guid userId)
        {
            return new Data.Entities.Portfolio
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Cash = 1000
            };
        }
    }

    public record Result(bool IsSuccess, string? ErrorReason);
}
