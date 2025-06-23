using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RpslsGameService.Api.Controllers;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.CQRS.Commands;
using RpslsGameService.Application.CQRS.Queries;

namespace RpslsGameService.Api.Tests.Controllers;

[TestClass]
public class GameControllerTests
{
    private Mock<IMediator> _mediatorMock;
    private Mock<ILogger<GameController>> _loggerMock;
    private GameController _sut;

    [TestInitialize]
    public void TestInitialize()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<GameController>>();
        _sut = new GameController(_mediatorMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetChoices_ShouldReturnOkWithChoices()
    {
        // Arrange
        var choices = new List<ChoiceDto>
        {
            new ChoiceDto { Id = 1, Name = "rock" },
            new ChoiceDto { Id = 2, Name = "paper" }
        };
        
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetChoicesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(choices);

        // Act
        var result = await _sut.GetChoices(CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.AreEqual(choices, okResult.Value);
    }

    [TestMethod]
    public async Task GetRandomChoice_ShouldReturnOkWithChoice()
    {
        // Arrange
        var choice = new ChoiceDto { Id = 3, Name = "scissors" };
        
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetRandomChoiceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(choice);

        // Act
        var result = await _sut.GetRandomChoice(CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.AreEqual(choice, okResult.Value);
    }

    [TestMethod]
    public async Task PlayGame_WithValidRequest_ShouldReturnOkWithResult()
    {
        // Arrange
        var request = new PlayGameRequest { Player = 1 };
        var response = new GameResultResponse { Player = 1, Computer = 2, Results = "win" };
        
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<PlayGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.PlayGame(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.AreEqual(response, okResult.Value);
        
        _mediatorMock.Verify(
            x => x.Send(It.Is<PlayGameCommand>(cmd => cmd.PlayerChoice == request.Player), 
                       It.IsAny<CancellationToken>()), 
            Times.Once);
    }

}