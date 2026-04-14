namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Update;

using FluentValidation;

public class UpdateLancamentoCommandValidator : AbstractValidator<UpdateLancamentoCommand>
{
    public UpdateLancamentoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.DataCompetencia)
            .Must(data => data.Year >= 1900 && data.Year <= 2100)
            .WithMessage("Data de competência deve estar entre 1900 e 2100");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria é obrigatória");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Máximo de 10 tags permitidas");
    }
}
