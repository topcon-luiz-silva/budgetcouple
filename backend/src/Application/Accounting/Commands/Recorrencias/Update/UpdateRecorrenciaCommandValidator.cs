namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.Update;

using FluentValidation;

public class UpdateRecorrenciaCommandValidator : AbstractValidator<UpdateRecorrenciaCommand>
{
    public UpdateRecorrenciaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");
    }
}
