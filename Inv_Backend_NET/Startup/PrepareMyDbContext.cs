using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Interceptor;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_Backend_NET.Startup;

public static class PrepareMyDbContext
{
    public static IServiceCollection PrepareMyDbContextWithInterceptor(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<MyDbContext>(
            optionsAction: (serviceProvider , options) =>
                options
                    .UseSqlServer(connectionString)
                    .AddInterceptors(
                        new UpdateTotalPengajuanByTanggalOnCreatePengajuanInterceptor(
                            timeProvider: serviceProvider.GetRequiredService<TimeProvider>()),
                        new UpdateCacheOnPengajuanChangedInterceptor(
                            memoryCache: serviceProvider.GetRequiredService<IMemoryCache>()))
        );

        return services;
    }
}