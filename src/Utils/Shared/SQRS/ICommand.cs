using MediatR;
using Shared.Results;

namespace Shared.SQRS
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
