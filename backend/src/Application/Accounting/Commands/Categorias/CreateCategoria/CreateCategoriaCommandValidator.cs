namespace BudgetCouple.Application.Accounting.Commands.Categorias.CreateCategoria;

using FluentValidation;

public class CreateCategoriaCommandValidator : AbstractValidator<CreateCategoriaCommand>
{
    public CreateCategoriaCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome da categoria é obrigatório");

        RuleFor(x => x.TipoCategoria)
            .NotEmpty().WithMessage("Tipo de categoria é obrigatório")
            .Must(tipo => new[] { "DESPESA", "RECEITA" }.Contains(tipo))
            .WithMessage("Tipo de categoria inválido");

        RuleFor(x => x.CorHex)
            .NotEmpty().WithMessage("Cor é obrigatória");
    }
}
