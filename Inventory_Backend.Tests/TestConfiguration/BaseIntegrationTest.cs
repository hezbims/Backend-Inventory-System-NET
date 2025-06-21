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
public abstract class BaseIntegrationTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;

    private HttpClient? _adminClient;
    protected HttpClient AdminClient => _adminClient ??= _webApp.GetAuthorizedClient(isAdmin: true);
    private HttpClient? _nonAdminClient;
    protected HttpClient NonAdminClient => _nonAdminClient ??= _webApp.GetAuthorizedClient(isAdmin: false);
    private HttpClient? _customClient;

    private readonly IServiceScope _scope;

    private MyDbContext? _dbContext;
    protected MyDbContext Db => _dbContext ??= _webApp.GetService<MyDbContext>();
    
    
    protected readonly ITestOutputHelper Output;
    
    protected BaseIntegrationTest(
        TestWebAppFactory factory,
        ITestOutputHelper output)
    {
        _webApp = factory;
        
        _webApp.ConfigureLoggingToTestOutput(output);
        _scope = factory.Server.Services.CreateScope();
        
        Output = output;
    }
    
    protected T GetService<T>() where T : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    protected HttpClient GetAuthorizedClient(int userId)
    {
        _customClient?.Dispose();
        _customClient = _webApp.GetAuthorizedClient(userId);
        return _customClient;
    }

    public void Dispose()
    {
        TestTimeProvider.Instance.Reset();
        
        var context = _scope.ServiceProvider.GetRequiredService<MyDbContext>();
        var memCache = _scope.ServiceProvider.GetRequiredService<IMemoryCache>() as MemoryCache;
        memCache!.Clear();

        context.RefreshDatabase();
        _scope.Dispose();
        
        _adminClient?.Dispose();
        _nonAdminClient?.Dispose();
        _customClient?.Dispose();
    }
}