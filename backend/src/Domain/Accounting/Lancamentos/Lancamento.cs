namespace BudgetCouple.Domain.Accounting.Lancamentos;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos.ValueObjects;
using BudgetCouple.Domain.Accounting.DomainEvents;
using BudgetCouple.Domain.Common;

/// <summary>
/// Aggregate root for a transaction (income or expense).
/// Supports simple, installment (parcelada), and recurring transactions.
/// </summary>
public class Lancamento : AggregateRoot
{
    public TipoLancamento Tipo { get; private set; }
    public NaturezaLancamento Natureza { get; private set; }
    public decimal Valor { get; private set; }
    public DateOnly Data { get; private set; }
    public Guid? ContaId { get; private set; }
    public Guid? CartaoId { get; private set; }
    public Guid CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public string? Descricao { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public StatusPagamento StatusPagamento { get; private set; }
    public DateOnly? DataPagamento { get; private set; }
    public bool IsParcelada { get; private set; }
    public DadosParcelamento? DadosParcelamento { get; private set; }
    public bool IsRecorrente { get; private set; }
    public Guid? RecorrenciaId { get; private set; }
    public List<string> Anexos { get; private set; } = new();
    public ClassificacaoRecorrencia Classificacao { get; private set; }
    public bool FaturaPaga { get; private set; } = false;
    public DateTime? FaturaPagaEm { get; private set; }
    public string? HashImportacao { get; private set; } // SHA256 hash for import deduplication
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF constructor
    protected Lancamento() { }

    /// <summary>
    /// Creates a simple (non-installment) transaction.
    /// </summary>
    public static Result<Lancamento> CriarSimples(
        TipoLancamento tipo,
        NaturezaLancamento natureza,
        decimal valor,
        DateOnly data,
        Guid categoriaId,
        Guid? subcategoriaId = null,
        Guid? contaId = null,
        Guid? cartaoId = null,
        string? descricao = null,
        ClassificacaoRecorrencia classificacao = ClassificacaoRecorrencia.VARIAVEL)
    {
        // Validation: conta XOR cartao, except PREVISTA can have both null
        var validacao = ValidarOrigemConta(natureza, contaId, cartaoId);
        if (validacao.IsFailure)
            return Result.Failure<Lancamento>(validacao.Error);

        // Validation: valor > 0
        if (valor <= 0)
            return Result.Failure<Lancamento>(Error.Validation("Valor deve ser maior que zero."));

        var lancamento = new Lancamento
        {
            Id = Guid.NewGuid(),
            Tipo = tipo,
            Natureza = natureza,
            Valor = valor,
            Data = data,
            ContaId = contaId,
            CartaoId = cartaoId,
            CategoriaId = categoriaId,
            SubcategoriaId = subcategoriaId,
            Descricao = descricao,
            StatusPagamento = StatusPagamento.PENDENTE,
            DataPagamento = null,
            IsParcelada = false,
            DadosParcelamento = null,
            IsRecorrente = false,
            RecorrenciaId = null,
            Classificacao = classificacao,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        lancamento.RaiseDomainEvent(new LancamentoCriado(
            lancamento.Id,
            lancamento.Tipo,
            lancamento.Valor,
            lancamento.Data,
            lancamento.CategoriaId));

        return Result.Success(lancamento);
    }

    /// <summary>
    /// Creates an installment transaction (parcelado).
    /// Returns a list with parent and all child transactions.
    /// </summary>
    public static Result<List<Lancamento>> CriarParcelado(
        TipoLancamento tipo,
        NaturezaLancamento natureza,
        decimal valor,
        DateOnly data,
        int totalParcelas,
        Guid categoriaId,
        Guid? subcategoriaId = null,
        Guid? contaId = null,
        Guid? cartaoId = null,
        string? descricao = null,
        ClassificacaoRecorrencia classificacao = ClassificacaoRecorrencia.VARIAVEL)
    {
        // Validation: conta XOR cartao
        var validacao = ValidarOrigemConta(natureza, contaId, cartaoId);
        if (validacao.IsFailure)
            return Result.Failure<List<Lancamento>>(validacao.Error);

        // Validation: totalParcelas >= 2
        if (totalParcelas < 2)
            return Result.Failure<List<Lancamento>>(Error.Validation("Total de parcelas deve ser >= 2."));

        // Validation: valor > 0
        if (valor <= 0)
            return Result.Failure<List<Lancamento>>(Error.Validation("Valor deve ser maior que zero."));

        var valorParcela = valor / totalParcelas;
        var lancamentos = new List<Lancamento>();

        var paiId = Guid.NewGuid();

        // Parent transaction
        var pai = new Lancamento
        {
            Id = paiId,
            Tipo = tipo,
            Natureza = natureza,
            Valor = valor,
            Data = data,
            ContaId = contaId,
            CartaoId = cartaoId,
            CategoriaId = categoriaId,
            SubcategoriaId = subcategoriaId,
            Descricao = descricao,
            StatusPagamento = StatusPagamento.PENDENTE,
            DataPagamento = null,
            IsParcelada = true,
            DadosParcelamento = DadosParcelamento.CriarPai(totalParcelas),
            IsRecorrente = false,
            RecorrenciaId = null,
            Classificacao = classificacao,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        pai.RaiseDomainEvent(new LancamentoCriado(pai.Id, pai.Tipo, pai.Valor, pai.Data, pai.CategoriaId));
        lancamentos.Add(pai);

        // Child transactions for each installment
        for (int i = 2; i <= totalParcelas; i++)
        {
            var dataFilho = data.AddMonths(i - 1);
            var descricaoFilho = $"{descricao} - Parc. {i}/{totalParcelas}".Trim();

            var filho = new Lancamento
            {
                Id = Guid.NewGuid(),
                Tipo = tipo,
                Natureza = natureza,
                Valor = valorParcela,
                Data = dataFilho,
                ContaId = contaId,
                CartaoId = cartaoId,
                CategoriaId = categoriaId,
                SubcategoriaId = subcategoriaId,
                Descricao = descricaoFilho,
                StatusPagamento = StatusPagamento.PENDENTE,
                DataPagamento = null,
                IsParcelada = true,
                DadosParcelamento = DadosParcelamento.CriarFilho(i, totalParcelas, paiId),
                IsRecorrente = false,
                RecorrenciaId = null,
                Classificacao = classificacao,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            };

            filho.RaiseDomainEvent(new LancamentoCriado(filho.Id, filho.Tipo, filho.Valor, filho.Data, filho.CategoriaId));
            lancamentos.Add(filho);
        }

        return Result.Success(lancamentos);
    }

    /// <summary>
    /// Creates a transaction from a recurrence occurrence.
    /// </summary>
    public static Result<Lancamento> CriarRecorrenciaOcorrencia(
        TipoLancamento tipo,
        NaturezaLancamento natureza,
        decimal valor,
        DateOnly data,
        Guid categoriaId,
        Guid recorrenciaId,
        Guid? subcategoriaId = null,
        Guid? contaId = null,
        Guid? cartaoId = null,
        string? descricao = null,
        ClassificacaoRecorrencia classificacao = ClassificacaoRecorrencia.FIXA)
    {
        // Validation: conta XOR cartao
        var validacao = ValidarOrigemConta(natureza, contaId, cartaoId);
        if (validacao.IsFailure)
            return Result.Failure<Lancamento>(validacao.Error);

        // Validation: valor > 0
        if (valor <= 0)
            return Result.Failure<Lancamento>(Error.Validation("Valor deve ser maior que zero."));

        var lancamento = new Lancamento
        {
            Id = Guid.NewGuid(),
            Tipo = tipo,
            Natureza = natureza,
            Valor = valor,
            Data = data,
            ContaId = contaId,
            CartaoId = cartaoId,
            CategoriaId = categoriaId,
            SubcategoriaId = subcategoriaId,
            Descricao = descricao,
            StatusPagamento = StatusPagamento.PENDENTE,
            DataPagamento = null,
            IsParcelada = false,
            DadosParcelamento = null,
            IsRecorrente = true,
            RecorrenciaId = recorrenciaId,
            Classificacao = classificacao,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        lancamento.RaiseDomainEvent(new LancamentoCriado(
            lancamento.Id,
            lancamento.Tipo,
            lancamento.Valor,
            lancamento.Data,
            lancamento.CategoriaId));

        return Result.Success(lancamento);
    }

    /// <summary>
    /// Marks transaction as paid.
    /// </summary>
    public Result Pagar(DateOnly dataPagamento, Guid? contaDebitoId = null)
    {
        // Validation: for realized, needs contaDebitoId
        if (Natureza == NaturezaLancamento.REALIZADA && !contaDebitoId.HasValue)
            return Result.Failure(Error.Validation("Lançamento realizado deve ter conta de débito."));

        StatusPagamento = StatusPagamento.PAGO;
        DataPagamento = dataPagamento;
        AtualizadoEm = DateTime.UtcNow;

        RaiseDomainEvent(new LancamentoPago(Id, Valor, dataPagamento));
        return Result.Success();
    }

    /// <summary>
    /// Marks credit card invoice as paid.
    /// </summary>
    public void MarcarFaturaPaga(DateTime dataPagamento)
    {
        FaturaPaga = true;
        FaturaPagaEm = dataPagamento;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks transaction as overdue.
    /// </summary>
    public void MarcarComoAtrasado(DateOnly hoje)
    {
        if (StatusPagamento != StatusPagamento.PENDENTE)
            return;

        if (Data < hoje)
        {
            StatusPagamento = StatusPagamento.ATRASADO;
            AtualizadoEm = DateTime.UtcNow;
            RaiseDomainEvent(new LancamentoAtrasado(Id, Data, hoje));
        }
    }

    /// <summary>
    /// Creates a "realized" transaction from a forecast (PREVISTA).
    /// Returns a new Lancamento as a child of this forecast.
    /// </summary>
    public Result<Lancamento> GerarRealizadoAPartirDePrevisto(
        decimal valorReal,
        DateOnly dataReal,
        Guid contaDebitoId)
    {
        if (Natureza != NaturezaLancamento.PREVISTA)
            return Result.Failure<Lancamento>(Error.Conflict("Apenas lançamentos PREVISTA podem gerar realizados."));

        if (valorReal <= 0)
            return Result.Failure<Lancamento>(Error.Validation("Valor real deve ser maior que zero."));

        var lancamentoRealizado = new Lancamento
        {
            Id = Guid.NewGuid(),
            Tipo = Tipo,
            Natureza = NaturezaLancamento.REALIZADA,
            Valor = valorReal,
            Data = dataReal,
            ContaId = contaDebitoId,
            CartaoId = null, // Realized is always from account
            CategoriaId = CategoriaId,
            SubcategoriaId = SubcategoriaId,
            Descricao = Descricao,
            StatusPagamento = StatusPagamento.PAGO,
            DataPagamento = dataReal,
            IsParcelada = false,
            DadosParcelamento = null,
            IsRecorrente = IsRecorrente,
            RecorrenciaId = RecorrenciaId,
            Classificacao = Classificacao,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        // Mark this forecast as paid
        Pagar(dataReal, contaDebitoId);

        lancamentoRealizado.RaiseDomainEvent(new LancamentoCriado(
            lancamentoRealizado.Id,
            lancamentoRealizado.Tipo,
            lancamentoRealizado.Valor,
            lancamentoRealizado.Data,
            lancamentoRealizado.CategoriaId));

        return Result.Success(lancamentoRealizado);
    }

    /// <summary>
    /// Updates transaction data (except origem).
    /// </summary>
    public Result AtualizarDados(
        TipoLancamento tipo,
        NaturezaLancamento natureza,
        decimal valor,
        DateOnly data,
        Guid categoriaId,
        Guid? subcategoriaId = null,
        string? descricao = null,
        ClassificacaoRecorrencia classificacao = ClassificacaoRecorrencia.VARIAVEL)
    {
        if (valor <= 0)
            return Result.Failure(Error.Validation("Valor deve ser maior que zero."));

        Tipo = tipo;
        Natureza = natureza;
        Valor = valor;
        Data = data;
        CategoriaId = categoriaId;
        SubcategoriaId = subcategoriaId;
        Descricao = descricao;
        Classificacao = classificacao;
        AtualizadoEm = DateTime.UtcNow;

        return Result.Success();
    }

    private static Result ValidarOrigemConta(NaturezaLancamento natureza, Guid? contaId, Guid? cartaoId)
    {
        var ambosNull = !contaId.HasValue && !cartaoId.HasValue;
        var ambosPresentes = contaId.HasValue && cartaoId.HasValue;

        if (natureza == NaturezaLancamento.PREVISTA)
        {
            // PREVISTA can have: contaId only, cartaoId only, or both null
            if (ambosPresentes)
                return Result.Failure(Error.Validation("Lançamento não pode ter conta e cartão simultaneamente."));
            return Result.Success();
        }
        else
        {
            // REALIZADA must have conta XOR cartao (exactly one)
            if (ambosNull || ambosPresentes)
                return Result.Failure(Error.Validation("Lançamento realizado deve ter conta OU cartão, mas não ambos ou nenhum."));
            return Result.Success();
        }
    }
}
