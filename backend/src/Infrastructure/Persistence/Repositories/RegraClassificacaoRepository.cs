namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Imports;
using Microsoft.EntityFrameworkCore;

public class RegraClassificacaoRepository : IRegraClassificacaoRepository
{
    private readonly AppDbContext _context;

    public RegraClassificacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegraClassificacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RegraClassificacao>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<RegraClassificacao>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<RegraClassificacao>()
            .OrderByDescending(r => r.Prioridade)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RegraClassificacao>> ListAtivasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<RegraClassificacao>()
            .Where(r => r.Ativa)
            .OrderByDescending(r => r.Prioridade)
            .ToListAsync(cancellationToken);
    }

    public void Add(RegraClassificacao regra)
    {
        _context.Add(regra);
    }

    public void Update(RegraClassificacao regra)
    {
        _context.Update(regra);
    }

    public void Delete(RegraClassificacao regra)
    {
        _context.Remove(regra);
    }
}
