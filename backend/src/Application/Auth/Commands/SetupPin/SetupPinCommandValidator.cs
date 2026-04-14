namespace BudgetCouple.Application.Auth.Commands.SetupPin;

using FluentValidation;

/// <summary>
/// Validator for SetupPinCommand.
/// PIN must be 6-8 digits.
/// </summary>
public class SetupPinCommandValidator : AbstractValidator<SetupPinCommand>
{
    public SetupPinCommandValidator()
    {
        RuleFor(x => x.Pin)
            .NotEmpty().WithMessage("PIN é obrigatório")
            .Matches(@"^\d{6,8}$").WithMessage("PIN deve conter de 6 a 8 dígitos");
    }
}
