// ============================================================
// ErrorHandlingMiddleware.cs — Tratamento de erros padronizado
// ============================================================

using FCG.UsersAPI.Domain.Entidades;
using System.Net;
using System.Text.Json;

namespace FCG.UsersAPI.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate             _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
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
            _logger.LogError(ex, "Erro: {Mensagem}", ex.Message);
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static async Task TratarExcecaoAsync(HttpContext context, Exception exception)
    {
        var (statusCode, mensagem) = exception switch
        {
            DomainException       domainEx => (HttpStatusCode.BadRequest, domainEx.Message),
            UnauthorizedAccessException   => (HttpStatusCode.Unauthorized, "Acesso não autorizado."),
            KeyNotFoundException          => (HttpStatusCode.NotFound, "Recurso não encontrado."),
            _                             => (HttpStatusCode.InternalServerError, "Erro interno.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var json = JsonSerializer.Serialize(
            new { statusCode = (int)statusCode, mensagem, timestamp = DateTime.UtcNow },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(json);
    }
}