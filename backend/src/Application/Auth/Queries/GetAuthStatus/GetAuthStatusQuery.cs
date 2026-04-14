namespace BudgetCouple.Application.Auth.Queries.GetAuthStatus;

using BudgetCouple.Application.Auth.DTOs;
using MediatR;

/// <summary>
/// Query to retrieve current authentication status.
/// </summary>
public record GetAuthStatusQuery : IRequest<AuthStatusDto>;
