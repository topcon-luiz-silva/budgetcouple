namespace BudgetCouple.Application.UnitTests.Notifications;

using BudgetCouple.Application.Notifications.Commands;
using BudgetCouple.Application.Notifications.Dtos;
using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Domain.Notifications;
using Moq;
using Xunit;

public class AtualizarPreferenciasCommandTests
{
    [Fact]
    public async Task Handle_WithValidPreferences_ShouldUpdateAndReturn()
    {
        // Arrange
        var mockRepository = new Mock<INotificationPreferencesRepository>();
        var existingPreferences = NotificationPreferences.Create();

        mockRepository
            .Setup(x => x.GetOrCreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPreferences);

        var handler = new AtualizarPreferenciasHandler(mockRepository.Object);

        var newPrefs = new NotificationPreferencesDto
        {
            EmailHabilitado = true,
            EmailEndereco = "test@example.com",
            WebPushHabilitado = true,
            TelegramHabilitado = false,
            TelegramChatId = null,
            NotificarVencimentos1Dia = true,
            NotificarVencimentosDia = false,
            NotificarAlertasOrcamento = true,
            NotificarFaturas = false
        };

        var command = new AtualizarPreferenciasCommand(newPrefs);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.EmailHabilitado);
        Assert.Equal("test@example.com", result.EmailEndereco);
        Assert.True(result.WebPushHabilitado);
        Assert.False(result.TelegramHabilitado);

        mockRepository.Verify(x => x.UpdateAsync(It.IsAny<NotificationPreferences>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmailDisabled_ShouldClearEmail()
    {
        // Arrange
        var mockRepository = new Mock<INotificationPreferencesRepository>();
        var existingPreferences = NotificationPreferences.Create();

        mockRepository
            .Setup(x => x.GetOrCreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPreferences);

        var handler = new AtualizarPreferenciasHandler(mockRepository.Object);

        var newPrefs = new NotificationPreferencesDto
        {
            EmailHabilitado = false,
            EmailEndereco = null,
            WebPushHabilitado = false,
            TelegramHabilitado = false,
            TelegramChatId = null,
            NotificarVencimentos1Dia = true,
            NotificarVencimentosDia = true,
            NotificarAlertasOrcamento = true,
            NotificarFaturas = true
        };

        var command = new AtualizarPreferenciasCommand(newPrefs);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.EmailHabilitado);
        Assert.Null(result.EmailEndereco);
    }
}
