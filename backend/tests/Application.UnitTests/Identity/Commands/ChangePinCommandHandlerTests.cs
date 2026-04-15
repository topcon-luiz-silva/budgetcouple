namespace BudgetCouple.Application.UnitTests.Identity.Commands;

using BudgetCouple.Application.Auth.Commands.ChangePin;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Identity;
using Moq;
using Xunit;

public class ChangePinCommandHandlerTests
{
    private readonly Mock<IAppConfigRepository> _mockRepository;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<IPinHasher> _mockPinHasher;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly ChangePinCommandHandler _handler;

    public ChangePinCommandHandlerTests()
    {
        _mockRepository = new Mock<IAppConfigRepository>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockPinHasher = new Mock<IPinHasher>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _handler = new ChangePinCommandHandler(
            _mockRepository.Object,
            _mockDbContext.Object,
            _mockPinHasher.Object,
            _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_WithValidCurrentPin_ShouldChangePin()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("currenthash", now);

        var command = new ChangePinCommand("1234", "5678");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Verify("1234", "currenthash")).Returns(true);
        _mockPinHasher.Setup(x => x.Hash("5678")).Returns("newhash");
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithIncorrectCurrentPin_ShouldReturnFailure()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("currenthash", now);

        var command = new ChangePinCommand("wrong", "5678");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Verify("wrong", "currenthash")).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("PIN atual incorreto", result.Error.Message);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAppConfigNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new ChangePinCommand("1234", "5678");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppConfig)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("não encontrada", result.Error.Message);
    }

    [Fact]
    public async Task Handle_ShouldHashNewPin()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("currenthash", now);

        var command = new ChangePinCommand("1234", "9999");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Verify("1234", "currenthash")).Returns(true);
        _mockPinHasher.Setup(x => x.Hash("9999")).Returns("newhashed9999");
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockPinHasher.Verify(x => x.Hash("9999"), Times.Once);
    }
}
