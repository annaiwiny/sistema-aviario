using FarmSystemProject.DTOs;
using FarmSystemProject.Exceptions;
using System.Net;

namespace FarmSystemProject.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string error;
        string message;

        switch (exception)
        {
            case BusinessException:
                status = HttpStatusCode.BadRequest;
                error = "Bad Request";
                message = exception.Message;
                break;

            case NotFoundException:
                status = HttpStatusCode.NotFound;
                error = "Not Found";
                message = exception.Message;
                break;

            case UnauthorizedException:
                status = HttpStatusCode.Unauthorized;
                error = "Unauthorized";
                message = exception.Message;
                break;

            default:
                _logger.LogError(exception, "Erro não tratado");
                status = HttpStatusCode.InternalServerError;
                error = "Internal Server Error";
                message = exception.ToString(); // Exibe o erro completo para debug
                break;
        }

        var response = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow,
            Status = (int)status,
            Error = error,
            Message = message,
            Path = context.Request.Path
        };

        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsJsonAsync(response);
    }
}
