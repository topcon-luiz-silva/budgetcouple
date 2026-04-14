namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateSimples;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record CreateLancamentoSimplesCommand(
    string Descricao,
    decimal Valor,
    DateOnly DataCompetencia,
    DateOnly? DataVencimento,
    string NaturezaLancamento,
    Guid? ContaId,
    Guid? CartaoId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags,
    string? Observacoes,
    string StatusPagamento) : IRequest<Result<LancamentoDto>>;
