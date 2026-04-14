namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Categorias;
using Microsoft.EntityFrameworkCore;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _dbContext;

    public CategoriaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Categoria?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Categoria>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categorias.ToListAsync(cancellationToken);
    }

    public void Add(Categoria categoria)
    {
        _dbContext.Categorias.Add(categoria);
    }

    public void Update(Categoria categoria)
    {
        _dbContext.Categorias.Update(categoria);
    }

    public void Delete(Categoria categoria)
    {
        _dbContext.Categorias.Remove(categoria);
    }
}
