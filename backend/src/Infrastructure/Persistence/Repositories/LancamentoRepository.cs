namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using Microsoft.EntityFrameworkCore;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly AppDbContext _dbContext;

    public LancamentoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Lancamentos.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<Lancamento>> ListAsync(
        DateOnly? dataInicio = null,
        DateOnly? dataFim = null,
        Guid? contaId = null,
        Guid? cartaoId = null,
        Guid? categoriaId = null,
        string? status = null,
        string? tipo = null,
        string? naturezaLancamento = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Lancamentos.AsQueryable();

        if (dataInicio.HasValue)
            query = query.Where(l => l.Data >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(l => l.Data <= dataFim.Value);

        if (contaId.HasValue)
            query = query.Where(l => l.ContaId == contaId.Value);

        if (cartaoId.HasValue)
            query = query.Where(l => l.CartaoId == cartaoId.Value);

        if (categoriaId.HasValue)
            query = query.Where(l => l.CategoriaId == categoriaId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<StatusPagamento>(status, out var statusEnum))
                query = query.Where(l => l.StatusPagamento == statusEnum);
        }

        if (!string.IsNullOrEmpty(tipo))
        {
            if (Enum.TryParse<TipoLancamento>(tipo, out var tipoEnum))
                query = query.Where(l => l.Tipo == tipoEnum);
        }

        if (!string.IsNullOrEmpty(naturezaLancamento))
        {
            if (Enum.TryParse<NaturezaLancamento>(naturezaLancamento, out var naturezaEnum))
                query = query.Where(l => l.Natureza == naturezaEnum);
        }

        return await query
            .OrderByDescending(l => l.Data)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        DateOnly? dataInicio = null,
        DateOnly? dataFim = null,
        Guid? contaId = null,
        Guid? cartaoId = null,
        Guid? categoriaId = null,
        string? status = null,
        string? tipo = null,
        string? naturezaLancamento = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Lancamentos.AsQueryable();

        if (dataInicio.HasValue)
            query = query.Where(l => l.Data >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(l => l.Data <= dataFim.Value);

        if (contaId.HasValue)
            query = query.Where(l => l.ContaId == contaId.Value);

        if (cartaoId.HasValue)
            query = query.Where(l => l.CartaoId == cartaoId.Value);

        if (categoriaId.HasValue)
            query = query.Where(l => l.CategoriaId == categoriaId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<StatusPagamento>(status, out var statusEnum))
                query = query.Where(l => l.StatusPagamento == statusEnum);
        }

        if (!string.IsNullOrEmpty(tipo))
        {
            if (Enum.TryParse<TipoLancamento>(tipo, out var tipoEnum))
                query = query.Where(l => l.Tipo == tipoEnum);
        }

        if (!string.IsNullOrEmpty(naturezaLancamento))
        {
            if (Enum.TryParse<NaturezaLancamento>(naturezaLancamento, out var naturezaEnum))
                query = query.Where(l => l.Natureza == naturezaEnum);
        }

        return await query.CountAsync(cancellationToken);
    }

    public void Add(Lancamento lancamento)
    {
        _dbContext.Lancamentos.Add(lancamento);
    }

    public void AddRange(IEnumerable<Lancamento> lancamentos)
    {
        _dbContext.Lancamentos.AddRange(lancamentos);
    }

    public void Update(Lancamento lancamento)
    {
        _dbContext.Lancamentos.Update(lancamento);
    }

    public void Delete(Lancamento lancamento)
    {
        _dbContext.Lancamentos.Remove(lancamento);
    }

    public async Task<List<Lancamento>> GetByRecorrenciaIdAsync(Guid recorrenciaId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Lancamentos
            .Where(l => l.RecorrenciaId == recorrenciaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Lancamento>> GetByParcelacaoAsync(Guid lancamentoPaiId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Lancamentos
            .Where(l => (l.Id == lancamentoPaiId || l.DadosParcelamento!.LancamentoPaiId == lancamentoPaiId))
            .ToListAsync(cancellationToken);
    }
}
