namespace BudgetCouple.Domain.Accounting.Lancamentos;

using BudgetCouple.Domain.Common;

/// <summary>
/// Entity representing an attachment to a transaction (Lançamento).
/// </summary>
public class LancamentoAnexo : Entity
{
    public Guid LancamentoId { get; private set; }
    public string NomeArquivo { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long TamanhoBytes { get; private set; }
    public string CaminhoStorage { get; private set; } = string.Empty;
    public DateTime EnviadoEm { get; private set; }

    // EF constructor
    protected LancamentoAnexo() { }

    /// <summary>
    /// Creates a new attachment for a transaction.
    /// </summary>
    public static Result<LancamentoAnexo> Criar(
        Guid lancamentoId,
        string nomeArquivo,
        string contentType,
        long tamanhoBytes,
        string caminhoStorage)
    {
        // Validation
        if (lancamentoId == Guid.Empty)
            return Result.Failure<LancamentoAnexo>(Error.Validation("LancamentoId deve ser um GUID válido."));

        if (string.IsNullOrWhiteSpace(nomeArquivo))
            return Result.Failure<LancamentoAnexo>(Error.Validation("NomeArquivo é obrigatório."));

        if (tamanhoBytes <= 0)
            return Result.Failure<LancamentoAnexo>(Error.Validation("TamanhoBytes deve ser maior que zero."));

        if (tamanhoBytes > 10 * 1024 * 1024) // 10 MB
            return Result.Failure<LancamentoAnexo>(Error.Validation("Arquivo excede o limite de 10 MB."));

        var anexo = new LancamentoAnexo
        {
            Id = Guid.NewGuid(),
            LancamentoId = lancamentoId,
            NomeArquivo = nomeArquivo,
            ContentType = contentType,
            TamanhoBytes = tamanhoBytes,
            CaminhoStorage = caminhoStorage,
            EnviadoEm = DateTime.UtcNow
        };

        return Result.Success(anexo);
    }
}
