namespace BudgetCouple.Application.Tests.Budgeting;

using BudgetCouple.Application.Budgeting.Metas.Commands;
using BudgetCouple.Application.Budgeting.Metas.Queries;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Budgeting.Metas;
using Moq;
using Xunit;

public class MetasTests
{
    [Fact]
    public async Task CriarMetaEconomia_DeveRetornarMetaComValoresCorretos()
    {
        // Arrange
        var metaRepositoryMock = new Mock<IMetaRepository>();
        var command = new CriarMetaCommand(
            Tipo: "ECONOMIA",
            Nome: "Fundo de Emergência",
            ValorAlvo: 10000,
            DataLimite: new DateOnly(2026, 12, 31),
            PercentualAlerta: 80);

        metaRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Meta>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        metaRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CriarMetaCommandHandler(metaRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("ECONOMIA", result.Value.Tipo);
        Assert.Equal("Fundo de Emergência", result.Value.Nome);
        Assert.Equal(10000, result.Value.ValorAlvo);
        Assert.Equal(80, result.Value.PercentualAlerta);
        metaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarMetaReducaoCategoria_DeveRetornarMetaComCategoriaId()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var metaRepositoryMock = new Mock<IMetaRepository>();
        var command = new CriarMetaCommand(
            Tipo: "REDUCAO_CATEGORIA",
            Nome: "Reduzir Gastos com Alimentação",
            ValorAlvo: 500,
            CategoriaId: categoriaId,
            Periodo: "MENSAL",
            PercentualAlerta: 80);

        metaRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Meta>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        metaRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CriarMetaCommandHandler(metaRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("REDUCAO_CATEGORIA", result.Value.Tipo);
        Assert.Equal(categoriaId, result.Value.CategoriaId);
        Assert.Equal(500, result.Value.ValorAlvo);
        metaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Meta>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterMetaProgresso_ParaMetaEconomia_DeveCalcularPercentualCorretamente()
    {
        // Arrange
        var metaId = Guid.NewGuid();
        var meta = new MetaEconomia
        {
            Id = metaId,
            Tipo = "ECONOMIA",
            Nome = "Fundo de Emergência",
            ValorAlvo = 10000,
            ValorAtual = 5000,
            DataLimite = new DateOnly(2026, 12, 31),
            PercentualAlerta = 80,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        var metaRepositoryMock = new Mock<IMetaRepository>();
        var lancamentoRepositoryMock = new Mock<ILancamentoRepository>();

        metaRepositoryMock
            .Setup(r => r.GetByIdAsync(metaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meta);

        lancamentoRepositoryMock
            .Setup(r => r.ListAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Lancamento>());

        metaRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new ObterMetaProgressoQueryHandler(metaRepositoryMock.Object, lancamentoRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new ObterMetaProgressoQuery(metaId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("ECONOMIA", result.Value.Tipo);
        Assert.Equal(50, result.Value.PercentualProgresso); // 5000/10000 = 50%
        Assert.True(result.Value.AtingiuAlerta == false); // 50% < 80%
        Assert.False(result.Value.Atingida); // 5000 < 10000
    }

    [Fact]
    public async Task AtualizarMeta_DeveModificarValoresCorretamente()
    {
        // Arrange
        var metaId = Guid.NewGuid();
        var meta = new MetaEconomia
        {
            Id = metaId,
            Tipo = "ECONOMIA",
            Nome = "Fundo Antigo",
            ValorAlvo = 5000,
            ValorAtual = 0,
            DataLimite = new DateOnly(2026, 06, 30),
            PercentualAlerta = 80,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        var metaRepositoryMock = new Mock<IMetaRepository>();
        metaRepositoryMock
            .Setup(r => r.GetByIdAsync(metaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meta);

        metaRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AtualizarMetaCommandHandler(metaRepositoryMock.Object);
        var command = new AtualizarMetaCommand(
            metaId,
            "Fundo de Emergência Atualizado",
            10000,
            new DateOnly(2026, 12, 31),
            75);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Fundo de Emergência Atualizado", result.Value.Nome);
        Assert.Equal(10000, result.Value.ValorAlvo);
        Assert.Equal(75, result.Value.PercentualAlerta);
        metaRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExcluirMeta_DeveRemoverMetaDoBancoDeDados()
    {
        // Arrange
        var metaId = Guid.NewGuid();
        var meta = new MetaEconomia
        {
            Id = metaId,
            Tipo = "ECONOMIA",
            Nome = "Meta para Excluir",
            ValorAlvo = 5000,
            ValorAtual = 0,
            PercentualAlerta = 80,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        var metaRepositoryMock = new Mock<IMetaRepository>();
        metaRepositoryMock
            .Setup(r => r.GetByIdAsync(metaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meta);

        metaRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new ExcluirMetaCommandHandler(metaRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new ExcluirMetaCommand(metaId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        metaRepositoryMock.Verify(r => r.Remove(meta), Times.Once);
        metaRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
