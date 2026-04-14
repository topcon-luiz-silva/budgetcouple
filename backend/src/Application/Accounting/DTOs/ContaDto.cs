namespace BudgetCouple.Application.Accounting.DTOs;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Contas;

/// <summary>
/// DTO for Conta aggregate.
/// </summary>
public record ContaDto(
    Guid Id,
    string Nome,
    TipoConta TipoConta,
    decimal SaldoInicial,
    decimal SaldoAtual,
    string CorHex,
    string? Icone,
    bool Ativa,
    DateTime CriadoEm)
{
    public static ContaDto FromDomain(Conta conta, decimal saldoAtual)
    {
        return new ContaDto(
            conta.Id,
            conta.Nome,
            conta.Tipo,
            conta.SaldoInicial,
            saldoAtual,
            conta.Cor,
            conta.Icone,
            conta.Ativa,
            conta.CriadoEm);
    }
}
