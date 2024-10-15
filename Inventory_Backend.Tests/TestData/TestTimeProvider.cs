namespace Inventory_Backend.Tests.TestData;

public class TestTimeProvider : TimeProvider
{
    public readonly static TestTimeProvider Instance = new TestTimeProvider();

    private TestTimeProvider()
    {
    }

    private static DateTimeOffset DefaultDateTime => new DateTimeOffset(2024, 10, 1, 12, 0, 0, TimeSpan.Zero);

    private DateTimeOffset _dateTimeNow = DefaultDateTime;
    public override DateTimeOffset GetUtcNow()
    {
        return _dateTimeNow;
    }

    public void AddDays(int days)
    {
        _dateTimeNow = _dateTimeNow.AddDays(days);
    }

    public void Reset()
    {
        _dateTimeNow = DefaultDateTime;
    }

    public override TimeZoneInfo LocalTimeZone { get; } =  TimeZoneInfo.Utc;
}