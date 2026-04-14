namespace BudgetCouple.Domain.Imports;

using BudgetCouple.Domain.Common;

/// <summary>
/// Rule for automatic classification of imported transactions.
/// Matches description using various pattern types.
/// </summary>
public class RegraClassificacao : AggregateRoot
{
    public string Nome { get; private set; } = null!; // Rule name
    public string Padrao { get; private set; } = null!; // Pattern to match
    public TipoPadrao TipoPadrao { get; private set; } // CONTEM, IGUAL, REGEX, COMECA_COM, TERMINA_COM
    public Guid CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public int Prioridade { get; private set; } = 100; // Higher = lower priority
    public bool Ativa { get; private set; } = true;
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF constructor
    protected RegraClassificacao() { }

    public static Result<RegraClassificacao> Create(
        string nome,
        string padrao,
        TipoPadrao tipoPadrao,
        Guid categoriaId,
        Guid? subcategoriaId = null,
        int prioridade = 100,
        bool ativa = true)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<RegraClassificacao>(Error.Validation("Nome não pode estar vazio."));

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

        var now = DateTime.UtcNow;
        return Result.Success(new RegraClassificacao
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Padrao = padrao,
            TipoPadrao = tipoPadrao,
            CategoriaId = categoriaId,
            SubcategoriaId = subcategoriaId,
            Prioridade = prioridade,
            Ativa = ativa,
            CriadoEm = now,
            AtualizadoEm = now
        });
    }

    public Result Atualizar(
        string nome,
        string padrao,
        TipoPadrao tipoPadrao,
        Guid categoriaId,
        Guid? subcategoriaId = null,
        int prioridade = 100,
        bool ativa = true)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(Error.Validation("Nome não pode estar vazio."));

        if (string.IsNullOrWhiteSpace(padrao))
            return Result.Failure(Error.Validation("Padrão não pode estar vazio."));

        if (categoriaId == Guid.Empty)
            return Result.Failure(Error.Validation("Categoria ID inválido."));

        if (prioridade < 1)
            return Result.Failure(Error.Validation("Prioridade deve ser >= 1."));

        if (tipoPadrao == TipoPadrao.REGEX)
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(padrao);
            }
            catch (System.Text.RegularExpressions.RegexParseException ex)
            {
                return Result.Failure(Error.Validation($"Regex inválido: {ex.Message}"));
            }
        }

        Nome = nome;
        Padrao = padrao;
        TipoPadrao = tipoPadrao;
        CategoriaId = categoriaId;
        SubcategoriaId = subcategoriaId;
        Prioridade = prioridade;
        Ativa = ativa;
        AtualizadoEm = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Checks if the rule matches the given description.
    /// </summary>
    public bool Corresponde(string descricao)
    {
        if (!Ativa || string.IsNullOrEmpty(descricao))
            return false;

        return TipoPadrao switch
        {
            TipoPadrao.CONTEM => descricao.Contains(Padrao, StringComparison.OrdinalIgnoreCase),
            TipoPadrao.IGUAL => descricao.Equals(Padrao, StringComparison.OrdinalIgnoreCase),
            TipoPadrao.REGEX => System.Text.RegularExpressions.Regex.IsMatch(descricao, Padrao, System.Text.RegularExpressions.RegexOptions.IgnoreCase),
            TipoPadrao.COMECA_COM => descricao.StartsWith(Padrao, StringComparison.OrdinalIgnoreCase),
            TipoPadrao.TERMINA_COM => descricao.EndsWith(Padrao, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}

public enum TipoPadrao
{
    CONTEM,
    IGUAL,
    REGEX,
    COMECA_COM,
    TERMINA_COM
}
