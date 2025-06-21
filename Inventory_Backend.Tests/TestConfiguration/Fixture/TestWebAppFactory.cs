using System.Net.Http.Headers;
using Inventory_Backend_NET;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Seeder;
using Inventory_Backend.Tests.Fitur._Preparation;
using Inventory_Backend.Tests.Seeder;
using Inventory_Backend.Tests.TestConfiguration.Logging;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration.Fixture;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
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
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddConfiguration(testConfiguration);
                Configuration = configurationBuilder.Build();
            })
            .ConfigureTestServices(services =>
            {
                services
                    .AddSingleton<TimeProvider>(TestTimeProvider.Instance)
                    .AddSeeders()
                    .AddDatasets();

                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.ValidateLifetime = false;
                    options.Events.OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"QQQ Test Auth Fail {context.Exception.Message}");
                        return Task.CompletedTask;
                    };
                });

                DeleteDatabase(services);
            });

    }

    public HttpClient GetAuthorizedClient(bool isAdmin)
    {
        return GetAuthorizedClient(user => user.IsAdmin == isAdmin);
    }

    public HttpClient GetAuthorizedClient(int userId)
    {
        return GetAuthorizedClient(user => user.Id == userId);
    }

    private HttpClient GetAuthorizedClient(Func<User, bool> condition)
    {
        var jwt = GenerateJwt(condition);

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", jwt);
        return client;
    }

    private string GenerateJwt(Func<User, bool> condition)
    {
        using var scope = Server.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

        var user = db.Users.First(condition);
        return jwtService.GenerateNewToken(user);
    }

    public T Get<T>() where T : notnull
    {
        var scope = Server.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    public string GenerateJwt(bool isAdmin)
    {
        return GenerateJwt(user => user.IsAdmin == isAdmin);
    }

    public void ConfigureLoggingToTestOutput(ITestOutputHelper testOutputHelper)
    {
        Console.SetOut(new TestOutputWriter(testOutputHelper));
    }

    private void DeleteDatabase(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        context.Database.EnsureDeleted();
    }

    /// <summary>
    /// Membersihkan : <br></br>
    /// - <c>Seluruh tabel di database</c> <br></br>
    /// - <c>Memory cache</c><br></br>
    /// - <c>TestTimeProvider</c><br></br>
    /// </summary>
    public void Cleanup(IServiceScope? scope = null)
    {
        scope ??= Server.Services.CreateScope();
        TestTimeProvider.Instance.Reset();
        
        var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        var memCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>() as MemoryCache;
        memCache!.Clear();

        context.RefreshDatabase();
        scope.Dispose();
    }

public MyDbContext GetDbContext()
    {
        var scope = Server.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        return db;
    }
}

static class TestWebAppFactoryExtension
{
    public static IServiceCollection AddSeeders(this IServiceCollection services)
    {
        return services
            .GetAndAddDependenciesWithType(typeof(IMySeeder));
    }

    public static IServiceCollection AddDatasets(this IServiceCollection services)
    {
        return services
            .GetAndAddDependenciesWithType(typeof(IBaseDataset))
            .GetAndAddDependenciesWithType(typeof(IDerivedDataset));
    }

    private static IServiceCollection GetAndAddDependenciesWithType(
        this IServiceCollection services,
        Type targetType)
    {
        IEnumerable<Type> types = typeof(TestWebAppFactory).Assembly
            .GetTypes()
            .Where(t => targetType.IsAssignableFrom(t) && 
                        !t.IsInterface && 
                        !t.IsAbstract);

        foreach (var type in types)
            services.AddScoped(type);

        return services;
    }
} 