namespace BudgetCouple.Application.Accounting.DTOs;

using BudgetCouple.Domain.Accounting.Cartoes;

/// <summary>
/// DTO for Cartao aggregate.
/// </summary>
public record CartaoDto(
    Guid Id,
    string Nome,
    string Bandeira,
    string? UltimosDigitos,
    decimal Limite,
    int DiaFechamento,
    int DiaVencimento,
    Guid ContaPagamentoId,
    string CorHex,
    string? Icone,
    bool Ativa,
    DateTime CriadoEm)
{
    public static CartaoDto FromDomain(Cartao cartao, string? ultimosDigitos = null)
    {
        return new CartaoDto(
            cartao.Id,
            cartao.Nome,
            cartao.Bandeira,
            ultimosDigitos,
            cartao.Limite,
            cartao.DiaFechamento,
            cartao.DiaVencimento,
            cartao.ContaPagamentoId,
            cartao.Cor,
            cartao.Icone,
            cartao.Ativa,
            cartao.CriadoEm);
    }
}
