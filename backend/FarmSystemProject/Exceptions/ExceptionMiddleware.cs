using FarmSystemProject.DTOs;
using FarmSystemProject.Exceptions;
using System.Net;

namespace FarmSystemProject.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
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
                status = HttpStatusCode.InternalServerError;
                error = "Internal Server Error";
                message = "Ocorreu um erro interno. Tente novamente mais tarde.";
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
