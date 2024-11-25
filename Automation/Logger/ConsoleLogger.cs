namespace Automation.Logger;

public class ConsoleLogger : ILogger
{
    public void Log(string log, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(log);
        Console.ResetColor();
    }
}