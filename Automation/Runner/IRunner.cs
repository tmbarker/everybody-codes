namespace Automation.Runner;

public interface IRunner
{
    Task Run(int year, int quest, string inputCachePath);
}