using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Portfolio.API.Contracts.Portfolios;
using Portfolio.API.Data;
using Shared.Results;
using Shared.SQRS;

namespace Portfolio.API.Features.Portfolios
{
    public static class GetUserPortfolio
    {
        public record Query(Guid UserId) : IQuery<UserPortfolioResponse> { }

        public sealed class QueryHandler : IQueryHandler<Query, UserPortfolioResponse>
        {
            public readonly PortfolioDbContext _dbContext;

            public QueryHandler(PortfolioDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<UserPortfolioResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userPortfolio = await _dbContext.Portfolios.Where(p => p.UserId == request.UserId)
                    .Select(p => new
                    {
                        p.UserId,
                        PortfolioId = p.Id,
                        p.Cash,
                        SotckMoney = p.PortfolioStocks.Sum(p => p.Quantity * p.Stock.Price)
                    }).FirstOrDefaultAsync();

                if (userPortfolio == null)
                {
                    return Result<UserPortfolioResponse>.Failure(new Error("User.NotFound", $"User with id: {request.UserId} has no profile"));
                }

                return Result<UserPortfolioResponse>.Success(new UserPortfolioResponse(userPortfolio.UserId, userPortfolio.PortfolioId, userPortfolio.Cash, userPortfolio.SotckMoney));
            }
        }

    }

    public class GetUserPortfolioEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/portfolio/{userId}", async (Guid userId, ISender sender) =>
            {
                var query = new GetUserPortfolio.Query(userId);

                var result = await sender.Send(query);

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok(result.Value);

            });
        }
    }
}
