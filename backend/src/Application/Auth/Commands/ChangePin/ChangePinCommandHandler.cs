namespace BudgetCouple.Application.Auth.Commands.ChangePin;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Handler for ChangePinCommand.
/// Verifies current PIN and updates to new PIN.
/// </summary>
public class ChangePinCommandHandler : IRequestHandler<ChangePinCommand, Result>
{
    private readonly IAppConfigRepository _repository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPinHasher _pinHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChangePinCommandHandler(
        IAppConfigRepository repository,
        IApplicationDbContext dbContext,
        IPinHasher pinHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dbContext = dbContext;
        _pinHasher = pinHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(ChangePinCommand request, CancellationToken cancellationToken)
    {
        // Load AppConfig
        var appConfig = await _repository.GetSingleAsync(cancellationToken);

        if (appConfig == null)
        {
            return Result.Failure(Error.NotFound("Configuração não encontrada"));
        }

        // Verify current PIN
        var pinMatches = _pinHasher.Verify(request.PinAtual, appConfig.PinHash);

        if (!pinMatches)
        {
            return Result.Failure(Error.Unauthorized("PIN atual incorreto"));
        }

        // Hash new PIN and update
        var newHash = _pinHasher.Hash(request.NovoPin);
        appConfig.TrocarPin(newHash, _dateTimeProvider.UtcNow);

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
