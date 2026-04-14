namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Delete;

using FluentValidation;

public class DeleteLancamentoCommandValidator : AbstractValidator<DeleteLancamentoCommand>
{
    public DeleteLancamentoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");

        RuleFor(x => x.Escopo)
            .Must(e => e == null || new[] { "one", "fromHere", "all" }.Contains(e))
            .WithMessage("Escopo deve ser 'one', 'fromHere' ou 'all'");
    }
}
