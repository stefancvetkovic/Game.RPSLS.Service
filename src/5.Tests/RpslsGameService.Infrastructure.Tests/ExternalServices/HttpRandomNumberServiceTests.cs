using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using RpslsGameService.Infrastructure.Configuration;
using RpslsGameService.Infrastructure.ExternalServices;
using System.Net;

namespace RpslsGameService.Infrastructure.Tests.ExternalServices;

[TestClass]
public class HttpRandomNumberServiceTests : IDisposable
{
    private Mock<IOptions<ExternalApiSettings>> _optionsMock;
    private Mock<ILogger<HttpRandomNumberService>> _loggerMock;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private ExternalApiSettings _settings;
    private HttpRandomNumberService _sut;

    [TestInitialize]
    public void TestInitialize()
    {
        _settings = new ExternalApiSettings
        {
            RandomNumberService = new RandomNumberServiceSettings
            {
                BaseUrl = "https://api.example.com",
                TimeoutSeconds = 30,
                EnableFallback = true
            }
        };
        
        _optionsMock = new Mock<IOptions<ExternalApiSettings>>();
        _optionsMock.Setup(x => x.Value).Returns(_settings);
        
        _loggerMock = new Mock<ILogger<HttpRandomNumberService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(_settings.RandomNumberService.BaseUrl)
        };
        
        _sut = new HttpRandomNumberService(_httpClient, _optionsMock.Object, _loggerMock.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    [TestMethod]
    public async Task GetRandomNumberAsync_WhenApiReturnsValidResponse_ShouldReturnNumber()
    {
        // Arrange
        var expectedNumber = 3;
        var plainTextResponse = expectedNumber.ToString();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(plainTextResponse, System.Text.Encoding.UTF8, "text/plain")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _sut.GetRandomNumberAsync();

        // Assert
        Assert.AreEqual(expectedNumber, result);
    }

    [TestMethod]
    public async Task GetRandomNumberAsync_WhenApiReturnsError_ShouldReturnFallbackNumber()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _sut.GetRandomNumberAsync();

        // Assert
        Assert.IsTrue(result >= 1 && result <= 100, "Fallback number should be between 1 and 100");
    }

    [TestMethod]
    public async Task GetRandomNumberAsync_WithCancellationToken_ShouldReturnFallbackOnCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        // Act
        var result = await _sut.GetRandomNumberAsync(cts.Token);

        // Assert
        Assert.IsTrue(result >= 1 && result <= 100, "Should return fallback number when request is cancelled");
    }

    [TestMethod]
    public async Task GetRandomNumberAsync_WhenFallbackDisabled_ShouldThrowException()
    {
        // Arrange
        var settingsWithFallbackDisabled = new ExternalApiSettings
        {
            RandomNumberService = new RandomNumberServiceSettings
            {
                BaseUrl = "https://api.example.com",
                TimeoutSeconds = 30,
                EnableFallback = false
            }
        };
        
        var optionsMock = new Mock<IOptions<ExternalApiSettings>>();
        optionsMock.Setup(x => x.Value).Returns(settingsWithFallbackDisabled);
        
        var sut = new HttpRandomNumberService(_httpClient, optionsMock.Object, _loggerMock.Object);
        
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => 
            sut.GetRandomNumberAsync());
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}