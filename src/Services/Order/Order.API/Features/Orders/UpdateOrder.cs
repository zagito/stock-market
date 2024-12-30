using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Contracts.Orders;
using Order.API.Data;
using Order.API.Data.Entities;
using Shared.Results;
using Shared.SQRS;

namespace Order.API.Features.Orders
{
    public static class UpdateOrder
    {
        public record Command(Guid OrderId, string Ticker, int Quantity, Side Side, Guid UserId) : ICommand<Guid> { }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.OrderId).NotEmpty();
                RuleFor(c => c.Ticker).NotEmpty();
                RuleFor(c => c.Quantity).GreaterThan(0);
                RuleFor(c => c.Side).IsInEnum();
            }
        }

        internal sealed class CommandHandler : ICommandHandler<Command, Guid>
        {
            private readonly OrderDbContext _orderDbContext;
            private readonly IValidator<Command> _validator;

            public CommandHandler(OrderDbContext orderDbContext, IValidator<Command> validator)
            {
                _orderDbContext = orderDbContext;
                _validator = validator;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result<Guid>.Failure(new Error(
                        "UpdateOrder.Validation",
                        validationResult.ToString()));
                }

                var order = await _orderDbContext.Orders
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result<Guid>.Failure(new Error("Error.OrderNotFound", $"Order with Id: {request.OrderId} not found"));
                }

                order.Ticker = request.Ticker;
                order.Quantity = request.Quantity;
                order.Side = request.Side;
                order.UserId = request.UserId;

                await _orderDbContext.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(order.Id);
            }
        }
    }

    public class UpdateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/order/{orderId}", async (Guid orderId, [FromBody] UpdateOrderRequet requet, ISender sender) =>
            {
                var command = new UpdateOrder.Command(orderId, requet.Ticker, requet.Quantity, requet.Side, requet.UserId);
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok(result.Value);
            });
        }
    }
}
