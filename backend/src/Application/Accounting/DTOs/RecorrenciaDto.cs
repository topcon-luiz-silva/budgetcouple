namespace BudgetCouple.Application.Accounting.DTOs;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Recorrencias;

/// <summary>
/// DTO for Recorrencia aggregate.
/// </summary>
public record RecorrenciaDto(
    Guid Id,
    FrequenciaRecorrencia Frequencia,
    DateOnly DataInicio,
    DateOnly? DataFim,
    bool Ativa,
    DateTime CriadoEm,
    DateTime AtualizadoEm)
{
    public static RecorrenciaDto FromDomain(Recorrencia recorrencia)
    {
        return new RecorrenciaDto(
            Id: recorrencia.Id,
            Frequencia: recorrencia.Frequencia,
            DataInicio: recorrencia.DataInicio,
            DataFim: recorrencia.DataFim,
            Ativa: recorrencia.Ativa,
            CriadoEm: recorrencia.CriadoEm,
            AtualizadoEm: recorrencia.AtualizadoEm);
    }
}
