namespace BudgetCouple.Application.Accounting.Commands.Cartoes.DeleteCartao;

using BudgetCouple.Domain.Common;
using MediatR;

public record DeleteCartaoCommand(Guid Id) : IRequest<Result>;
