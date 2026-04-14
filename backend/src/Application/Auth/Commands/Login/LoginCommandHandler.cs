namespace BudgetCouple.Application.Auth.Commands.Login;

using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Handler for LoginCommand.
/// Authenticates with PIN, manages lock-out after 5 failed attempts (15 min lock).
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
{
    private readonly IAppConfigRepository _repository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPinHasher _pinHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
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

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;

        // Load AppConfig
        var appConfig = await _repository.GetSingleAsync(cancellationToken);

        if (appConfig == null)
        {
            return Result.Failure<AuthResult>(
                Error.Unauthorized("PIN não configurado"));
        }

        // Check if PIN is configured
        if (string.IsNullOrEmpty(appConfig.PinHash))
        {
            return Result.Failure<AuthResult>(
                Error.Unauthorized("PIN não configurado"));
        }

        // Check if account is locked
        if (appConfig.LockedUntil.HasValue && appConfig.LockedUntil > now)
        {
            var remainingMinutes = (int)Math.Ceiling((appConfig.LockedUntil.Value - now).TotalMinutes);
            return Result.Failure<AuthResult>(
                Error.Unauthorized($"Acesso bloqueado. Tente novamente em {remainingMinutes} minuto(s)."));
        }

        // Verify PIN
        var pinMatches = _pinHasher.Verify(request.Pin, appConfig.PinHash);

        if (!pinMatches)
        {
            appConfig.RegistrarFalha(now);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var remainingAttempts = appConfig.TentativasRestantes();
            return Result.Failure<AuthResult>(
                Error.Unauthorized($"PIN incorreto. Tentativas restantes: {remainingAttempts}"));
        }

        // Success: reset failures and generate JWT
        appConfig.ResetarFalhas();
        await _dbContext.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken();
        var expiresAt = now.AddDays(30);

        return Result.Success(new AuthResult(token, expiresAt));
    }
}
