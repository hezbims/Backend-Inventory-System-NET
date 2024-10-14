namespace Inventory_Backend.Tests.TestData;

public class TestTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow()
    {
        return new DateTimeOffset(2024, 10, 1, 12, 0, 0, TimeSpan.Zero);
    }

    public override TimeZoneInfo LocalTimeZone { get; } =  TimeZoneInfo.Utc;
}