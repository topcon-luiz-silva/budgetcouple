using FluentValidation;

namespace BudgetCouple.Application.Accounting.Commands.Faturas.PagarFatura;

public class PagarFaturaCommandValidator : AbstractValidator<PagarFaturaCommand>
{
    public PagarFaturaCommandValidator()
    {
        RuleFor(x => x.CartaoId)
            .NotEmpty().WithMessage("CartaoId é obrigatório.");

        RuleFor(x => x.Competencia)
            .NotEmpty().WithMessage("Competência é obrigatória.")
            .Matches(@"^\d{4}-\d{2}$").WithMessage("Competência deve estar no formato YYYY-MM.");
    }
}
