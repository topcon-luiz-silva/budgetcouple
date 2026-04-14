namespace BudgetCouple.Application.Accounting.Commands.Cartoes.UpdateCartao;

using FluentValidation;

public class UpdateCartaoCommandValidator : AbstractValidator<UpdateCartaoCommand>
{
    public UpdateCartaoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID do cartão é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do cartão é obrigatório");

        RuleFor(x => x.Bandeira)
            .NotEmpty().WithMessage("Bandeira é obrigatória");

        RuleFor(x => x.DiaFechamento)
            .InclusiveBetween(1, 31).WithMessage("Dia de fechamento deve estar entre 1 e 31");

        RuleFor(x => x.DiaVencimento)
            .InclusiveBetween(1, 31).WithMessage("Dia de vencimento deve estar entre 1 e 31");

        RuleFor(x => x.Limite)
            .GreaterThan(0).WithMessage("Limite deve ser maior que 0");

        RuleFor(x => x.ContaPagamentoId)
            .NotEmpty().WithMessage("Conta de pagamento é obrigatória");

        RuleFor(x => x.CorHex)
            .NotEmpty().WithMessage("Cor é obrigatória");
    }
}
