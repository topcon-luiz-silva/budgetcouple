namespace BudgetCouple.Application.Auth.Commands.SetupPin;

using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Identity;
using MediatR;

/// <summary>
/// Handler for SetupPinCommand.
/// Sets up the initial PIN, generates JWT, and returns auth result.
/// </summary>
public class SetupPinCommandHandler : IRequestHandler<SetupPinCommand, Result<AuthResult>>
{
    private readonly IAppConfigRepository _repository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPinHasher _pinHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SetupPinCommandHandler(
        IAppConfigRepository repository,
        IApplicationDbContext dbContext,
        IPinHasher pinHasher,
        IJwtTokenService jwtTokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dbContext = dbContext;
        _pinHasher = pinHasher;
        _jwtTokenService = jwtTokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<AuthResult>> Handle(SetupPinCommand request, CancellationToken cancellationToken)
    {
        // Load or create AppConfig
        var appConfig = await _repository.GetSingleAsync(cancellationToken);

        if (appConfig == null)
        {
            appConfig = AppConfig.Empty();
            _repository.Add(appConfig);
        }

        // Try to configure PIN
        var hash = _pinHasher.Hash(request.Pin);
        var configResult = appConfig.ConfigurarPin(hash, _dateTimeProvider.UtcNow);

        if (!configResult.IsSuccess)
        {
            return Result.Failure<AuthResult>(configResult.Error);
        }

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Generate JWT
        var token = _jwtTokenService.GenerateToken();
        var expiresAt = _dateTimeProvider.UtcNow.AddDays(30);

        return Result.Success(new AuthResult(token, expiresAt));
    }
}
