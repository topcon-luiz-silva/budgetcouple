namespace BudgetCouple.Application.Accounting.DTOs;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;

/// <summary>
/// DTO for Lancamento aggregate.
/// </summary>
public record LancamentoDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateOnly DataCompetencia,
    DateOnly? DataVencimento,
    DateOnly? DataPagamento,
    TipoLancamento TipoLancamento,
    NaturezaLancamento NaturezaLancamento,
    StatusPagamento StatusPagamento,
    Guid? ContaId,
    string? ContaNome,
    Guid? CartaoId,
    string? CartaoNome,
    Guid CategoriaId,
    string CategoriaNome,
    Guid? SubcategoriaId,
    string? SubcategoriaNome,
    int? Parcela,
    int? TotalParcelas,
    Guid? RecorrenciaId,
    Guid? LancamentoPaiId,
    List<string> Tags,
    string? Observacoes,
    DateTime CriadoEm,
    DateTime AtualizadoEm)
{
    public static LancamentoDto FromDomain(
        Lancamento lancamento,
        string? contaNome = null,
        string? cartaoNome = null,
        string categoriaNome = "",
        string? subcategoriaNome = null)
    {
        return new LancamentoDto(
            Id: lancamento.Id,
            Descricao: lancamento.Descricao ?? "",
            Valor: lancamento.Valor,
            DataCompetencia: lancamento.Data,
            DataVencimento: null, // DataVencimento is not stored in domain
            DataPagamento: lancamento.DataPagamento,
            TipoLancamento: lancamento.Tipo,
            NaturezaLancamento: lancamento.Natureza,
            StatusPagamento: lancamento.StatusPagamento,
            ContaId: lancamento.ContaId,
            ContaNome: contaNome,
            CartaoId: lancamento.CartaoId,
            CartaoNome: cartaoNome,
            CategoriaId: lancamento.CategoriaId,
            CategoriaNome: categoriaNome,
            SubcategoriaId: lancamento.SubcategoriaId,
            SubcategoriaNome: subcategoriaNome,
            Parcela: lancamento.IsParcelada ? lancamento.DadosParcelamento?.ParcelaAtual : null,
            TotalParcelas: lancamento.IsParcelada ? lancamento.DadosParcelamento?.TotalParcelas : null,
            RecorrenciaId: lancamento.RecorrenciaId,
            LancamentoPaiId: lancamento.IsParcelada ? lancamento.DadosParcelamento?.LancamentoPaiId : null,
            Tags: lancamento.Tags,
            Observacoes: null,
            CriadoEm: lancamento.CriadoEm,
            AtualizadoEm: lancamento.AtualizadoEm);
    }
}
