using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Domain.Services;

/// <summary>
/// Service responsible for generating computer choices in the RPSLS game.
/// Service converts random numbers into valid game choices using an optimized logic.
/// 
/// Uses cached Choice instances and optimized logic for better performance.
/// </summary>
public class ChoiceGenerationService : IChoiceGenerationService
{
    private static readonly Choice[] ChoiceCache = {
        Choice.Rock,
        Choice.Paper,
        Choice.Scissors,
        Choice.Lizard,
        Choice.Spock
    };

    /// <summary>
    /// Example mappings:
    /// - Input: 1-5 → Output: 1-5 (fast path, direct cache access)
    /// - Input: 6-10 → Output: 1-5 (moduo math)
    /// - Input: 73 → Output: 3 (73-1=72, 72%5=2, cache[2]=Scissors)
    /// </summary>
    /// <param name="randomNumber">
    /// Any positive integer from an external random source.
    /// </param>
    /// <returns>
    /// A cached Choice object representing one of the five game options:
    /// 1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock
    /// </returns>
    public Choice GenerateComputerChoice(int randomNumber)
    {
        // Its not stated in the task, but in any case if 3rd party decides to insert also negative numbers, than we will use Absolute value
        if (randomNumber < 0)
        {
            randomNumber = Math.Abs(randomNumber);
        }
        
        // Fast path: if number is already in valid range 1-5, use direct cache access
        if (randomNumber >= 1 && randomNumber <= 5)
        {
            return ChoiceCache[randomNumber - 1];
        }
        
        // For numbers outside 1-5, use optimized moduo math and map it accordingly to the array
        var index = (randomNumber - 1) % 5;
        return ChoiceCache[index];
    }
}