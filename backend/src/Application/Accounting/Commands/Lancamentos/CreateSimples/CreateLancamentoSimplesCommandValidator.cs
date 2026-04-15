namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateSimples;

using FluentValidation;

public class CreateLancamentoSimplesCommandValidator : AbstractValidator<CreateLancamentoSimplesCommand>
{
    public CreateLancamentoSimplesCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.DataCompetencia)
            .Must(data => data.Year >= 1900 && data.Year <= 2100)
            .WithMessage("Data de competência deve estar entre 1900 e 2100");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria é obrigatória");

        RuleFor(x => x.NaturezaLancamento)
            .NotEmpty().WithMessage("Natureza do lançamento é obrigatória")
            .Must(n => new[] { "RECEITA", "DESPESA", "TRANSFERENCIA" }.Contains(n))
            .WithMessage("Natureza do lançamento inválida");

        RuleFor(x => x.StatusPagamento)
            .NotEmpty().WithMessage("Status de pagamento é obrigatório")
            .Must(s => new[] { "PREVISTO", "REALIZADO", "ATRASADO" }.Contains(s))
            .WithMessage("Status de pagamento inválido");

        RuleFor(x => x)
            .Custom((x, context) =>
            {
                var ambosNull = !x.ContaId.HasValue && !x.CartaoId.HasValue;
                var ambosPresentes = x.ContaId.HasValue && x.CartaoId.HasValue;

                if (x.StatusPagamento == "REALIZADO")
                {
                    if (ambosNull || ambosPresentes)
                        context.AddFailure("Lançamento realizado deve ter conta OU cartão, mas não ambos ou nenhum");
                }
                else if (ambosPresentes)
                {
                    context.AddFailure("Lançamento não pode ter conta e cartão simultaneamente");
                }
            });

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Máximo de 10 tags permitidas");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória");
    }
}
