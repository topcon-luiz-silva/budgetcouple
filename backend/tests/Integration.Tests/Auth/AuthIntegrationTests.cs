namespace BudgetCouple.Integration.Tests.Auth;

using System.Net;
using System.Net.Http.Json;
using BudgetCouple.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AuthIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Fact]
    public async Task GetAuthStatus_WithoutPinConfigured_ShouldReturn200WithNotConfiguredStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/auth/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsAsync<dynamic>();
        Assert.False((bool)content.pinConfigured);
        Assert.False((bool)content.locked);
    }

    [Fact]
    public async Task SetupPin_WithValidPin_ShouldReturn200AndAuthResult()
    {
        // Arrange
        var setupRequest = new { pin = "1234" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsAsync<dynamic>();
        Assert.NotNull(content.token);
        Assert.NotNull(content.expiresAt);
    }

    [Fact]
    public async Task Login_WithValidPin_ShouldReturn200AndAuthResult()
    {
        // Arrange - Setup PIN first
        var setupRequest = new { pin = "1234" };
        await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);

        var loginRequest = new { pin = "1234" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsAsync<dynamic>();
        Assert.NotNull(content.token);
    }

    [Fact]
    public async Task Login_WithInvalidPin_ShouldReturn401()
    {
        // Arrange - Setup PIN first
        var setupRequest = new { pin = "1234" };
        await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);

        var loginRequest = new { pin = "wrong" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SetupPin_WhenAlreadyConfigured_ShouldReturn409Conflict()
    {
        // Arrange - Setup PIN first
        var setupRequest = new { pin = "1234" };
        await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);

        // Try to setup again
        var secondSetupRequest = new { pin = "5678" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", secondSetupRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_After5FailedAttempts_ShouldReturnLockedError()
    {
        // Arrange - Setup PIN
        var setupRequest = new { pin = "1234" };
        await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);

        var loginRequest = new { pin = "wrong" };

        // Act - Attempt login 5 times with wrong PIN
        HttpResponseMessage response = null;
        for (int i = 0; i < 5; i++)
        {
            response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        }

        // Assert - 5th attempt should return Unauthorized (locked)
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var content = await response.Content.ReadAsAsync<dynamic>();
        Assert.Contains("bloqueado", content.message.ToString().ToLower());
    }

    [Fact]
    public async Task ChangePin_WithValidCurrentPin_ShouldReturn200()
    {
        // Arrange - Setup PIN first
        var setupRequest = new { pin = "1234" };
        var setupResponse = await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);
        var setupContent = await setupResponse.Content.ReadAsAsync<dynamic>();
        var token = (string)setupContent.token;

        // Create new client with auth token
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var changeRequest = new { pinAtual = "1234", novoPin = "5678" };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/v1/auth/change-pin", changeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ChangePin_WithInvalidCurrentPin_ShouldReturn401()
    {
        // Arrange - Setup PIN first
        var setupRequest = new { pin = "1234" };
        var setupResponse = await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);
        var setupContent = await setupResponse.Content.ReadAsAsync<dynamic>();
        var token = (string)setupContent.token;

        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var changeRequest = new { pinAtual = "wrong", novoPin = "5678" };

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/v1/auth/change-pin", changeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthStatus_AfterLogin_ShouldShowPinConfigured()
    {
        // Arrange - Setup PIN first
        var setupRequest = new { pin = "1234" };
        await _client.PostAsJsonAsync("/api/v1/auth/setup-pin", setupRequest);

        // Act
        var response = await _client.GetAsync("/api/v1/auth/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsAsync<dynamic>();
        Assert.True((bool)content.pinConfigured);
    }
}
