using Carter;
using MassTransit;
using MediatR;
using MessageBroker.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Contracts.Orders;
using Order.API.Data;
using Price.Grpc;
using Shared.Results;
using Shared.SQRS;

namespace Order.API.Features.Orders
{
    public static class CreateOrder
    {
        public record Command (string Ticker, int Quantity, Side Side) : ICommand<Guid> {}

        internal sealed class CommandHandler : ICommandHandler<Command, Guid>
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

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {

                var result = await _stockPriceProtoServiceClient.GetStockPriceAsync(new GettStockPriceRequest() { Ticker = request.Ticker }, cancellationToken: cancellationToken);

                var orderId = Guid.NewGuid();

                await _publishEndpoint.Publish(
                new OrderCreatedEvent
                {
                    Id = orderId
                },
                cancellationToken);

                return Result<Guid>.Success(Guid.NewGuid());
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
