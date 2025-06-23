using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetArchTest.Rules;
using System.Reflection;

namespace RpslsGameService.ArchitectureTests;

[TestClass]
public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(RpslsGameService.Domain.Entities.Entity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(RpslsGameService.Application.DTOs.ChoiceDto).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(RpslsGameService.Infrastructure.Caching.ICacheService).Assembly;
    private static readonly Assembly PresentationAssembly = typeof(RpslsGameService.Api.Controllers.GameController).Assembly;

    [TestMethod]
    public void Domain_Should_Not_Have_Dependencies_On_Other_Layers()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny("Application", "Infrastructure", "Api")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "Domain layer should not depend on other layers");
    }

    [TestMethod]
    public void Application_Should_Not_Have_Dependencies_On_Infrastructure_Or_Presentation()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny("Infrastructure", "Api")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "Application layer should not depend on Infrastructure or Presentation layers");
    }

    [TestMethod]
    public void Infrastructure_Should_Not_Have_Dependencies_On_Presentation()
    {
        // Arrange & Act
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn("Api")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "Infrastructure layer should not depend on Presentation layer");
    }

    [TestMethod]
    public void Entities_Should_Be_In_Domain_Layer()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("RpslsGameService.Domain.Entities")
            .Should()
            .BeClasses()
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "All entities should be in the Domain layer");
    }

    [TestMethod]
    public void ValueObjects_Should_Be_In_Domain_Layer()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("RpslsGameService.Domain.Models")
            .Should()
            .BeClasses()
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "All value objects should be in the Domain layer");
    }

    [TestMethod]
    public void Controllers_Should_Be_In_Presentation_Layer()
    {
        // Arrange & Act
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace("RpslsGameService.Api.Controllers")
            .Should()
            .BeClasses()
            .And()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "All controllers should be in the Presentation layer");
    }

    [TestMethod]
    public void CommandHandlers_Should_Be_In_Application_Layer()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .ResideInNamespace("RpslsGameService.Application.CQRS.Commands")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "All command handlers should be in the Application layer");
    }

    [TestMethod]
    public void QueryHandlers_Should_Be_In_Application_Layer()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .ResideInNamespace("RpslsGameService.Application.CQRS.Queries")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "All query handlers should be in the Application layer");
    }

    [TestMethod]
    public void Validators_Should_Be_In_Application_Layer()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Validator")
            .Should()
            .ResideInNamespace("RpslsGameService.Application.Validators")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "All validators should be in the Application layer");
    }

    [TestMethod]
    public void Domain_Services_Should_Not_Have_Infrastructure_Dependencies()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("RpslsGameService.Domain.Services")
            .Should()
            .NotHaveDependencyOnAny("System.Net.Http", "Microsoft.Extensions.Http")
            .GetResult();

        // Assert
        Assert.IsTrue(result.IsSuccessful, "Domain services should not have infrastructure dependencies");
    }
}