namespace BudgetCouple.Application.Dashboard.Queries.ExportLancamentos;

using BudgetCouple.Domain.Common;
using MediatR;

public record ExportLancamentosPdfQuery(
    DateOnly? DataInicio = null,
    DateOnly? DataFim = null,
    Guid? ContaId = null,
    Guid? CartaoId = null,
    Guid? CategoriaId = null) : IRequest<Result<byte[]>>;
