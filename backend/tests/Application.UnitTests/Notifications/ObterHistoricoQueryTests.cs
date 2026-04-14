namespace BudgetCouple.Application.UnitTests.Notifications;

using BudgetCouple.Application.Notifications.Dtos;
using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Application.Notifications.Queries;
using BudgetCouple.Domain.Notifications;
using Moq;
using Xunit;

public class ObterHistoricoQueryTests
{
    [Fact]
    public async Task Handle_WithExistingHistory_ShouldReturnRecentNotifications()
    {
        // Arrange
        var mockRepository = new Mock<INotificationHistoryRepository>();

        var historyItems = new List<NotificationHistory>
        {
            NotificationHistory.Create(
                NotificationChannel.Email,
                NotificationType.VencimentoProximo,
                "Test 1",
                "Body 1",
                NotificationStatus.Success
            ),
            NotificationHistory.Create(
                NotificationChannel.WebPush,
                NotificationType.AlertaOrcamento,
                "Test 2",
                "Body 2",
                NotificationStatus.Failed,
                "Error message"
            )
        };

        mockRepository
            .Setup(x => x.GetRecentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(historyItems);

        var handler = new ObterHistoricoHandler(mockRepository.Object);
        var query = new ObterHistoricoQuery(20);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(NotificationChannel.Email, result[0].Canal);
        Assert.Equal(NotificationStatus.Success, result[0].Status);
        Assert.Equal(NotificationChannel.WebPush, result[1].Canal);
        Assert.Equal(NotificationStatus.Failed, result[1].Status);

        mockRepository.Verify(x => x.GetRecentAsync(20, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoHistory_ShouldReturnEmptyList()
    {
        // Arrange
        var mockRepository = new Mock<INotificationHistoryRepository>();

        mockRepository
            .Setup(x => x.GetRecentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationHistory>());

        var handler = new ObterHistoricoHandler(mockRepository.Object);
        var query = new ObterHistoricoQuery(20);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithCustomLimit_ShouldRespectLimit()
    {
        // Arrange
        var mockRepository = new Mock<INotificationHistoryRepository>();

        mockRepository
            .Setup(x => x.GetRecentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationHistory>());

        var handler = new ObterHistoricoHandler(mockRepository.Object);
        var query = new ObterHistoricoQuery(50);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        mockRepository.Verify(x => x.GetRecentAsync(50, It.IsAny<CancellationToken>()), Times.Once);
    }
}
