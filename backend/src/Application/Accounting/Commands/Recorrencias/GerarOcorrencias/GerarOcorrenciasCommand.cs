namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.GerarOcorrencias;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GerarOcorrenciasCommand(
    Guid RecorrenciaId,
    DateOnly Ate) : IRequest<Result<List<LancamentoDto>>>;
