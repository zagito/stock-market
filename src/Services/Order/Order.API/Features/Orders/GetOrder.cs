using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.API.Contracts.Orders;
using Order.API.Data;
using Shared.Results;
using Shared.SQRS;

namespace Order.API.Features.Orders
{
    public static class GetOrder
    {
        public record Query(Guid OrderId) : IQuery<OrderResponse> { }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(c => c.OrderId).NotEmpty();
            }
        }

        internal sealed class QueryHandler : IQueryHandler<Query, OrderResponse>
        {
            private readonly OrderDbContext _orderDbContext;
            private readonly IValidator<Query> _validator;

            public QueryHandler(OrderDbContext orderDbContext, IValidator<Query> validator)
            {
                _orderDbContext = orderDbContext;
                _validator = validator;
            }

            public async Task<Result<OrderResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result<OrderResponse>.Failure(new Error(
                        "GetOrder.Validation",
                        validationResult.ToString()));
                }

                var order = await GetOrderById(request.OrderId, cancellationToken);
                if (order == null)
                {
                    return Result<OrderResponse>.Failure(new Error(
                        "Error.OrderNotFound",
                        $"Order with Id: {request.OrderId} not found"));
                }

                return Result<OrderResponse>.Success(MapToOrderResponse(order));
            }

            private async Task<Data.Entities.Order?> GetOrderById(Guid orderId, CancellationToken cancellationToken)
            {
                return await _orderDbContext.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
            }

            private OrderResponse MapToOrderResponse(Data.Entities.Order order)
            {
                return new OrderResponse
                (
                    order.Id,
                    order.Ticker,
                    order.Quantity,
                    order.Side,
                    order.UserId,
                    order.CreateDate,
                    order.ExecuteDate,
                    order.FailureReason ?? string.Empty,
                    order.IsSuccessful ?? false
                );
            }
        }

        public class GetOrderEndpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapGet("/api/orders/{orderId}", async (Guid orderId, ISender sender) =>
                {
                    var query = new Query(orderId);
                    var result = await sender.Send(query);
                    if (result.IsSuccess)
                    {
                        return Results.Ok(result.Value);
                    }
                    return Results.NotFound(result.Error);
                });
            }
        }
    }
}
