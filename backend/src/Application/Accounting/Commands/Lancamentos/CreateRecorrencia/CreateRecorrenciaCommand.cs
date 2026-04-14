namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateRecorrencia;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record CreateRecorrenciaCommand(
    string DescricaoBase,
    decimal ValorBase,
    string Frequencia,
    DateOnly DataInicio,
    DateOnly? DataFim,
    string NaturezaLancamento,
    Guid? ContaId,
    Guid? CartaoId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags,
    string? Observacoes,
    DateOnly? GerarOcorrenciasAte) : IRequest<Result<(RecorrenciaDto recorrencia, List<LancamentoDto> lancamentos)>>;
