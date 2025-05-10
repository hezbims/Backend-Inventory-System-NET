using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Inventory_Backend_NET.Startup;

public static class PrepareExceptionHandling
{
    public static WebApplicationBuilder PrepareExceptionHandlingServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(); // handle kalau exception handling return false
        builder.Services.AddExceptionHandler<GlobalExceptionHandling>();
        
        return builder;
    }
    
    internal sealed class GlobalExceptionHandling(ILogger logger) : IExceptionHandler
    {
        private readonly ILogger _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error(
                exception: exception,
                messageTemplate: "Unhandled exception : {Message}",
                propertyValue: exception.Message);
            
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            await httpContext.Response.WriteAsJsonAsync(new
            {
                errors = new
                {
                    _general_ = "Internal Server Error",
                }
            }, cancellationToken);

            return true;
        }
    }
}