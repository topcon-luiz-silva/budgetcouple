namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Cartoes;
using Microsoft.EntityFrameworkCore;

public class CartaoRepository : ICartaoRepository
{
    private readonly AppDbContext _dbContext;

    public CartaoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cartao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cartoes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Cartao>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cartoes.ToListAsync(cancellationToken);
    }

    public void Add(Cartao cartao)
    {
        _dbContext.Cartoes.Add(cartao);
    }

    public void Update(Cartao cartao)
    {
        _dbContext.Cartoes.Update(cartao);
    }

    public void Delete(Cartao cartao)
    {
        _dbContext.Cartoes.Remove(cartao);
    }
}
