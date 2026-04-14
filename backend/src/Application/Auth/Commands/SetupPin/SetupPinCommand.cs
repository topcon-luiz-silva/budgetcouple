namespace BudgetCouple.Application.Auth.Commands.SetupPin;

using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Command to set up the initial PIN for the application.
/// </summary>
public record SetupPinCommand(string Pin) : IRequest<Result<AuthResult>>;
