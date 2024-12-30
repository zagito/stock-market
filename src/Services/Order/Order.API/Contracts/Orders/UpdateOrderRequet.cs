using Order.API.Data.Entities;

namespace Order.API.Contracts.Orders
{
    public record UpdateOrderRequet(string Ticker, int Quantity, Side Side, Guid UserId);
}
