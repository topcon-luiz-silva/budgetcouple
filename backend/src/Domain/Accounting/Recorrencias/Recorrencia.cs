namespace BudgetCouple.Domain.Accounting.Recorrencias;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Common;
using System.Text.Json;

/// <summary>
/// Aggregate root for recurring transaction rules.
/// Generates occurrences based on frequency.
/// </summary>
public class Recorrencia : AggregateRoot
{
    public FrequenciaRecorrencia Frequencia { get; private set; }
    public DateOnly DataInicio { get; private set; }
    public DateOnly? DataFim { get; private set; }
    public string TemplateJson { get; private set; } = null!; // JSON serialized template for creating Lancamentos
    public bool Ativa { get; private set; } = true;
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF constructor
    protected Recorrencia() { }

    public static Result<Recorrencia> Create(
        FrequenciaRecorrencia frequencia,
        DateOnly dataInicio,
        DateOnly? dataFim,
        string templateJson)
    {
        // Validation: dataFim must be after dataInicio if provided
        if (dataFim.HasValue && dataFim.Value <= dataInicio)
            return Result.Failure<Recorrencia>(Error.Validation("Data fim deve ser após data início."));

        // Validation: templateJson not empty
        if (string.IsNullOrWhiteSpace(templateJson))
            return Result.Failure<Recorrencia>(Error.Validation("Template JSON não pode estar vazio."));

        return Result.Success(new Recorrencia
        {
            Id = Guid.NewGuid(),
            Frequencia = frequencia,
            DataInicio = dataInicio,
            DataFim = dataFim,
            TemplateJson = templateJson,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Generates all occurrences up to a given date.
    /// Returns a list of snapshots containing data for creating Lancamentos.
    /// </summary>
    public List<RecorrenciaSnapshot> GerarProximasOcorrencias(DateOnly ate)
    {
        var snapshots = new List<RecorrenciaSnapshot>();

        if (!Ativa || DataInicio > ate)
            return snapshots;

        var dataAtual = DataInicio;

        while (dataAtual <= ate)
        {
            // Stop if we've reached the end date
            if (DataFim.HasValue && dataAtual > DataFim.Value)
                break;

            snapshots.Add(new RecorrenciaSnapshot
            {
                RecorrenciaId = Id,
                DataOcorrencia = dataAtual,
                TemplateJson = TemplateJson
            });

            // Calculate next occurrence
            dataAtual = ProximaOcorrencia(dataAtual);
        }

        return snapshots;
    }

    /// <summary>
    /// Calculates the next occurrence date based on frequency.
    /// </summary>
    private DateOnly ProximaOcorrencia(DateOnly data)
    {
        return Frequencia switch
        {
            FrequenciaRecorrencia.MENSAL => data.AddMonths(1),
            FrequenciaRecorrencia.BIMESTRAL => data.AddMonths(2),
            FrequenciaRecorrencia.TRIMESTRAL => data.AddMonths(3),
            FrequenciaRecorrencia.SEMESTRAL => data.AddMonths(6),
            FrequenciaRecorrencia.ANUAL => data.AddYears(1),
            _ => data.AddMonths(1) // default
        };
    }

    public void Desativar()
    {
        Ativa = false;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativa = true;
        AtualizadoEm = DateTime.UtcNow;
    }
}

/// <summary>
/// DTO-like snapshot of a recurrence occurrence.
/// Used internally to pass data for creating Lancamentos.
/// </summary>
public record RecorrenciaSnapshot
{
    public Guid RecorrenciaId { get; init; }
    public DateOnly DataOcorrencia { get; init; }
    public string TemplateJson { get; init; } = null!;
}
