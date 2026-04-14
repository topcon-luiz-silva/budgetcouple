namespace BudgetCouple.Application.UnitTests.Accounting.Commands.Contas;

using BudgetCouple.Application.Accounting.Commands.Contas.CreateConta;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Contas;
using Moq;

public class CreateContaCommandHandlerTests
{
    private readonly Mock<IContaRepository> _mockContaRepository;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly CreateContaCommandHandler _handler;

    public CreateContaCommandHandlerTests()
    {
        _mockContaRepository = new Mock<IContaRepository>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _handler = new CreateContaCommandHandler(_mockContaRepository.Object, _mockDbContext.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateConta()
    {
        // Arrange
        var command = new CreateContaCommand(
            Nome: "Conta Corrente",
            TipoConta: "CONTA_CORRENTE",
            SaldoInicial: 1000m,
            CorHex: "#FF0000",
            Icone: "wallet",
            Observacoes: "Minha conta corrente");

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Conta Corrente", result.Value.Nome);
        Assert.Equal(TipoConta.CONTA_CORRENTE, result.Value.TipoConta);
        Assert.Equal(1000m, result.Value.SaldoInicial);
        Assert.Equal(1000m, result.Value.SaldoAtual);
        Assert.Equal("#FF0000", result.Value.CorHex);
        _mockContaRepository.Verify(x => x.Add(It.IsAny<Conta>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidTipoConta_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateContaCommand(
            Nome: "Conta",
            TipoConta: "INVALID_TYPE",
            SaldoInicial: 1000m,
            CorHex: "#FF0000",
            Icone: "wallet",
            Observacoes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockContaRepository.Verify(x => x.Add(It.IsAny<Conta>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativeSaldoInicial_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateContaCommand(
            Nome: "Conta",
            TipoConta: "CONTA_CORRENTE",
            SaldoInicial: -100m,
            CorHex: "#FF0000",
            Icone: "wallet",
            Observacoes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockContaRepository.Verify(x => x.Add(It.IsAny<Conta>()), Times.Never);
    }
}
