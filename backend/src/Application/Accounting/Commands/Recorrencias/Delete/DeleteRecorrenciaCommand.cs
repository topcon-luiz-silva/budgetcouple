namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.Delete;

using BudgetCouple.Domain.Common;
using MediatR;

public record DeleteRecorrenciaCommand(Guid Id) : IRequest<Result>;
