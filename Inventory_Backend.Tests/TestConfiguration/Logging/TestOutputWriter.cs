using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration.Logging;

public class TestOutputWriter : StringWriter
{
    private ITestOutputHelper _output;
    public TestOutputWriter(ITestOutputHelper output)
    {
        _output = output;
    }

    public override void WriteLine(string? m)
    {
        _output.WriteLine(m);
    }
}