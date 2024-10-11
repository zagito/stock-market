using MassTransit;
using MessageBroker.Events;
using Microsoft.EntityFrameworkCore;
using Order.API.Data;
using Shared.Exceptions;

namespace Order.API.Features.Orders
{
    public class OrderExecuted : IConsumer<OrderExecutedEvent>
    {
        private readonly OrderDbContext _dbContext;
        private readonly ILogger<OrderExecuted> _logger;

        public OrderExecuted(OrderDbContext dbContext, ILogger<OrderExecuted> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderExecutedEvent> context)
        {
            var orderResult = context.Message;

            if (orderResult == null)
            {
                _logger.LogError("Empty order execution result recieved, date {date}", DateTime.UtcNow);
                return;
            }

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderResult.OrderId);

            if (order == null)
                throw new OrderExecutionException($"Order with Id: {order} is missing int the database but an execution is recieved");

            order.ExecuteDate = orderResult.ExecutedDate;
            order.FailureReason = orderResult.FailureReason;
            order.IsSuccessful = orderResult.IsSuccess;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Order with Id {} was completed", order.Id);

        }
    }
}
