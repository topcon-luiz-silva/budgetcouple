namespace BudgetCouple.Domain.Imports;

using BudgetCouple.Domain.Common;

/// <summary>
/// Rule for automatic classification of imported transactions.
/// Matches description using CONTAINS or REGEX patterns.
/// </summary>
public class RegraClassificacao : AggregateRoot
{
    public string Padrao { get; private set; } = null!; // Pattern to match
    public TipoPadrao TipoPadrao { get; private set; } // CONTAINS or REGEX
    public Guid CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public int Prioridade { get; private set; } = 100; // Higher = lower priority
    public DateTime CriadoEm { get; private set; }

    // EF constructor
    protected RegraClassificacao() { }

    public static Result<RegraClassificacao> Create(
        string padrao,
        TipoPadrao tipoPadrao,
        Guid categoriaId,
        Guid? subcategoriaId = null,
        int prioridade = 100)
    {
        if (string.IsNullOrWhiteSpace(padrao))
            return Result.Failure<RegraClassificacao>(Error.Validation("Padrão não pode estar vazio."));

        if (categoriaId == Guid.Empty)
            return Result.Failure<RegraClassificacao>(Error.Validation("Categoria ID inválido."));

        if (prioridade < 1)
            return Result.Failure<RegraClassificacao>(Error.Validation("Prioridade deve ser >= 1."));

        // Validate regex if type is REGEX
        if (tipoPadrao == TipoPadrao.REGEX)
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(padrao);
            }
            catch (System.Text.RegularExpressions.RegexParseException ex)
            {
                return Result.Failure<RegraClassificacao>(Error.Validation($"Regex inválido: {ex.Message}"));
            }
        }

        return Result.Success(new RegraClassificacao
        {
            Id = Guid.NewGuid(),
            Padrao = padrao,
            TipoPadrao = tipoPadrao,
            CategoriaId = categoriaId,
            SubcategoriaId = subcategoriaId,
            Prioridade = prioridade,
            CriadoEm = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Checks if the rule matches the given description.
    /// </summary>
    public bool Corresponde(string descricao)
    {
        if (string.IsNullOrEmpty(descricao))
            return false;

        return TipoPadrao switch
        {
            TipoPadrao.CONTAINS => descricao.Contains(Padrao, StringComparison.OrdinalIgnoreCase),
            TipoPadrao.REGEX => System.Text.RegularExpressions.Regex.IsMatch(descricao, Padrao, System.Text.RegularExpressions.RegexOptions.IgnoreCase),
            _ => false
        };
    }
}

public enum TipoPadrao
{
    CONTAINS,
    REGEX
}
