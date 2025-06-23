using FluentValidation;
using RpslsGameService.Domain;
using RpslsGameService.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace RpslsGameService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred: {ExceptionMessage}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message) = GetErrorResponse(exception);
        response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            error = new
            {
                message,
                type = exception.GetType().Name,
                timestamp = DateTime.UtcNow
            }
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private static (HttpStatusCode statusCode, string message) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))
            ),
            InvalidChoiceException => (
                HttpStatusCode.BadRequest,
                exception.Message
            ),
            DomainException => (
                HttpStatusCode.BadRequest,
                exception.Message
            ),
            ArgumentNullException => (
                HttpStatusCode.BadRequest,
                Constants.ValidationMessages.RequiredParameterMissing
            ),
            ArgumentException => (
                HttpStatusCode.BadRequest,
                exception.Message
            ),
            InvalidOperationException => (
                HttpStatusCode.BadRequest,
                exception.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                Constants.ValidationMessages.InternalServerError
            )
        };
    }
}