namespace Inventory_Backend_NET.Fitur.Logging;

public class MyDevLogger : IMyLogger
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }
}