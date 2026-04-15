namespace BudgetCouple.Application.UnitTests.Identity.Queries;

using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Auth.Queries.GetAuthStatus;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Identity;
using Moq;
using Xunit;

public class GetAuthStatusQueryHandlerTests
{
    private readonly Mock<IAppConfigRepository> _mockRepository;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly GetAuthStatusQueryHandler _handler;

    public GetAuthStatusQueryHandlerTests()
    {
        _mockRepository = new Mock<IAppConfigRepository>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _handler = new GetAuthStatusQueryHandler(
            _mockRepository.Object,
            _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_WhenPinConfiguredAndNotLocked_ShouldReturnCorrectStatus()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("hashedpin", now);

        var query = new GetAuthStatusQuery();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.PinConfigured);
        Assert.False(result.Locked);
        Assert.Null(result.LockedUntil);
    }

    [Fact]
    public async Task Handle_WhenPinNotConfigured_ShouldReturnNotConfiguredStatus()
    {
        // Arrange
        var appConfig = AppConfig.Empty(); // No PIN configured

        var query = new GetAuthStatusQuery();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.PinConfigured);
        Assert.False(result.Locked);
    }

    [Fact]
    public async Task Handle_WhenAccountIsLocked_ShouldReturnLockedStatus()
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

        var query = new GetAuthStatusQuery();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.PinConfigured);
        Assert.True(result.Locked);
        Assert.NotNull(result.LockedUntil);
        Assert.True(result.LockedUntil > now);
    }

    [Fact]
    public async Task Handle_WhenLockoutExpired_ShouldReturnNotLockedStatus()
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

        var futureTime = now.AddMinutes(20); // After 15-minute lockout

        var query = new GetAuthStatusQuery();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(futureTime);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.PinConfigured);
        Assert.False(result.Locked); // Lockout has expired
        Assert.NotNull(result.LockedUntil); // But LockedUntil still has the value
    }

    [Fact]
    public async Task Handle_WhenAppConfigNotFound_ShouldReturnDefaultNotConfiguredStatus()
    {
        // Arrange
        var query = new GetAuthStatusQuery();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppConfig)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.PinConfigured);
        Assert.False(result.Locked);
        Assert.Null(result.LockedUntil);
    }

    [Fact]
    public async Task Handle_WithPartialFailedAttempts_ShouldReturnNotLockedButAttemptCountTracked()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("hashedpin", now);

        // Register 2 failed attempts (not locked yet)
        appConfig.RegistrarFalha(now);
        appConfig.RegistrarFalha(now);

        var query = new GetAuthStatusQuery();

        _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);
        _mockRepository.Setup(x => x.GetSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appConfig);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.PinConfigured);
        Assert.False(result.Locked); // Not locked with only 2 attempts
        Assert.Null(result.LockedUntil); // No lockout timestamp yet
        Assert.Equal(2, appConfig.FailedAttempts);
    }
}
