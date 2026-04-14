namespace BudgetCouple.Infrastructure.Repositories;

using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class MetaRepository : IMetaRepository
{
    private readonly AppDbContext _dbContext;

    public MetaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Meta meta, CancellationToken cancellationToken = default)
    {
        await _dbContext.Metas.AddAsync(meta, cancellationToken);
    }

    public async Task<Meta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Metas.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<List<Meta>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Metas
            .Where(m => m.Ativa)
            .OrderByDescending(m => m.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Meta meta)
    {
        _dbContext.Metas.Remove(meta);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
