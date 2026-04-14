namespace BudgetCouple.Application.Auth.Commands.Login;

using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Command to authenticate with PIN.
/// </summary>
public record LoginCommand(string Pin) : IRequest<Result<AuthResult>>;
