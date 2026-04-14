namespace BudgetCouple.Infrastructure.Services.Import;

using BudgetCouple.Application.Import.Interfaces;
using BudgetCouple.Domain.Imports;
using BudgetCouple.Infrastructure.Persistence;

/// <summary>
/// Engine for automatic classification of imported transactions using rules and history.
/// </summary>
public class ClassificationEngine : IClassificationEngine
{
    private readonly AppDbContext _context;

    public ClassificationEngine(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(Guid? categoriaId, Guid? subcategoriaId)?> ClassifyAsync(string descricao)
    {
        if (string.IsNullOrEmpty(descricao))
            return null;

        // Get all active rules ordered by priority (DESC - higher priority first)
        var regras = _context.Set<RegraClassificacao>()
            .Where(r => r.Ativa)
            .OrderByDescending(r => r.Prioridade)
            .ToList();

        // Apply rules in priority order
        foreach (var regra in regras)
        {
            if (regra.Corresponde(descricao))
            {
                return (regra.CategoriaId, regra.SubcategoriaId);
            }
        }

        // Fallback: Try to find similar past transactions and return their category
        // Use simple string contains match on past lancamentos
        var categoriaSimilar = _context.Set<BudgetCouple.Domain.Accounting.Lancamentos.Lancamento>()
            .Where(l => l.Descricao != null && l.Descricao.Contains(descricao, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.CriadoEm)
            .FirstOrDefault();

        if (categoriaSimilar != null)
        {
            return (categoriaSimilar.CategoriaId, categoriaSimilar.SubcategoriaId);
        }

        return null;
    }
}
