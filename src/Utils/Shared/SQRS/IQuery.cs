using MediatR;
using Shared.Results;

namespace Shared.SQRS
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
