namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateParcelado;

using FluentValidation;

public class CreateLancamentoParceladoCommandValidator : AbstractValidator<CreateLancamentoParceladoCommand>
{
    public CreateLancamentoParceladoCommandValidator()
    {
        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("Valor total deve ser maior que zero");

        RuleFor(x => x.TotalParcelas)
            .GreaterThanOrEqualTo(2).WithMessage("Total de parcelas deve ser >= 2")
            .LessThanOrEqualTo(99).WithMessage("Total de parcelas deve ser <= 99");

        RuleFor(x => x.DataPrimeiraParcela)
            .Must(data => data.Year >= 1900 && data.Year <= 2100)
            .WithMessage("Data de competência deve estar entre 1900 e 2100");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria é obrigatória");

        RuleFor(x => x.NaturezaLancamento)
            .NotEmpty().WithMessage("Natureza do lançamento é obrigatória")
            .Must(n => new[] { "PREVISTA", "REALIZADA" }.Contains(n))
            .WithMessage("Natureza do lançamento inválida");

        RuleFor(x => x)
            .Custom((x, context) =>
            {
                var ambosNull = !x.ContaId.HasValue && !x.CartaoId.HasValue;
                var ambosPresentes = x.ContaId.HasValue && x.CartaoId.HasValue;

                if (x.NaturezaLancamento == "REALIZADA")
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

        RuleFor(x => x.DescricaoBase)
            .NotEmpty().WithMessage("Descrição é obrigatória");
    }
}
