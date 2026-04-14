namespace BudgetCouple.Application.Accounting.Commands.Contas.UpdateConta;

using FluentValidation;

public class UpdateContaCommandValidator : AbstractValidator<UpdateContaCommand>
{
    public UpdateContaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID da conta é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome da conta é obrigatório");

        RuleFor(x => x.CorHex)
            .NotEmpty().WithMessage("Cor é obrigatória");
    }
}
