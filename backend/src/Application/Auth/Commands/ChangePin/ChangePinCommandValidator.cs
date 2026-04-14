namespace BudgetCouple.Application.Auth.Commands.ChangePin;

using FluentValidation;

/// <summary>
/// Validator for ChangePinCommand.
/// Both old and new PIN must be 6-8 digits.
/// </summary>
public class ChangePinCommandValidator : AbstractValidator<ChangePinCommand>
{
    public ChangePinCommandValidator()
    {
        RuleFor(x => x.PinAtual)
            .NotEmpty().WithMessage("PIN atual é obrigatório")
            .Matches(@"^\d{6,8}$").WithMessage("PIN atual deve conter de 6 a 8 dígitos");

        RuleFor(x => x.NovoPin)
            .NotEmpty().WithMessage("Novo PIN é obrigatório")
            .Matches(@"^\d{6,8}$").WithMessage("Novo PIN deve conter de 6 a 8 dígitos");

        RuleFor(x => x)
            .Must(x => x.PinAtual != x.NovoPin)
            .WithMessage("Novo PIN deve ser diferente do PIN atual")
            .WithName("");
    }
}
