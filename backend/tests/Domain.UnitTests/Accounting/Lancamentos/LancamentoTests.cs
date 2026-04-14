namespace BudgetCouple.Domain.UnitTests.Accounting.Lancamentos;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Common;
using Xunit;

public class LancamentoTests
{
    [Fact]
    public void CriarSimples_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var descricao = "Compra de alimentos";
        var valor = 100.00m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = Lancamento.CriarSimples(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            categoriaId,
            contaId: contaId,
            descricao: descricao);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(valor, result.Value.Valor);
        Assert.Equal(data, result.Value.Data);
        Assert.Equal(NaturezaLancamento.PREVISTA, result.Value.Natureza);
        Assert.Equal(StatusPagamento.PENDENTE, result.Value.StatusPagamento);
    }

    [Fact]
    public void CriarSimples_WithZeroValue_ReturnsFalure()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 0m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = Lancamento.CriarSimples(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            categoriaId,
            contaId: contaId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Valor deve ser maior que zero.", result.Error.Message);
    }

    [Fact]
    public void CriarParcelado_WithTotalParcelas3_GeneratesThreeLancamentos()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 300m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var totalParcelas = 3;

        // Act
        var result = Lancamento.CriarParcelado(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            totalParcelas,
            categoriaId,
            contaId: contaId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(totalParcelas, result.Value.Count);

        // Check parent
        var pai = result.Value[0];
        Assert.True(pai.IsParcelada);
        Assert.Null(pai.DadosParcelamento?.LancamentoPaiId);
        Assert.Equal(valor, pai.Valor); // Parent has total value

        // Check children
        for (int i = 1; i < totalParcelas; i++)
        {
            var filho = result.Value[i];
            Assert.True(filho.IsParcelada);
            Assert.Equal(pai.Id, filho.DadosParcelamento?.LancamentoPaiId);
            Assert.Equal(100m, filho.Valor); // Children have divided value
        }
    }

    [Fact]
    public void CriarParcelado_WithTotalParcelas1_ReturnsFalure()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 100m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var totalParcelas = 1;

        // Act
        var result = Lancamento.CriarParcelado(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            totalParcelas,
            categoriaId,
            contaId: contaId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Total de parcelas deve ser >= 2.", result.Error.Message);
    }

    [Fact]
    public void Pagar_WithValidData_UpdatesStatusToPago()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var lancamento = Lancamento.CriarSimples(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            100m,
            DateOnly.FromDateTime(DateTime.UtcNow),
            categoriaId,
            contaId: contaId).Value;

        var dataPagamento = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = lancamento.Pagar(dataPagamento, contaId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StatusPagamento.PAGO, lancamento.StatusPagamento);
        Assert.Equal(dataPagamento, lancamento.DataPagamento);
    }

    [Fact]
    public void CriarRecorrenciaOcorrencia_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var recorrenciaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 100m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = Lancamento.CriarRecorrenciaOcorrencia(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            categoriaId,
            recorrenciaId,
            contaId: contaId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsRecorrente);
        Assert.Equal(recorrenciaId, result.Value.RecorrenciaId);
        Assert.Equal(ClassificacaoRecorrencia.FIXA, result.Value.Classificacao);
    }

    [Fact]
    public void CriarSimples_WithContaAndCartao_ReturnsFalure()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var cartaoId = Guid.NewGuid();
        var valor = 100m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = Lancamento.CriarSimples(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            categoriaId,
            contaId: contaId,
            cartaoId: cartaoId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("não pode ter conta e cartão simultaneamente", result.Error.Message);
    }
}
