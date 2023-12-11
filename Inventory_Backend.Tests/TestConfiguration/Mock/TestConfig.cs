using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Inventory_Backend.Tests.TestConfiguration.Mock;

public class TestConfig
{
    public static IConfiguration GetTestConfig()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();
        return configuration;
    }
}