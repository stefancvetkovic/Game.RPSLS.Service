using Microsoft.VisualStudio.TestTools.UnitTesting;
using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Services;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Tests.Services;

[TestClass]
public class GameLogicServiceTests
{
    private GameLogicService _sut;

    [TestInitialize]
    public void TestInitialize()
    {
        _sut = new GameLogicService();
    }

    [TestMethod]
    public void DetermineWinner_RockVsScissors_ShouldReturnWin()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Rock);
        var computer = Choice.FromType(ChoiceType.Scissors);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Win, result.Outcome);
        Assert.AreEqual(player, result.PlayerChoice);
        Assert.AreEqual(computer, result.ComputerChoice);
    }

    [TestMethod]
    public void DetermineWinner_RockVsLizard_ShouldReturnWin()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Rock);
        var computer = Choice.FromType(ChoiceType.Lizard);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Win, result.Outcome);
        Assert.AreEqual(player, result.PlayerChoice);
        Assert.AreEqual(computer, result.ComputerChoice);
    }

    [TestMethod]
    public void DetermineWinner_PaperVsRock_ShouldReturnWin()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Paper);
        var computer = Choice.FromType(ChoiceType.Rock);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Win, result.Outcome);
    }

    [TestMethod]
    public void DetermineWinner_PaperVsSpock_ShouldReturnWin()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Paper);
        var computer = Choice.FromType(ChoiceType.Spock);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Win, result.Outcome);
    }

    [TestMethod]
    public void DetermineWinner_ScissorsVsPaper_ShouldReturnWin()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Scissors);
        var computer = Choice.FromType(ChoiceType.Paper);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Win, result.Outcome);
    }

    [TestMethod]
    public void DetermineWinner_RockVsRock_ShouldReturnTie()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Rock);
        var computer = Choice.FromType(ChoiceType.Rock);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Tie, result.Outcome);
    }

    [TestMethod]
    public void DetermineWinner_PaperVsPaper_ShouldReturnTie()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Paper);
        var computer = Choice.FromType(ChoiceType.Paper);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Tie, result.Outcome);
    }

    [TestMethod]
    public void DetermineWinner_RockVsPaper_ShouldReturnLose()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Rock);
        var computer = Choice.FromType(ChoiceType.Paper);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Lose, result.Outcome);
    }

    [TestMethod]
    public void DetermineWinner_ScissorsVsRock_ShouldReturnLose()
    {
        // Arrange
        var player = Choice.FromType(ChoiceType.Scissors);
        var computer = Choice.FromType(ChoiceType.Rock);

        // Act
        var result = _sut.DetermineWinner(player, computer);

        // Assert
        Assert.AreEqual(GameOutcome.Lose, result.Outcome);
    }

    [TestMethod]
    public void GetAllChoices_ShouldReturnAllFiveChoices()
    {
        // Act
        var choices = _sut.GetAllChoices();

        // Assert
        Assert.AreEqual(5, choices.Count);
        Assert.IsTrue(choices.Any(c => c.Type == ChoiceType.Rock));
        Assert.IsTrue(choices.Any(c => c.Type == ChoiceType.Paper));
        Assert.IsTrue(choices.Any(c => c.Type == ChoiceType.Scissors));
        Assert.IsTrue(choices.Any(c => c.Type == ChoiceType.Lizard));
        Assert.IsTrue(choices.Any(c => c.Type == ChoiceType.Spock));
    }
}