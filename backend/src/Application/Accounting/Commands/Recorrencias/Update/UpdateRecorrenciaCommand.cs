namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.Update;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record UpdateRecorrenciaCommand(
    Guid Id,
    DateOnly? DataFim) : IRequest<Result<RecorrenciaDto>>;
