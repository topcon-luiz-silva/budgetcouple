namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.List;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record ListLancamentosQuery(
    DateOnly? DataInicio = null,
    DateOnly? DataFim = null,
    Guid? ContaId = null,
    Guid? CartaoId = null,
    Guid? CategoriaId = null,
    string? Status = null,
    string? Tipo = null,
    string? NaturezaLancamento = null,
    int Skip = 0,
    int Take = 50) : IRequest<Result<ListaLancamentosResponse>>;
