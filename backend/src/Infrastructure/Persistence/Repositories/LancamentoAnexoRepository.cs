namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Domain.Accounting.Lancamentos;
using Microsoft.EntityFrameworkCore;
using BudgetCouple.Application.Common.Interfaces.Accounting;

public class LancamentoAnexoRepository : ILancamentoAnexoRepository
{
    private readonly AppDbContext _context;

    public LancamentoAnexoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LancamentoAnexo anexo, CancellationToken cancellationToken = default)
    {
        await _context.LancamentoAnexos.AddAsync(anexo, cancellationToken);
    }

    public async Task<LancamentoAnexo?> GetByIdAsync(Guid anexoId, CancellationToken cancellationToken = default)
    {
        return await _context.LancamentoAnexos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == anexoId, cancellationToken);
    }

    public async Task<List<LancamentoAnexo>> GetByLancamentoIdAsync(Guid lancamentoId, CancellationToken cancellationToken = default)
    {
        return await _context.LancamentoAnexos.AsNoTracking()
            .Where(x => x.LancamentoId == lancamentoId)
            .OrderBy(x => x.EnviadoEm)
            .ToListAsync(cancellationToken);
    }

    public void Delete(LancamentoAnexo anexo)
    {
        _context.LancamentoAnexos.Remove(anexo);
    }
}
