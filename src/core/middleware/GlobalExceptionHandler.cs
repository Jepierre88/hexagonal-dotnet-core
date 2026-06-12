using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Core.Middleware;

/// <summary>
/// Centraliza la traducción de excepciones de dominio a respuestas HTTP.
/// Los use cases lanzan las excepciones; este handler las convierte en status codes,
/// sin necesidad de try/catch en cada controller.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException    => (StatusCodes.Status404NotFound,    "Recurso no encontrado"),
            InvalidOperationException => (StatusCodes.Status409Conflict,  "Conflicto de negocio"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Acceso denegado"),
            ArgumentException       => (StatusCodes.Status400BadRequest,  "Solicitud inválida"),
            _                       => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
