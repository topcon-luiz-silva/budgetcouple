namespace BudgetCouple.Api.Middleware;

using Serilog.Context;

/// <summary>
/// Middleware que gera e rastreia Correlation ID para requests.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract or generate correlation ID
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();

        // Add to response headers
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Add to LogContext for enrichment
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Extract user ID if available from claims
            var userId = context.User?.FindFirst("sub")?.Value ?? context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                using (LogContext.PushProperty("UserId", userId))
                {
                    using (LogContext.PushProperty("RequestPath", context.Request.Path))
                    {
                        await _next(context);
                    }
                }
            }
            else
            {
                using (LogContext.PushProperty("RequestPath", context.Request.Path))
                {
                    await _next(context);
                }
            }
        }
    }
}
