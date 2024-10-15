using System.Net.Http.Headers;
using Inventory_Backend_NET;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Logic.Services;
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
                services.AddSingleton<TimeProvider>(new TestTimeProvider());

                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.ValidateLifetime = false;
                    options.Events.OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"QQQ Test Auth Fail {context.Exception.Message}");
                        return Task.CompletedTask;
                    };
                });
                
                RefreshDatabase(services);
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
        using var scope = Server.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
        
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", 
            jwtService.GenerateNewToken(db.Users.First(condition)));
        return client;
    }

    public void ConfigureLoggingToTestOutput(ITestOutputHelper testOutputHelper)
    {
        Console.SetOut(new TestOutputWriter(testOutputHelper));
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