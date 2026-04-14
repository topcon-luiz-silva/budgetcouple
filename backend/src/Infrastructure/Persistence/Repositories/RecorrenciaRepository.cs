namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Recorrencias;
using Microsoft.EntityFrameworkCore;

public class RecorrenciaRepository : IRecorrenciaRepository
{
    private readonly AppDbContext _dbContext;

    public RecorrenciaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Recorrencia?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Recorrencias.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Recorrencia>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Recorrencias.ToListAsync(cancellationToken);
    }

    public void Add(Recorrencia recorrencia)
    {
        _dbContext.Recorrencias.Add(recorrencia);
    }

    public void Update(Recorrencia recorrencia)
    {
        _dbContext.Recorrencias.Update(recorrencia);
    }

    public void Delete(Recorrencia recorrencia)
    {
        _dbContext.Recorrencias.Remove(recorrencia);
    }
}
