namespace BudgetCouple.Application.UnitTests.Identity.Commands;

using BudgetCouple.Application.Auth.Commands.Login;
using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Identity;
using Moq;
using Xunit;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAppConfigRepository> _mockRepository;
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<IPinHasher> _mockPinHasher;
    private readonly Mock<IJwtTokenService> _mockJwtService;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockRepository = new Mock<IAppConfigRepository>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockPinHasher = new Mock<IPinHasher>();
        _mockJwtService = new Mock<IJwtTokenService>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _handler = new LoginCommandHandler(
            _mockRepository.Object,
            _mockDbContext.Object,
            _mockPinHasher.Object,
            _mockJwtService.Object,
            _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_WithValidPin_ShouldReturnAuthResult()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("hashedpin", now);

        var command = new LoginCommand("1234");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Verify("1234", "hashedpin")).Returns(true);
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
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithIncorrectPin_ShouldReturnFailureAndIncrementAttempts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("hashedpin", now);

        var command = new LoginCommand("wrong");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Verify("wrong", "hashedpin")).Returns(false);
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("PIN incorreto", result.Error.Message);
        Assert.Contains("4", result.Error.Message); // 4 remaining attempts
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountLocked_ShouldReturnFailureWithLockoutError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("hashedpin", now);

        // Lock the account
        for (int i = 0; i < 5; i++)
        {
            appConfig.RegistrarFalha(now);
        }

        var command = new LoginCommand("1234");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("bloqueado", result.Error.Message.ToLower());
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPinNotConfigured_ShouldReturnFailure()
    {
        // Arrange
        var appConfig = AppConfig.Empty(); // No PIN configured
        var command = new LoginCommand("1234");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("não configurado", result.Error.Message);
    }

    [Fact]
    public async Task Handle_WhenAppConfigNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand("1234");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppConfig)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("não configurado", result.Error.Message);
    }

    [Fact]
    public async Task Handle_After5FailedAttempts_ShouldLockAccount()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("hashedpin", now);

        var command = new LoginCommand("wrong");

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);
        _mockPinHasher.Setup(x => x.Verify("wrong", "hashedpin")).Returns(false);
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act - Simulate 5 failed attempts
        for (int i = 0; i < 5; i++)
        {
            await _handler.Handle(command, CancellationToken.None);
        }

        // Assert - Account should be locked on 5th attempt
        Assert.NotNull(appConfig.LockedUntil);
        Assert.True(appConfig.EstaBloqueado(now));
    }
}
