using Inventory_Backend_NET.Common.Presentation.Service;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Fitur.Barang._Dependency;
using Inventory_Backend_NET.Fitur.Logging;
using Inventory_Backend_NET.Fitur.Pengajuan._Dependency;
using Inventory_Backend_NET.Startup.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

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

        string connectionString = builder
            .Configuration
            .GetConnectionString(MyConstants.AppSettingsKey.InventoryDbConnectionString)!;

        builder.Services.PrepareMyDbContextWithInterceptor(connectionString);

        return builder;
    }
}