namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateParcelado;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record CreateLancamentoParceladoCommand(
    string DescricaoBase,
    decimal ValorTotal,
    int TotalParcelas,
    DateOnly DataPrimeiraParcela,
    string NaturezaLancamento,
    Guid? ContaId,
    Guid? CartaoId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags,
    string? Observacoes) : IRequest<Result<List<LancamentoDto>>>;
