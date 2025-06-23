using Microsoft.VisualStudio.TestTools.UnitTesting;
using RpslsGameService.Application.CQRS.Commands;
using RpslsGameService.Application.Validators;

namespace RpslsGameService.Application.Tests.Validators;

[TestClass]
public class PlayGameCommandValidatorTests
{
    private PlayGameCommandValidator _sut;

    [TestInitialize]
    public void TestInitialize()
    {
        _sut = new PlayGameCommandValidator();
    }

    [TestMethod]
    public void Validate_WithValidChoice1_ShouldPass()
    {
        // Arrange
        var command = new PlayGameCommand(1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_WithValidChoice5_ShouldPass()
    {
        // Arrange
        var command = new PlayGameCommand(5);

        // Act
        var result = _sut.Validate(command);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_WithInvalidChoice0_ShouldFail()
    {
        // Arrange
        var command = new PlayGameCommand(0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("PlayerChoice", result.Errors[0].PropertyName);
    }

    [TestMethod]
    public void Validate_WithInvalidChoice6_ShouldFail()
    {
        // Arrange
        var command = new PlayGameCommand(6);

        // Act
        var result = _sut.Validate(command);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("PlayerChoice", result.Errors[0].PropertyName);
    }
}