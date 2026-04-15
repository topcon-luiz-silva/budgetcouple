namespace BudgetCouple.Application.UnitTests.Identity.Commands;

using BudgetCouple.Application.Auth.Commands.SetupPin;
using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Identity;
using Moq;
using Xunit;

public class SetupPinCommandHandlerTests
{
    private readonly Mock<IAppConfigRepository> _mockRepository;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<IPinHasher> _mockPinHasher;
    private readonly Mock<IJwtTokenService> _mockJwtService;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly SetupPinCommandHandler _handler;

    public SetupPinCommandHandlerTests()
    {
        _mockRepository = new Mock<IAppConfigRepository>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockPinHasher = new Mock<IPinHasher>();
        _mockJwtService = new Mock<IJwtTokenService>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _handler = new SetupPinCommandHandler(
            _mockRepository.Object,
            _mockDbContext.Object,
            _mockPinHasher.Object,
            _mockJwtService.Object,
            _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_WhenAppConfigDoesNotExist_ShouldCreateAndReturnAuthResult()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var command = new SetupPinCommand("1234");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppConfig)null);
        _mockPinHasher.Setup(x => x.Hash("1234")).Returns("hashedpin");
        _mockJwtService.Setup(x => x.GenerateToken()).Returns("jwt-token");
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("jwt-token", result.Value.Token);
        Assert.Equal(now.AddDays(30), result.Value.ExpiresAt);
        _mockRepository.Verify(x => x.Add(It.IsAny<AppConfig>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAppConfigExists_ShouldUpdateAndReturnAuthResult()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        var command = new SetupPinCommand("5678");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Hash("5678")).Returns("hashedpin2");
        _mockJwtService.Setup(x => x.GenerateToken()).Returns("jwt-token-2");
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("jwt-token-2", result.Value.Token);
        _mockRepository.Verify(x => x.Add(It.IsAny<AppConfig>()), Times.Never);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPinAlreadyConfigured_ShouldReturnConflictError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("existinghash", now);

        var command = new SetupPinCommand("newpin");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Hash("newpin")).Returns("newhash");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Conflict", result.Error.Code);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldGenerateJwtWithCorrectExpiration()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var command = new SetupPinCommand("1234");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppConfig)null);
        _mockPinHasher.Setup(x => x.Hash("1234")).Returns("hashedpin");
        _mockJwtService.Setup(x => x.GenerateToken()).Returns("jwt-token");
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var expectedExpiration = now.AddDays(30);
        Assert.Equal(expectedExpiration, result.Value.ExpiresAt);
    }
}
