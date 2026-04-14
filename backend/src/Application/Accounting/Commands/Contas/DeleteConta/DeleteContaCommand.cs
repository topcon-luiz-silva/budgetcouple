namespace BudgetCouple.Application.Accounting.Commands.Contas.DeleteConta;

using BudgetCouple.Domain.Common;
using MediatR;

public record DeleteContaCommand(Guid Id) : IRequest<Result>;
