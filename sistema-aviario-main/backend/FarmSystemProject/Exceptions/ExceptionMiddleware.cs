using FarmSystemProject.DTOs;
using FarmSystemProject.Exceptions;
using System.Net;

namespace FarmSystemProject.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
                // O detalhe completo (stack trace, connection string em erros de SQL, ...)
                // fica só no log do servidor. O cliente recebe uma mensagem genérica.
                message = _environment.IsDevelopment()
                    ? exception.ToString()
                    : "Ocorreu um erro inesperado. Tente novamente mais tarde.";
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
