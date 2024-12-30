using Order.API.Data.Entities;

namespace Order.API.Contracts.Orders
{
    public record OrderResponse
    (
        Guid Id,
        string Ticker,
        int Quantity,
        Side Side,
        Guid UserId,
        DateTime CreatedDate,
        DateTime? ExecuteDate,
        string FailureReason,
        bool IsSuccessful
    );
    
}
