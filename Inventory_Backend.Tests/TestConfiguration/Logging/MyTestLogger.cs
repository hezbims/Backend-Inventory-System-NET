using Inventory_Backend_NET.Fitur.Logging;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration.Logging;

public class MyTestLogger : IMyLogger
{
    private readonly ITestOutputHelper _logger;

    public MyTestLogger(ITestOutputHelper logger)
    {
        _logger = logger;
    }
    public void WriteLine(string message)
    {
        _logger.WriteLine(message);
    }
}