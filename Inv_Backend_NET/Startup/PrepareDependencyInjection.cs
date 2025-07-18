using Inventory_Backend_NET.Common.Presentation.Service;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Interceptor;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Fitur.Barang._Dependency;
using Inventory_Backend_NET.Fitur.Logging;
using Inventory_Backend_NET.Fitur.Pengajuan._Dependency;
using Inventory_Backend_NET.Startup.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_Backend_NET.Startup;

public static class PrepareDependencyInjection
{
    public static WebApplicationBuilder PrepareDependencyInjectionServices(this WebApplicationBuilder builder)
    {
        // Add All subdomain dependency
        builder.Services.AddBarangDependency();
        builder.Services.AddPengajuanDependency();
        
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<IMyLogger>(new MyDevLogger());
        builder.Services.AddSingleton<IAllDomainErrorTranslator, AllDomainErrorTranslatorImpl>();


        builder.Services.AddControllers(options =>
        {
            // untuk ngegunain JsonPropertyName kalo ada error
            options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
    
            options.MaxModelValidationErrors = 100;
            
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy)); // 👈 Global AuthGuard
        });
        
        builder.Services.AddDbContext<MyDbContext>(
            optionsAction: (serviceProvider , options) =>
                options
                    .UseSqlServer(
                        builder.Configuration
                            .GetConnectionString(name: MyConstants.AppSettingsKey.MyConnectionString))
                    .AddInterceptors(
                        new UpdateTotalPengajuanByTanggalOnCreatePengajuanInterceptor(
                            timeProvider: serviceProvider.GetRequiredService<TimeProvider>()),
                        new UpdateCacheOnPengajuanChangedInterceptor(
                            memoryCache: serviceProvider.GetRequiredService<IMemoryCache>()))
        );

        return builder;
    }
}