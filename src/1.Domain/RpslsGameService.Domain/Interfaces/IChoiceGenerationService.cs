using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Interfaces;

public interface IChoiceGenerationService
{
    Choice GenerateComputerChoice(int randomNumber);
}