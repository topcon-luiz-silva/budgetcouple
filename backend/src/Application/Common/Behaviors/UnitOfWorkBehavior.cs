namespace BudgetCouple.Application.Common.Behaviors;

using MediatR;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;

/// <summary>
/// Wraps commands in a transaction.
/// Only applies to requests that are IRequest (commands), not IRequest<Result<T>>.
/// </summary>
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only wrap in transaction if response is a Result type
        var isResultResponse = typeof(TResponse) == typeof(Result) ||
                               (typeof(TResponse).IsGenericType &&
                                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>));

        if (!isResultResponse)
        {
            return await next();
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            // Check if response indicates failure
            var isFailure = false;
            if (response is Result result)
            {
                isFailure = result.IsFailure;
            }
            else if (response != null)
            {
                var failureProperty = typeof(TResponse).GetProperty("IsFailure");
                if (failureProperty != null)
                {
                    isFailure = (bool)(failureProperty.GetValue(response) ?? false);
                }
            }

            if (isFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            else
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
