namespace BudgetCouple.Domain.Accounting.DomainEvents;

using BudgetCouple.Domain.Common;

public record LancamentoCriado(
    Guid LancamentoId,
    TipoLancamento Tipo,
    decimal Valor,
    DateOnly Data,
    Guid CategoriaId) : DomainEvent;

public record LancamentoPago(
    Guid LancamentoId,
    decimal Valor,
    DateOnly DataPagamento) : DomainEvent;

public record LancamentoAtrasado(
    Guid LancamentoId,
    DateOnly DataEsperada,
    DateOnly DataAtual) : DomainEvent;

public record FaturaFechada(
    Guid CartaoId,
    DateOnly Competencia,
    decimal TotalFatura) : DomainEvent;

public record FaturaPaga(
    Guid CartaoId,
    DateOnly Competencia,
    decimal TotalPago,
    Guid ContaDebitoId) : DomainEvent;

public record CategoriaOrcamentoAtingiu80(
    Guid CategoriaId,
    string NomeCategoria,
    decimal OrcadoMes,
    decimal GastoMes) : DomainEvent;

public record CategoriaOrcamentoEstourou(
    Guid CategoriaId,
    string NomeCategoria,
    decimal OrcadoMes,
    decimal GastoMes) : DomainEvent;

public record MetaAtingida(
    Guid MetaId,
    string NomeMeta,
    decimal Valor) : DomainEvent;

public record RecorrenciaGerouLancamentos(
    Guid RecorrenciaId,
    int QuantidadeLancamentos,
    DateOnly Periodo) : DomainEvent;
