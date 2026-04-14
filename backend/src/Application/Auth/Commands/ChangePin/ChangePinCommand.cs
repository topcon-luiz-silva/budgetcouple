namespace BudgetCouple.Application.Auth.Commands.ChangePin;

using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Command to change the current PIN.
/// </summary>
public record ChangePinCommand(string PinAtual, string NovoPin) : IRequest<Result>;
