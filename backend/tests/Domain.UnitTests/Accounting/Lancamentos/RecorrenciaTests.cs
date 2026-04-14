namespace BudgetCouple.Domain.UnitTests.Accounting.Lancamentos;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Recorrencias;
using BudgetCouple.Domain.Common;
using System.Text.Json;
using Xunit;

public class RecorrenciaTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
        var dataFim = dataInicio.AddMonths(12);
        var template = JsonSerializer.Serialize(new { valor = 100m, descricao = "Test" });

        // Act
        var result = Recorrencia.Create(frequencia, dataInicio, dataFim, template);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(frequencia, result.Value.Frequencia);
        Assert.Equal(dataInicio, result.Value.DataInicio);
        Assert.Equal(dataFim, result.Value.DataFim);
        Assert.True(result.Value.Ativa);
    }

    [Fact]
    public void Create_WithDataFimBeforeDataInicio_ReturnsFalure()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
        var dataFim = dataInicio.AddMonths(-1);
        var template = JsonSerializer.Serialize(new { valor = 100m });

        // Act
        var result = Recorrencia.Create(frequencia, dataInicio, dataFim, template);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Data fim deve ser após data início.", result.Error.Message);
    }

    [Fact]
    public void Create_WithEmptyTemplate_ReturnsFalure()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
        var template = "";

        // Act
        var result = Recorrencia.Create(frequencia, dataInicio, null, template);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Template JSON não pode estar vazio.", result.Error.Message);
    }

    [Fact]
    public void GerarProximasOcorrencias_WithMensalFrequencia_GeneratesCorrectDates()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = new DateOnly(2026, 1, 15);
        var dataFim = new DateOnly(2026, 3, 15);
        var template = JsonSerializer.Serialize(new { valor = 100m });

        var recorrenciaResult = Recorrencia.Create(frequencia, dataInicio, dataFim, template);
        var recorrencia = recorrenciaResult.Value;

        // Act
        var snapshots = recorrencia.GerarProximasOcorrencias(dataFim);

        // Assert
        Assert.Equal(3, snapshots.Count);
        Assert.Equal(dataInicio, snapshots[0].DataOcorrencia);
        Assert.Equal(dataInicio.AddMonths(1), snapshots[1].DataOcorrencia);
        Assert.Equal(dataInicio.AddMonths(2), snapshots[2].DataOcorrencia);
    }

    [Fact]
    public void GerarProximasOcorrencias_Inactive_ReturnsEmptyList()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = new DateOnly(2026, 1, 15);
        var template = JsonSerializer.Serialize(new { valor = 100m });

        var recorrenciaResult = Recorrencia.Create(frequencia, dataInicio, null, template);
        var recorrencia = recorrenciaResult.Value;
        recorrencia.Desativar();

        // Act
        var snapshots = recorrencia.GerarProximasOcorrencias(dataInicio.AddMonths(12));

        // Assert
        Assert.Empty(snapshots);
    }

    [Fact]
    public void Desativar_UpdatesAtiva()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
        var template = JsonSerializer.Serialize(new { valor = 100m });

        var recorrenciaResult = Recorrencia.Create(frequencia, dataInicio, null, template);
        var recorrencia = recorrenciaResult.Value;
        Assert.True(recorrencia.Ativa);

        // Act
        recorrencia.Desativar();

        // Assert
        Assert.False(recorrencia.Ativa);
    }

    [Fact]
    public void Ativar_UpdatesAtiva()
    {
        // Arrange
        var frequencia = FrequenciaRecorrencia.MENSAL;
        var dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
        var template = JsonSerializer.Serialize(new { valor = 100m });

        var recorrenciaResult = Recorrencia.Create(frequencia, dataInicio, null, template);
        var recorrencia = recorrenciaResult.Value;
        recorrencia.Desativar();
        Assert.False(recorrencia.Ativa);

        // Act
        recorrencia.Ativar();

        // Assert
        Assert.True(recorrencia.Ativa);
    }
}
