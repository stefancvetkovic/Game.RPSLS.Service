using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RpslsGameService.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RpslsGameService.IntegrationTests;

[TestClass]
public class GameApiIntegrationTests
{
    private static WebApplicationFactory<Program> _factory;
    private static HttpClient _client;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task GetChoices_ShouldReturnAllChoices()
    {
        // Act
        var response = await _client.GetAsync("/api/game/choices");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var choices = JsonSerializer.Deserialize<ChoiceDto[]>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.IsNotNull(choices);
        Assert.AreEqual(5, choices.Length);
        Assert.IsTrue(choices.Any(c => c.Name.Equals("rock", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(choices.Any(c => c.Name.Equals("paper", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(choices.Any(c => c.Name.Equals("scissors", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(choices.Any(c => c.Name.Equals("lizard", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(choices.Any(c => c.Name.Equals("spock", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task GetRandomChoice_ShouldReturnValidChoice()
    {
        // Act
        var response = await _client.GetAsync("/api/game/choice");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var choice = JsonSerializer.Deserialize<ChoiceDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.IsNotNull(choice);
        Assert.IsTrue(choice.Id >= 1 && choice.Id <= 5);
        Assert.IsTrue(new[] { "rock", "paper", "scissors", "lizard", "spock" }.Contains(choice.Name.ToLower()));
    }

    [TestMethod]
    public async Task PlayGame_WithValidChoice_ShouldReturnGameResult()
    {
        // Arrange
        var request = new PlayGameRequest { Player = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/game/play", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GameResultResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Player);
        Assert.IsTrue(result.Computer >= 1 && result.Computer <= 5);
        Assert.IsTrue(new[] { "win", "lose", "tie" }.Contains(result.Results));
    }

    [TestMethod]
    public async Task PlayGame_WithInvalidChoice_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PlayGameRequest { Player = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/game/play", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

}