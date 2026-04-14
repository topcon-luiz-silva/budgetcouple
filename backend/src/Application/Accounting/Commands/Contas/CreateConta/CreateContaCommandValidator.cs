namespace BudgetCouple.Application.Accounting.Commands.Contas.CreateConta;

using FluentValidation;

public class CreateContaCommandValidator : AbstractValidator<CreateContaCommand>
{
    public CreateContaCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome da conta é obrigatório");

        RuleFor(x => x.TipoConta)
            .NotEmpty().WithMessage("Tipo de conta é obrigatório")
            .Must(tipo => new[] { "CONTA_CORRENTE", "POUPANCA", "CARTEIRA", "INVESTIMENTO" }.Contains(tipo))
            .WithMessage("Tipo de conta inválido");

        RuleFor(x => x.SaldoInicial)
            .GreaterThanOrEqualTo(0).WithMessage("Saldo inicial não pode ser negativo");

        RuleFor(x => x.CorHex)
            .NotEmpty().WithMessage("Cor é obrigatória");
    }
}
