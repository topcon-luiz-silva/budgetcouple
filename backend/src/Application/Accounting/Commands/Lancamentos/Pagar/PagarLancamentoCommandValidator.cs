namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Pagar;

using FluentValidation;

public class PagarLancamentoCommandValidator : AbstractValidator<PagarLancamentoCommand>
{
    public PagarLancamentoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");

        RuleFor(x => x.DataPagamento)
            .Must(data => data.Year >= 1900 && data.Year <= 2100)
            .WithMessage("Data de pagamento deve estar entre 1900 e 2100");
    }
}
