namespace Inventory_Backend_NET.Fitur._Logic.Extension;

public static class DateTimeExtension
{
    public static bool IsToday(this DateTime date)
    {
        var today = DateTime.Now;
        return date.Year == today.Year &&
               date.Month == today.Month &&
               date.Day == today.Day;
    }
}