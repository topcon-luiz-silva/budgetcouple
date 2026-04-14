namespace BudgetCouple.Application.Auth.Queries.GetAuthStatus;

using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using MediatR;

/// <summary>
/// Handler for GetAuthStatusQuery.
/// Returns current PIN configuration and lock status.
/// </summary>
public class GetAuthStatusQueryHandler : IRequestHandler<GetAuthStatusQuery, AuthStatusDto>
{
    private readonly IAppConfigRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetAuthStatusQueryHandler(
        IAppConfigRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthStatusDto> Handle(GetAuthStatusQuery request, CancellationToken cancellationToken)
    {
        var appConfig = await _repository.GetSingleAsync(cancellationToken);

        if (appConfig == null)
        {
            return new AuthStatusDto(PinConfigured: false, Locked: false, LockedUntil: null);
        }

        var pinConfigured = !string.IsNullOrEmpty(appConfig.PinHash);
        var now = _dateTimeProvider.UtcNow;
        var locked = appConfig.LockedUntil.HasValue && appConfig.LockedUntil > now;

        return new AuthStatusDto(
            PinConfigured: pinConfigured,
            Locked: locked,
            LockedUntil: appConfig.LockedUntil);
    }
}
