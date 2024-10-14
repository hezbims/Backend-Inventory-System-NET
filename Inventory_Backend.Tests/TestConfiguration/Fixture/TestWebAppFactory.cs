using Inventory_Backend_NET;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur.Logging;
using Inventory_Backend_NET.Seeder;
using Inventory_Backend.Tests.TestConfiguration.Logging;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration.Fixture;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    public ITestOutputHelper? Logger { get; set; }
    private IConfiguration? _configuration;
    public IConfiguration Configuration
    {
        get => _configuration!;
        private set => _configuration = value;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testConfiguration = TestConfig.GetTestConfig();
        builder
            .UseConfiguration(testConfiguration)
            .ConfigureLogging(loggingBuilder =>
            {
                if (Logger != null)
                    loggingBuilder.AddFilter(level => level == LogLevel.Error).AddXUnit(Logger);
            })
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddConfiguration(testConfiguration);
                Configuration = configurationBuilder.Build();
            })
            .ConfigureTestServices(services =>
            {
                services.AddSingleton<TimeProvider>(new TestTimeProvider());
                if (Logger != null)
                {
                    Logger.WriteLine("QQQ Logger added");
                    services.AddSingleton<IMyLogger>(new MyTestLogger(logger: Logger!));
                }

                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.ValidateLifetime = false;
                    options.Events.OnAuthenticationFailed = context =>
                    {
                        Logger?.WriteLine($"QQQ Test Auth Fail {context.Exception.Message}");
                        return Task.CompletedTask;
                    };
                });
                
                RefreshDatabase(services);
            });
        
    }

    public void RefreshDatabase(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        context.Database.EnsureDeleted();
    }

    public void Cleanup()
    {
        using var context = GetDbContext();
        context.RefreshDatabase();
    }

    public MyDbContext GetDbContext()
    {
        var scope = Server.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        return db;
    } 
        
}