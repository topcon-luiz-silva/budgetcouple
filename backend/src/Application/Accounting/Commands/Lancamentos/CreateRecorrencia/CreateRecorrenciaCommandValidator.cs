namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateRecorrencia;

using FluentValidation;

public class CreateRecorrenciaCommandValidator : AbstractValidator<CreateRecorrenciaCommand>
{
    public CreateRecorrenciaCommandValidator()
    {
        RuleFor(x => x.ValorBase)
            .GreaterThan(0).WithMessage("Valor base deve ser maior que zero");

        RuleFor(x => x.Frequencia)
            .NotEmpty().WithMessage("Frequência é obrigatória")
            .Must(f => new[] { "DIARIA", "SEMANAL", "QUINZENAL", "MENSAL", "BIMESTRAL", "TRIMESTRAL", "SEMESTRAL", "ANUAL" }.Contains(f))
            .WithMessage("Frequência inválida");

        RuleFor(x => x.DataInicio)
            .Must(data => data.Year >= 1900 && data.Year <= 2100)
            .WithMessage("Data de início deve estar entre 1900 e 2100");

        RuleFor(x => x)
            .Custom((x, context) =>
            {
                if (x.DataFim.HasValue && x.DataFim.Value <= x.DataInicio)
                    context.AddFailure("Data fim deve ser após data início");
            });

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria é obrigatória");

        RuleFor(x => x.NaturezaLancamento)
            .NotEmpty().WithMessage("Natureza do lançamento é obrigatória")
            .Must(n => new[] { "RECEITA", "DESPESA", "TRANSFERENCIA" }.Contains(n))
            .WithMessage("Natureza do lançamento inválida");

        RuleFor(x => x)
            .Custom((x, context) =>
            {
                var ambosPresentes = x.ContaId.HasValue && x.CartaoId.HasValue;

                if (ambosPresentes)
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
