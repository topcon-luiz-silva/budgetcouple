namespace BudgetCouple.Application.UnitTests.Accounting.Commands.Cartoes;

using BudgetCouple.Application.Accounting.Commands.Cartoes.CreateCartao;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Cartoes;
using BudgetCouple.Domain.Accounting.Contas;
using Moq;

public class CreateCartaoCommandHandlerTests
{
    private readonly Mock<ICartaoRepository> _mockCartaoRepository;
    private readonly Mock<IContaRepository> _mockContaRepository;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly CreateCartaoCommandHandler _handler;

    public CreateCartaoCommandHandlerTests()
    {
        _mockCartaoRepository = new Mock<ICartaoRepository>();
        _mockContaRepository = new Mock<IContaRepository>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _handler = new CreateCartaoCommandHandler(
            _mockCartaoRepository.Object,
            _mockContaRepository.Object,
            _mockDbContext.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateCartao()
    {
        // Arrange
        var contaPagamentoId = Guid.NewGuid();
        var mockConta = new Mock<Conta>();

        _mockContaRepository.Setup(x => x.GetByIdAsync(contaPagamentoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockConta.Object);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateCartaoCommand(
            Nome: "Cartão Crédito",
            Bandeira: "VISA",
            UltimosDigitos: "1234",
            Limite: 5000m,
            DiaFechamento: 10,
            DiaVencimento: 15,
            ContaPagamentoId: contaPagamentoId,
            CorHex: "#0000FF",
            Icone: "creditcard");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Cartão Crédito", result.Value.Nome);
        Assert.Equal("VISA", result.Value.Bandeira);
        Assert.Equal(5000m, result.Value.Limite);
        Assert.Equal(10, result.Value.DiaFechamento);
        Assert.Equal(15, result.Value.DiaVencimento);
        _mockCartaoRepository.Verify(x => x.Add(It.IsAny<Cartao>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidDiaFechamento_ShouldReturnFailure()
    {
        // Arrange
        var contaPagamentoId = Guid.NewGuid();
        var mockConta = new Mock<Conta>();

        _mockContaRepository.Setup(x => x.GetByIdAsync(contaPagamentoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockConta.Object);

        var command = new CreateCartaoCommand(
            Nome: "Cartão",
            Bandeira: "VISA",
            UltimosDigitos: null,
            Limite: 5000m,
            DiaFechamento: 32, // Invalid
            DiaVencimento: 15,
            ContaPagamentoId: contaPagamentoId,
            CorHex: "#0000FF",
            Icone: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockCartaoRepository.Verify(x => x.Add(It.IsAny<Cartao>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentConta_ShouldReturnFailure()
    {
        // Arrange
        var contaPagamentoId = Guid.NewGuid();

        _mockContaRepository.Setup(x => x.GetByIdAsync(contaPagamentoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conta?)null);

        var command = new CreateCartaoCommand(
            Nome: "Cartão",
            Bandeira: "VISA",
            UltimosDigitos: null,
            Limite: 5000m,
            DiaFechamento: 10,
            DiaVencimento: 15,
            ContaPagamentoId: contaPagamentoId,
            CorHex: "#0000FF",
            Icone: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockCartaoRepository.Verify(x => x.Add(It.IsAny<Cartao>()), Times.Never);
    }
}
