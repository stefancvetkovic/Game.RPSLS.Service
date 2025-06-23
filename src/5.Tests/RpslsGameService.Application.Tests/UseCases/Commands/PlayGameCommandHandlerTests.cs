using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Application.CQRS.Commands;
using RpslsGameService.Domain.Entities;
using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Application.Tests.UseCases.Commands;

[TestClass]
public class PlayGameCommandHandlerTests
{
    private Mock<IGameLogicService> _gameLogicServiceMock;
    private Mock<IChoiceGenerationService> _choiceGenerationServiceMock;
    private Mock<IRandomNumberService> _randomNumberServiceMock;
    private Mock<IGameSessionRepository> _gameSessionRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private PlayGameCommandHandler _sut;

    [TestInitialize]
    public void TestInitialize()
    {
        _gameLogicServiceMock = new Mock<IGameLogicService>();
        _choiceGenerationServiceMock = new Mock<IChoiceGenerationService>();
        _randomNumberServiceMock = new Mock<IRandomNumberService>();
        _gameSessionRepositoryMock = new Mock<IGameSessionRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _sut = new PlayGameCommandHandler(
            _gameLogicServiceMock.Object,
            _choiceGenerationServiceMock.Object,
            _randomNumberServiceMock.Object,
            _gameSessionRepositoryMock.Object,
            _mapperMock.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidPlayerChoice_ShouldReturnGameResult()
    {
        // Arrange
        var playerChoice = Choice.FromId(1);
        var computerChoice = Choice.FromId(2);
        var gameResult = new GameResult(playerChoice, computerChoice, GameOutcome.Win);
        var command = new PlayGameCommand(1);
        var gameSession = new GameSession();
        var expectedResponse = new GameResultResponse { Player = 1, Computer = 2, Results = "win" };
        
        _randomNumberServiceMock
            .Setup(x => x.GetRandomNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
            
        _choiceGenerationServiceMock
            .Setup(x => x.GenerateComputerChoice(2))
            .Returns(computerChoice);
            
        _gameLogicServiceMock
            .Setup(x => x.DetermineWinner(playerChoice, computerChoice))
            .Returns(gameResult);
            
        _gameSessionRepositoryMock
            .Setup(x => x.GetCurrentSessionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameSession);
            
        _mapperMock
            .Setup(x => x.Map<GameResultResponse>(gameResult))
            .Returns(expectedResponse);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResponse.Player, result.Player);
        Assert.AreEqual(expectedResponse.Computer, result.Computer);
        Assert.AreEqual(expectedResponse.Results, result.Results);
        
        _randomNumberServiceMock.Verify(x => x.GetRandomNumberAsync(It.IsAny<CancellationToken>()), Times.Once);
        _choiceGenerationServiceMock.Verify(x => x.GenerateComputerChoice(2), Times.Once);
        _gameLogicServiceMock.Verify(x => x.DetermineWinner(playerChoice, computerChoice), Times.Once);
        _gameSessionRepositoryMock.Verify(x => x.GetCurrentSessionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _gameSessionRepositoryMock.Verify(x => x.SaveAsync(gameSession, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WhenRandomServiceFails_ShouldThrowException()
    {
        // Arrange
        var command = new PlayGameCommand(1);
        
        _randomNumberServiceMock
            .Setup(x => x.GetRandomNumberAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() => 
            _sut.Handle(command, CancellationToken.None));
    }
}