namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.Delete;

using FluentValidation;

public class DeleteRecorrenciaCommandValidator : AbstractValidator<DeleteRecorrenciaCommand>
{
    public DeleteRecorrenciaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");
    }
}
