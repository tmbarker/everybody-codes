namespace Automation.Runner;

public interface ISolutionRunner
{
    Task Run(int series, int quest, string inputCachePath);
}