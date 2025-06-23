using Microsoft.VisualStudio.TestTools.UnitTesting;
using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Tests.ValueObjects;

[TestClass]
public class ChoiceTests
{
    [TestMethod]
    public void FromType_WithRock_ShouldCreateRockChoice()
    {
        // Act
        var choice = Choice.FromType(ChoiceType.Rock);

        // Assert
        Assert.IsNotNull(choice);
        Assert.AreEqual(ChoiceType.Rock, choice.Type);
        Assert.AreEqual("Rock", choice.Name);
    }

    [TestMethod]
    public void FromType_WithPaper_ShouldCreatePaperChoice()
    {
        // Act
        var choice = Choice.FromType(ChoiceType.Paper);

        // Assert
        Assert.IsNotNull(choice);
        Assert.AreEqual(ChoiceType.Paper, choice.Type);
        Assert.AreEqual("Paper", choice.Name);
    }

    [TestMethod]
    public void FromType_WithInvalidChoiceType_ShouldThrowException()
    {
        // Arrange
        var invalidChoice = (ChoiceType)999;

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => Choice.FromType(invalidChoice));
    }

    [TestMethod]
    public void Equals_WithSameChoice_ShouldReturnTrue()
    {
        // Arrange
        var choice1 = Choice.FromType(ChoiceType.Rock);
        var choice2 = Choice.FromType(ChoiceType.Rock);

        // Act & Assert
        Assert.AreEqual(choice1, choice2);
        Assert.IsTrue(choice1 == choice2);
    }

    [TestMethod]
    public void Equals_WithDifferentChoice_ShouldReturnFalse()
    {
        // Arrange
        var choice1 = Choice.FromType(ChoiceType.Rock);
        var choice2 = Choice.FromType(ChoiceType.Paper);

        // Act & Assert
        Assert.AreNotEqual(choice1, choice2);
        Assert.IsTrue(choice1 != choice2);
    }

    [TestMethod]
    public void GetHashCode_ForSameChoice_ShouldBeEqual()
    {
        // Arrange
        var choice1 = Choice.FromType(ChoiceType.Spock);
        var choice2 = Choice.FromType(ChoiceType.Spock);

        // Act & Assert
        Assert.AreEqual(choice1.GetHashCode(), choice2.GetHashCode());
    }
}