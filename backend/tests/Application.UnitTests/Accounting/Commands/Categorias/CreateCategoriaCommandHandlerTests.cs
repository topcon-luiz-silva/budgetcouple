namespace BudgetCouple.Application.UnitTests.Accounting.Commands.Categorias;

using BudgetCouple.Application.Accounting.Commands.Categorias.CreateCategoria;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Categorias;
using Moq;

public class CreateCategoriaCommandHandlerTests
{
    private readonly Mock<ICategoriaRepository> _mockCategoriaRepository;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly CreateCategoriaCommandHandler _handler;

    public CreateCategoriaCommandHandlerTests()
    {
        _mockCategoriaRepository = new Mock<ICategoriaRepository>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _handler = new CreateCategoriaCommandHandler(_mockCategoriaRepository.Object, _mockDbContext.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateCategoria()
    {
        // Arrange
        var command = new CreateCategoriaCommand(
            Nome: "Alimentação",
            TipoCategoria: "DESPESA",
            CorHex: "#FF6B6B",
            Icone: "utensils",
            ParentCategoriaId: null);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Alimentação", result.Value.Nome);
        Assert.False(result.Value.Sistema);
        Assert.True(result.Value.Ativa);
        Assert.Equal("#FF6B6B", result.Value.CorHex);
        _mockCategoriaRepository.Verify(x => x.Add(It.IsAny<Categoria>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidTipoCategoria_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateCategoriaCommand(
            Nome: "Categoria",
            TipoCategoria: "INVALID_TYPE",
            CorHex: "#FF6B6B",
            Icone: null,
            ParentCategoriaId: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockCategoriaRepository.Verify(x => x.Add(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptyNome_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateCategoriaCommand(
            Nome: "",
            TipoCategoria: "DESPESA",
            CorHex: "#FF6B6B",
            Icone: null,
            ParentCategoriaId: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mockCategoriaRepository.Verify(x => x.Add(It.IsAny<Categoria>()), Times.Never);
    }
}
