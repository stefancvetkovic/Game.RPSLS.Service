using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.CQRS.Commands;
using RpslsGameService.Application.CQRS.Queries;
using RpslsGameService.Domain;

namespace RpslsGameService.Api.Controllers;

/// <summary>
/// Game controller for Rock, Paper, Scissors, Lizard, Spock game operations
/// </summary>
[ApiController]
[Route("")]
[Produces("application/json")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GameController> _logger;

    public GameController(IMediator mediator, ILogger<GameController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all available game choices
    /// </summary>
    /// <returns>List of available choices</returns>
    /// <response code="200">Returns the list of choices</response>
    [HttpGet(Constants.ApiRoutes.Choices)]
    [ProducesResponseType(typeof(IReadOnlyList<ChoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ChoiceDto>>> GetChoices(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting all game choices");
        
        var query = new GetChoicesQuery();
        var choices = await _mediator.Send(query, cancellationToken);
        
        return Ok(choices);
    }

    /// <summary>
    /// Get a random computer choice
    /// </summary>
    /// <returns>A random choice for the computer</returns>
    /// <response code="200">Returns a random choice</response>
    [HttpGet(Constants.ApiRoutes.Choice)]
    [ProducesResponseType(typeof(ChoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ChoiceDto>> GetRandomChoice(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting random computer choice");
        
        var query = new GetRandomChoiceQuery();
        var choice = await _mediator.Send(query, cancellationToken);
        
        return Ok(choice);
    }

    /// <summary>
    /// Play a game round
    /// </summary>
    /// <param name="request">Player's choice (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Game result</returns>
    /// <response code="200">Returns the game result</response>
    /// <response code="400">Invalid player choice</response>
    [HttpPost(Constants.ApiRoutes.Play)]
    [ProducesResponseType(typeof(GameResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GameResultResponse>> PlayGame(
        [FromBody] PlayGameRequest request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Playing game round with player choice: {PlayerChoice}", request.Player);
        
        var command = new PlayGameCommand(request.Player);
        var result = await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation("Game round completed. Result: {Result}", result.Results);
        
        return Ok(result);
    }

    /// <summary>
    /// Get game history and statistics
    /// </summary>
    /// <returns>Game history with statistics</returns>
    /// <response code="200">Returns game history and stats</response>
    [HttpGet(Constants.ApiRoutes.History)]
    [ProducesResponseType(typeof(GameHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GameHistoryResponse>> GetGameHistory(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting game history");
        
        var query = new GetGameHistoryQuery();
        var history = await _mediator.Send(query, cancellationToken);
        
        return Ok(history);
    }

    /// <summary>
    /// Reset game statistics and history
    /// </summary>
    /// <returns>Success confirmation</returns>
    /// <response code="204">Game statistics reset successfully</response>
    [HttpDelete(Constants.ApiRoutes.Reset)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetGame(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting game statistics");
        
        var command = new ResetScoreboardCommand();
        await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation("Game statistics reset successfully");
        
        return NoContent();
    }
}