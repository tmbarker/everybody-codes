using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Automation.Logger;
using Solutions.Attributes;
using Solutions.Common;

namespace Automation.Runner;

/// <summary>
///     Reflective utility for running quest solutions.
/// </summary>
public class SolutionRunner(ILogger logger) : IRunner
{
    public Task Run(int year, int quest, string inputCachePath)
    {
        if (!TryCreateSolution(year, quest, out var solution))
        {
            return Task.CompletedTask;
        }
        
        if (CheckSolutionInputSpecific(solution, out var message))
        {
            Log(year, quest, log: message, ConsoleColor.DarkYellow);
        }
        
        RunSolution(year, quest, solution, inputCachePath);
        return Task.CompletedTask;
    }
    
    private void RunSolution(int year, int quest, SolutionBase solution, string inputCachePath)
    {
        try
        {
            solution.InputDirectory = Path.Combine(inputCachePath, $"Y{year}", $"Q{quest:D2}");
            
            var stopwatch = new Stopwatch();
            
            for (var i = 0; i < solution.Parts; i++)
            {
                stopwatch.Restart();
                var result = solution.Solve(part: i + 1);
                var elapsed = FormElapsedString(stopwatch.Elapsed);
                var message = $"[Elapsed: {elapsed}] Part {i + 1} => {result}";
                
                Log(year, quest, log: message, color: ConsoleColor.Green);
            }
        }
        catch (Exception e)
        {
            Log(year, quest, log: $"Error running solution:\n{e}", color: ConsoleColor.Red);
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
    
    private bool TryCreateSolution(int year, int quest,
        [NotNullWhen(returnValue: true)] out SolutionBase? solution)
    {
        solution = null;
        
        try
        {
            var owningAssembly = typeof(SolutionBase).Assembly;
            var assemblyName = owningAssembly.GetName().Name;
            var typeName = $"{assemblyName}.Y{year}.Q{quest:D2}.Solution";
            var type = owningAssembly.GetType(typeName);

            if (type == null)
            {
                Log(year, quest, "Unable to locate solution type", ConsoleColor.Red);
                return false;
            }
            
            solution = (SolutionBase)Activator.CreateInstance(type)!;
            return true;
        }
        catch (Exception e)
        {
            Log(year, quest, $"Failed to activate solution instance:\n{e}", ConsoleColor.Red);
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
    
    private void Log(int year, int quest, string log, ConsoleColor color)
    {
        logger.Log($"[Year: {year}, Quest: {quest}] {log}", color);
    }
    
}