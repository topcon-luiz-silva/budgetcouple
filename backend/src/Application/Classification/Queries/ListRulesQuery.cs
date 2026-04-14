namespace BudgetCouple.Application.Classification.Queries;

using BudgetCouple.Domain.Common;
using MediatR;

public class RegraClassificacaoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Padrao { get; set; } = null!;
    public string TipoPadrao { get; set; } = null!;
    public Guid CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public Guid? SubcategoriaId { get; set; }
    public string? SubcategoriaNome { get; set; }
    public int Prioridade { get; set; }
    public bool Ativa { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}

public class ListRulesQuery : IRequest<Result<List<RegraClassificacaoDto>>>
{
    public bool? ApenasAtivas { get; set; }
}
