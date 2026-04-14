namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Contas;
using Microsoft.EntityFrameworkCore;

public class ContaRepository : IContaRepository
{
    private readonly AppDbContext _dbContext;

    public ContaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Conta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Contas.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Conta>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Contas.ToListAsync(cancellationToken);
    }

    public void Add(Conta conta)
    {
        _dbContext.Contas.Add(conta);
    }

    public void Update(Conta conta)
    {
        _dbContext.Contas.Update(conta);
    }

    public void Delete(Conta conta)
    {
        _dbContext.Contas.Remove(conta);
    }
}
