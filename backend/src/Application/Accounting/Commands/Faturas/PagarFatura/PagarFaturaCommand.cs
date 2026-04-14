using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Commands.Faturas.PagarFatura;

public record PagarFaturaCommand(
    Guid CartaoId,
    string Competencia, // YYYY-MM
    DateOnly? DataPagamento = null,
    Guid? ContaDebitoIdOverride = null
) : IRequest<Result<FaturaDto>>;
