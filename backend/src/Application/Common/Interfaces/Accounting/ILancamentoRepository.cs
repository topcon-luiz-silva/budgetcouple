namespace BudgetCouple.Application.Common.Interfaces.Accounting;

using BudgetCouple.Domain.Accounting.Lancamentos;

/// <summary>
/// Repository interface for Lancamento aggregate.
/// </summary>
public interface ILancamentoRepository
{
    Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Lancamento>> ListAsync(
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
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        DateOnly? dataInicio = null,
        DateOnly? dataFim = null,
        Guid? contaId = null,
        Guid? cartaoId = null,
        Guid? categoriaId = null,
        string? status = null,
        string? tipo = null,
        string? naturezaLancamento = null,
        CancellationToken cancellationToken = default);

    void Add(Lancamento lancamento);
    void AddRange(IEnumerable<Lancamento> lancamentos);
    void Update(Lancamento lancamento);
    void Delete(Lancamento lancamento);
    Task<List<Lancamento>> GetByRecorrenciaIdAsync(Guid recorrenciaId, CancellationToken cancellationToken = default);
    Task<List<Lancamento>> GetByParcelacaoAsync(Guid lancamentoPaiId, CancellationToken cancellationToken = default);
}
