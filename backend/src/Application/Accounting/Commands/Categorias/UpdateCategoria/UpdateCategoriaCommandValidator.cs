namespace BudgetCouple.Application.Accounting.Commands.Categorias.UpdateCategoria;

using FluentValidation;

public class UpdateCategoriaCommandValidator : AbstractValidator<UpdateCategoriaCommand>
{
    public UpdateCategoriaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID da categoria é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome da categoria é obrigatório");

        RuleFor(x => x.CorHex)
            .NotEmpty().WithMessage("Cor é obrigatória");
    }
}
