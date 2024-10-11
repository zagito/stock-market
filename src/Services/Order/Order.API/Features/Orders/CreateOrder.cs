using Carter;
using MassTransit;
using MediatR;
using MessageBroker.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Contracts.Orders;
using Order.API.Data;
using Order.API.Data.Entities;
using Price.Grpc;
using Shared.Results;
using Shared.SQRS;

namespace Order.API.Features.Orders
{
    public static class CreateOrder
    {
        public record Command (string Ticker, int Quantity, Side Side) : ICommand<decimal> {}

        internal sealed class CommandHandler : ICommandHandler<Command, decimal>
        {
            private readonly IPublishEndpoint _publishEndpoint;
            private readonly StockPriceProtoService.StockPriceProtoServiceClient _stockPriceProtoServiceClient;
            private readonly OrderDbContext _orderDbContext;

            public CommandHandler(IPublishEndpoint publishEndpoint, StockPriceProtoService.StockPriceProtoServiceClient stockPriceProtoServiceClient, OrderDbContext orderDbContext)
            {
                _publishEndpoint = publishEndpoint;
                _stockPriceProtoServiceClient = stockPriceProtoServiceClient;
                _orderDbContext = orderDbContext;
            }

            public async Task<Result<decimal>> Handle(Command request, CancellationToken cancellationToken)
            {

                var result = await _stockPriceProtoServiceClient.GetStockPriceAsync(new GettStockPriceRequest() { Ticker = request.Ticker }, cancellationToken: cancellationToken);

                if (result.Price <= 0) 
                {
                    return Result<decimal>.Failure(new Error("Error.NoStock", $"No stocks with ticker {request.Ticker} are selling"));
                }

                var orderId = Guid.NewGuid();

                _orderDbContext.Orders.Add(new Data.Entities.Order
                {
                    Ticker = request.Ticker,
                    Id = orderId,
                    Quantity = request.Quantity,
                    Side = request.Side,
                    Status = OrderStatus.Created,
                    StatusNormalized = OrderStatus.Created.ToString()

                });

                await _orderDbContext.SaveChangesAsync();

                await _publishEndpoint.Publish(
                new OrderCreatedEvent
                {
                    Id = orderId
                },
                cancellationToken);

                return Result<decimal>.Success(result.Price);
            }
        }
    }

    public class CreateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/order/add/{userId}", async (Guid userId, [FromBody] CreateOrderRequest request, ISender sender) =>
            {
                var command = new CreateOrder.Command(request.Ticker, request.Quantity, request.Side);

                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);
            });
        }
    }

}
