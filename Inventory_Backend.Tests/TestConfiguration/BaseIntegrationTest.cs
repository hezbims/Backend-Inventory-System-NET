using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Seeder;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration;

/// <summary>
/// Class ini digunakan agar ketika membuat test, kita enggak perlu ribet-ribet lagi mikirin cleanup
/// </summary>
public abstract class BaseIntegrationTest
{
    private readonly TestWebAppFactory _webApp;

    private HttpClient? _adminClient;
    protected HttpClient AdminClient => _adminClient ??= _webApp.GetAuthorizedClient(isAdmin: true);
    private HttpClient? _nonAdminClient;
    protected HttpClient NonAdminClient => _nonAdminClient ??= _webApp.GetAuthorizedClient(isAdmin: false);
    private HttpClient? _customClient;

    private IServiceScope? _scope;

    private MyDbContext? _dbContext;
    protected MyDbContext Db => _dbContext ??= _webApp.Get<MyDbContext>();
    
    
    protected readonly ITestOutputHelper Output;
    
    protected BaseIntegrationTest(
        TestWebAppFactory factory,
        ITestOutputHelper output)
    {
        _webApp = factory;
        _webApp.ConfigureLoggingToTestOutput(output);
        Output = output;
        
        ResetServices();
    }
    
    protected T Get<T>() where T : notnull
    {
        return _scope!.ServiceProvider.GetRequiredService<T>();
    }

    protected HttpClient GetAuthorizedClient(int userId)
    {
        _customClient?.Dispose();
        _customClient = _webApp.GetAuthorizedClient(userId);
        return _customClient;
    }

    private void ResetServices()
    {
        TestTimeProvider.Instance.Reset();

        using  (var currentScope = _scope ?? _webApp.Server.Services.CreateScope())
        {
            var context = currentScope.ServiceProvider.GetRequiredService<MyDbContext>();
            var memCache = currentScope.ServiceProvider.GetRequiredService<IMemoryCache>() as MemoryCache;
            
            memCache!.Clear();
            context.RefreshDatabase();
        }

        _adminClient?.Dispose();
        _nonAdminClient?.Dispose();
        _customClient?.Dispose();
        
        _scope = _webApp.Server.Services.CreateScope();
    }
}