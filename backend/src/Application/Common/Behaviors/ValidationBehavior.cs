namespace BudgetCouple.Application.Common.Behaviors;

using FluentValidation;
using MediatR;
using BudgetCouple.Domain.Common;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(vr => vr.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            // If response implements Result, return failure
            var firstFailure = failures.First();
            var errorMessage = firstFailure.ErrorMessage;

            // Try to return a Result failure if the response type supports it
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Result<T> case
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethod("Failure", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?
                    .MakeGenericMethod(valueType);

                var error = Error.Validation(string.Join(", ", failures.Select(f => f.ErrorMessage)));
                var result = failureMethod?.Invoke(null, new object[] { error });
                return (TResponse?)result!;
            }
            else if (typeof(TResponse) == typeof(Result))
            {
                // Result case (non-generic)
                var error = Error.Validation(string.Join(", ", failures.Select(f => f.ErrorMessage)));
                var result = Result.Failure(error);
                return (TResponse?)(object?)result!;
            }

            // If no Result support, throw ValidationException
            throw new ValidationException(failures);
        }

        return await next();
    }
}
