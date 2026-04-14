namespace BudgetCouple.Domain.UnitTests.Identity;

using BudgetCouple.Domain.Identity;
using Xunit;

public class AppConfigTests
{
    [Fact]
    public void RegistrarFalha_QuintaVez_DeveBloquear()
    {
        // Arrange
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("dummyhash", DateTime.UtcNow);
        var now = DateTime.UtcNow;

        // Act
        for (int i = 0; i < 5; i++)
        {
            appConfig.RegistrarFalha(now);
        }

        // Assert
        Assert.Equal(5, appConfig.FailedAttempts);
        Assert.NotNull(appConfig.LockedUntil);
        Assert.True(appConfig.LockedUntil > now);
        Assert.True(appConfig.EstaBloqueado(now.AddMinutes(1)));
    }

    [Fact]
    public void ResetarFalhas_DeveLimparContadorEBloqueio()
    {
        // Arrange
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("dummyhash", DateTime.UtcNow);
        var now = DateTime.UtcNow;

        // Simulate 5 failed attempts
        for (int i = 0; i < 5; i++)
        {
            appConfig.RegistrarFalha(now);
        }

        // Act
        appConfig.ResetarFalhas();

        // Assert
        Assert.Equal(0, appConfig.FailedAttempts);
        Assert.Null(appConfig.LockedUntil);
        Assert.False(appConfig.EstaBloqueado(now));
    }

    [Fact]
    public void ConfigurarPin_DeveRetornarConflitQuandoJaConfigrado()
    {
        // Arrange
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("firsthash", DateTime.UtcNow);

        // Act
        var result = appConfig.ConfigurarPin("secondhash", DateTime.UtcNow);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Conflict", result.Error.Code);
    }

    [Fact]
    public void TentativasRestantes_DeveRetornarValorCorreto()
    {
        // Arrange
        var appConfig = AppConfig.Empty();
        appConfig.ConfigurarPin("dummyhash", DateTime.UtcNow);
        var now = DateTime.UtcNow;

        // Act & Assert
        Assert.Equal(5, appConfig.TentativasRestantes());

        appConfig.RegistrarFalha(now);
        Assert.Equal(4, appConfig.TentativasRestantes());

        appConfig.RegistrarFalha(now);
        Assert.Equal(3, appConfig.TentativasRestantes());
    }
}
