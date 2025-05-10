using Inventory_Backend_NET.Database;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration;

public abstract class BaseEmptyIntegrationTest : IDisposable
{
    protected readonly TestWebAppFactory _webApp;
    protected HttpClient _adminClient => _webApp.GetAuthorizedClient(isAdmin: true);
    protected HttpClient _nonAdminClient => _webApp.GetAuthorizedClient(isAdmin: false);
    protected MyDbContext _dbContext => _webApp.GetDbContext();
    protected readonly ITestOutputHelper _output;
    
    public BaseEmptyIntegrationTest(
        TestWebAppFactory factory,
        ITestOutputHelper output)
    {
        _webApp = factory;
        _webApp.ConfigureLoggingToTestOutput(output);
        _output = output;
    }

    public void Dispose()
    {
        _webApp.Dispose();
    }
}