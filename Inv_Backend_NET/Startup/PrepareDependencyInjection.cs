using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Interceptor;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Fitur.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using NeoSmart.Caching.Sqlite.AspNetCore;

namespace Inventory_Backend_NET.Startup;

public static class PrepareDependencyInjection
{
    public static WebApplicationBuilder PrepareDependencyInjectionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
        builder.Services.AddHttpContextAccessor();

        // Alasan kenapa kok pakai sqlite cache :
        // Sqlite Cache disini fungsinya untuk mentrack urutan hari dari pengajuan yang dibuat (3 digit terakhir dari kode transaksi, lihat model pengajuan)
        //
        // kenapa kok enggak pakai memory cache?
        // karena kalo enggak sengaja server mati, memory cache bakalan terhapus datanya
        //
        // kenapa field urutan dari pengajuan, enggak di berdasarkan query dari database SQL Server? (berdasarkan pengajuan-pengajuan sebelumnya)
        // karena kalo di query dari database, hasilnya bakal salah, kalo ada pengajuan ditengah-tengah yang kehapus
        builder.Services.AddSqliteCache(
            options =>
            {
                options.CachePath = Path.Combine(Environment.CurrentDirectory, "Cache/cache.db");
            }
        );

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<IMyLogger>(new MyDevLogger());


        builder.Services.AddControllers(options =>
        {
            // untuk ngegunain JsonPropertyName kalo ada error
            options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
    
            options.MaxModelValidationErrors = 100;             
        });
        
        builder.Services.AddDbContext<MyDbContext>(
            optionsAction: (serviceProvider , options) =>
                options
                    .UseSqlServer(
                        builder.Configuration
                            .GetConnectionString(name: MyConstants.AppSettingsKey.MyConnectionString))
                    .AddInterceptors(
                        new CreateNewPengajuanInterceptor(
                            timeProvider: serviceProvider.GetRequiredService<TimeProvider>()))
        );

        return builder;
    }
}