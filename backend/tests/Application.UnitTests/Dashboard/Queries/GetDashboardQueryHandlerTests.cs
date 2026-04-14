namespace BudgetCouple.Application.UnitTests.Dashboard.Queries;

using BudgetCouple.Application.Budgeting.Metas.Queries;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Application.Dashboard.Queries.GetDashboard;
using BudgetCouple.Domain.Accounting;
using MediatR;
using BudgetCouple.Domain.Accounting.Cartoes;
using BudgetCouple.Domain.Accounting.Categorias;
using BudgetCouple.Domain.Accounting.Contas;
using BudgetCouple.Domain.Accounting.Lancamentos;
using Microsoft.Extensions.Logging;
using Moq;

public class GetDashboardQueryHandlerTests
{
    private readonly Mock<ILancamentoRepository> _mockLancamentoRepository;
    private readonly Mock<ICategoriaRepository> _mockCategoriaRepository;
    private readonly Mock<IContaRepository> _mockContaRepository;
    private readonly Mock<ICartaoRepository> _mockCartaoRepository;
    private readonly Mock<IMetaRepository> _mockMetaRepository;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<GetDashboardQueryHandler>> _mockLogger;
    private readonly GetDashboardQueryHandler _handler;

    public GetDashboardQueryHandlerTests()
    {
        _mockLancamentoRepository = new Mock<ILancamentoRepository>();
        _mockCategoriaRepository = new Mock<ICategoriaRepository>();
        _mockContaRepository = new Mock<IContaRepository>();
        _mockCartaoRepository = new Mock<ICartaoRepository>();
        _mockMetaRepository = new Mock<IMetaRepository>();
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<GetDashboardQueryHandler>>();

        _handler = new GetDashboardQueryHandler(
            _mockLancamentoRepository.Object,
            _mockCategoriaRepository.Object,
            _mockContaRepository.Object,
            _mockCartaoRepository.Object,
            _mockMetaRepository.Object,
            _mockMediator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidMonth_ShouldReturnDashboard()
    {
        // Arrange
        var mes = "2026-04";

        _mockLancamentoRepository
            .Setup(x => x.ListAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                int.MaxValue,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Lancamento>());

        _mockCategoriaRepository
            .Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Categoria>());

        _mockContaRepository
            .Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Conta>());

        _mockCartaoRepository
            .Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cartao>());

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ListarAlertasOrcamentoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Domain.Common.Result.Success(new List<BudgetCouple.Application.Budgeting.Metas.DTOs.AlertaOrcamentoDashboardDto>()));

        var query = new GetDashboardQuery(mes);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var dashboard = result.Value;
        Assert.Equal(mes, dashboard.Competencia);
        Assert.NotNull(dashboard.Resumo);
        Assert.NotNull(dashboard.PorCategoria);
        Assert.NotNull(dashboard.PorConta);
        Assert.NotNull(dashboard.PorCartao);
        Assert.NotNull(dashboard.EvolucaoMensal);
        Assert.NotNull(dashboard.ProximosVencimentos);
    }

    [Fact]
    public async Task Handle_WithInvalidMonth_ShouldReturnFailure()
    {
        // Arrange
        var invalidMes = "invalid-month";
        var query = new GetDashboardQuery(invalidMes);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }
}
