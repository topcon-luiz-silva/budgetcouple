namespace BudgetCouple.Application.Auth.Commands.Login;

using FluentValidation;

/// <summary>
/// Validator for LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Pin)
            .NotEmpty().WithMessage("PIN é obrigatório");
    }
}
