using System.Net;
using System.Text.Json;
namespace SmartphoneStore.Api.Services;
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
    public async Task InvokeAsync(HttpContext context) {
        try { await next(context); }
        catch (Exception ex) {
            logger.LogError(ex, "Unhandled API exception. TraceId: {TraceId}", context.TraceIdentifier);
            if (context.Response.HasStarted) throw;
            context.Response.Clear(); context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Внутрішня помилка сервера.", traceId = context.TraceIdentifier }));
        }
    }
}