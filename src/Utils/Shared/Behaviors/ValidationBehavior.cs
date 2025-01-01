using FluentValidation;
using MediatR;
using Shared.Results;
using Shared.SQRS;

namespace Shared.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    //where TResponse : Result<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults =
                await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            Error[] errors = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .Select(failure => new Error(
                    failure.PropertyName,
                    failure.ErrorMessage))
                .Distinct()
                .ToArray();

            if (errors.Any())
                throw new ValidationException("Ala bala");
               //return Create<TResponse>(errors);

            return await next();
        }


        private static TResult Create<TResult>(Error[] errors)
            where TResult : Result<TResult>
        {
            return (errors.First() as TResult)!;
        }
    }
}
