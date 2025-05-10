using Serilog;
using ILogger = Serilog.ILogger;

namespace Inventory_Backend_NET.Startup;

public static class PrepareMonitoring
{
    public static WebApplicationBuilder PrepareMonitongServices(
        this WebApplicationBuilder builder)
    {
        var serilogLogger = new LoggerConfiguration()
            .WriteTo.File(
                path: "log/logs-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: TimeSpan.FromDays(30))
            .CreateLogger();
        
        builder.Services.AddSingleton<ILogger>(serilogLogger);

        return builder;
    }
}