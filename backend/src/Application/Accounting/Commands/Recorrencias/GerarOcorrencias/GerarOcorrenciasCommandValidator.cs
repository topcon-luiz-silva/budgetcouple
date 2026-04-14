namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.GerarOcorrencias;

using FluentValidation;

public class GerarOcorrenciasCommandValidator : AbstractValidator<GerarOcorrenciasCommand>
{
    public GerarOcorrenciasCommandValidator()
    {
        RuleFor(x => x.RecorrenciaId)
            .NotEmpty().WithMessage("ID da recorrência é obrigatório");

        RuleFor(x => x.Ate)
            .Must(data => data.Year >= 1900 && data.Year <= 2100)
            .WithMessage("Data deve estar entre 1900 e 2100");
    }
}
