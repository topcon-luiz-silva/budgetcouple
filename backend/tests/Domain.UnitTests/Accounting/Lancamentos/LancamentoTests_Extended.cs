namespace BudgetCouple.Domain.UnitTests.Accounting.Lancamentos;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using Xunit;

public class LancamentoTestsExtended
{
    [Fact]
    public void CriarParcelado_With24Parcelas_GeneratesCorrectly()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 2400m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var totalParcelas = 24;

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
    }

    [Fact]
    public void CriarSimples_WithNegativeValue_ReturnsFalure()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = -100m;
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
    }

    [Fact]
    public void MarcarComoAtrasado_UpdatesStatusProperly()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var dataPastro = new DateOnly(2026, 3, 1);
        var lancamento = Lancamento.CriarSimples(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            100m,
            dataPastro,
            categoriaId,
            contaId: contaId).Value;

        var dataHoje = new DateOnly(2026, 4, 15);

        // Act
        lancamento.MarcarComoAtrasado(dataHoje);

        // Assert
        Assert.Equal(StatusPagamento.ATRASADO, lancamento.StatusPagamento);
    }

    [Fact]
    public void CriarSimples_WithCartaoOnly_ReturnsSuccess()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
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
            cartaoId: cartaoId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cartaoId, result.Value.CartaoId);
        Assert.Null(result.Value.ContaId);
    }

    [Fact]
    public void GerarRealizadoAPartirDePrevisto_ConvertsPrevToReal()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var lancamento = Lancamento.CriarSimples(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            100m,
            new DateOnly(2026, 4, 1),
            categoriaId,
            contaId: contaId).Value;

        var valorReal = 95m;
        var dataRealizacao = new DateOnly(2026, 4, 5);

        // Act
        var result = lancamento.GerarRealizadoAPartirDePrevisto(valorReal, dataRealizacao, contaId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(NaturezaLancamento.REALIZADA, result.Value.Natureza);
        Assert.Equal(valorReal, result.Value.Valor);
        Assert.Equal(dataRealizacao, result.Value.Data);
    }

    [Fact]
    public void GerarRealizadoAPartirDePrevisto_WithZeroValue_ReturnsFalure()
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

        // Act
        var result = lancamento.GerarRealizadoAPartirDePrevisto(0, DateOnly.FromDateTime(DateTime.UtcNow), contaId);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void CriarRecorrenciaOcorrencia_WithDifferentFrequencies()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var recorrenciaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 100m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = Lancamento.CriarRecorrenciaOcorrencia(
            TipoLancamento.RECEITA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            categoriaId,
            recorrenciaId,
            contaId: contaId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsRecorrente);
        Assert.Equal(TipoLancamento.RECEITA, result.Value.Tipo);
    }

    [Fact]
    public void CriarParcelado_With2Parcelas_MinimumAllowed()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var contaId = Guid.NewGuid();
        var valor = 200m;
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = Lancamento.CriarParcelado(
            TipoLancamento.DESPESA,
            NaturezaLancamento.PREVISTA,
            valor,
            data,
            2,
            categoriaId,
            contaId: contaId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }
}
