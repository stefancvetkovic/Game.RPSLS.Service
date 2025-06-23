using Microsoft.VisualStudio.TestTools.UnitTesting;
using RpslsGameService.Domain.Services;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Tests.Services;

[TestClass]
public class ChoiceGenerationServiceTests
{
    private ChoiceGenerationService _sut;

    [TestInitialize]
    public void Setup()
    {
        _sut = new ChoiceGenerationService();
    }

    [TestMethod]
    public void GenerateComputerChoice_WithNegativeNumber_ShouldReturnValidChoice()
    {
        // Arrange
        int negativeRandomNumber = -1;

        // Act
        var result = _sut.GenerateComputerChoice(negativeRandomNumber);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id >= 1 && result.Id <= 5);
    }

    [TestMethod]
    public void GenerateComputerChoice_WithZero_ShouldReturnValidChoice()
    {
        // Arrange
        int zeroRandomNumber = 0;

        // Act
        var result = _sut.GenerateComputerChoice(zeroRandomNumber);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id >= 1 && result.Id <= 5);
    }

    [TestMethod]
    public void GenerateComputerChoice_WithPositiveNumbers_ShouldReturnValidChoices()
    {
        // Arrange & Act & Assert
        for (int i = 1; i <= 100; i++)
        {
            var result = _sut.GenerateComputerChoice(i);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id >= 1 && result.Id <= 5, $"Failed for input {i}, got ID {result.Id}");
        }
    }

    [TestMethod]
    public void GenerateComputerChoice_WithVeryLargeNegativeNumber_ShouldReturnValidChoice()
    {
        // Arrange
        int largeNegativeNumber = -12345;

        // Act
        var result = _sut.GenerateComputerChoice(largeNegativeNumber);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id >= 1 && result.Id <= 5);
    }
}