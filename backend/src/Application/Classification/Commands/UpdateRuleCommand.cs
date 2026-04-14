namespace BudgetCouple.Application.Classification.Commands;

using BudgetCouple.Domain.Common;
using MediatR;

public class UpdateRuleCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Padrao { get; set; } = null!;
    public string TipoPadrao { get; set; } = null!;
    public Guid CategoriaId { get; set; }
    public Guid? SubcategoriaId { get; set; }
    public int Prioridade { get; set; } = 100;
    public bool Ativa { get; set; } = true;
}
