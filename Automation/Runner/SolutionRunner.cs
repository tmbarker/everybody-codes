using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Automation.Logger;
using Automation.Utilities;
using Solutions.Attributes;
using Solutions.Common;

namespace Automation.Runner;

/// <summary>
///     Reflective utility for running quest solutions.
/// </summary>
public class SolutionRunner(ILogger logger) : ISolutionRunner
{
    public Task Run(int series, int quest, string inputCachePath)
    {
        if (!TryCreateSolution(series, quest, out var solution))
        {
            return Task.CompletedTask;
        }
        
        if (CheckSolutionInputSpecific(solution, out var message))
        {
            Log(series, quest, log: message, ConsoleColor.DarkYellow);
        }
        
        RunSolution(series, quest, solution, inputCachePath);
        return Task.CompletedTask;
    }
    
    private void RunSolution(int series, int quest, SolutionBase solution, string inputCachePath)
    {
        try
        {
            solution.InputDirectory = Path.Combine(inputCachePath, FormatHelper.GetDirectoryName(series), $"Q{quest:D2}");
            
            var stopwatch = new Stopwatch();
            
            for (var i = 0; i < solution.Parts; i++)
            {
                stopwatch.Restart();
                var result = solution.Solve(part: i + 1);
                var elapsed = FormElapsedString(stopwatch.Elapsed);
                var message = $"[Elapsed: {elapsed}] Part {i + 1} => {result}";
                
                Log(series, quest, log: message, color: ConsoleColor.Green);
            }
        }
        catch (Exception e)
        {
            Log(series, quest, log: $"Error running solution:\n{e}", color: ConsoleColor.Red);
        }
    }
    
    private static bool CheckSolutionInputSpecific(SolutionBase instance, out string message)
    {
        message = string.Empty;
        var attr = Attribute.GetCustomAttribute(
            element: instance.GetType(),
            attributeType: typeof(InputSpecificSolutionAttribute));

        if (attr != null)
        {
            message = $"[Warning] {((InputSpecificSolutionAttribute)attr).Message}";
        }

        return attr != null;
    }
    
    private bool TryCreateSolution(int series, int quest,
        [NotNullWhen(returnValue: true)] out SolutionBase? solution)
    {
        solution = null;
        
        try
        {
            var owningAssembly = typeof(SolutionBase).Assembly;
            var assemblyName = owningAssembly.GetName().Name;
            var typeName = $"{assemblyName}.{FormatHelper.GetDirectoryName(series)}.Q{quest:D2}.Solution";
            var type = owningAssembly.GetType(typeName);

            if (type == null)
            {
                Log(series, quest, "Unable to locate solution type", ConsoleColor.Red);
                return false;
            }
            
            solution = (SolutionBase)Activator.CreateInstance(type)!;
            return true;
        }
        catch (Exception e)
        {
            Log(series, quest, $"Failed to activate solution instance:\n{e}", ConsoleColor.Red);
            return false;
        }
    }
    
    private static string FormElapsedString(TimeSpan elapsed)
    {
        var sb = new StringBuilder();
        var overASecond = false;
        
        if (elapsed.TotalSeconds >= 1f)
        {
            sb.Append($"{(int)elapsed.TotalSeconds}.");
            overASecond = true;
        }

        sb.Append(overASecond ? $"{elapsed.Milliseconds:D3}" : $"{elapsed.Milliseconds}");
        sb.Append(overASecond ? "s" : "ms");
        
        return sb.ToString();
    }
    
    private void Log(int series, int quest, string log, ConsoleColor color)
    {
        var eventLabel = FormatHelper.GetEventLabel(series);
        logger.Log($"[{eventLabel}: {series}, Quest: {quest}] {log}", color);
    }
    
}