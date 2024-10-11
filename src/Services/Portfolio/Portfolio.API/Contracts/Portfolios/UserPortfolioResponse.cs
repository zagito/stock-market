namespace Portfolio.API.Contracts.Portfolios
{
    public record UserPortfolioResponse(Guid UserId, Guid PortfolioId, decimal CashMoney, decimal StockMoney);
}
