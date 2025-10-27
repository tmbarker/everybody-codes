using System.CommandLine;
using Automation.Logger;
using Automation.Runner;
using Automation.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EC;

internal static class Program
{
    private const string InputCachePathKey = "InputCachePath";
    private const string TemplateFilePathKey = "TemplateFilePath";
    private const string SolutionDirsPathKey = "SolutionsDirPath";
    
    private static readonly IConfiguration Configuration = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
        .Build();

    private static readonly ServiceProvider ServiceProvider = new ServiceCollection()
        .AddSingleton<ILogger, ConsoleLogger>()
        .AddSingleton<ISolutionRunner, SolutionRunner>()
        .AddSingleton<ITemplateBuilder, TemplateBuilder>()
        .BuildServiceProvider();
    
    public static async Task<int> Main(string[] args)
    {
        var seriesArg = new Argument<int>(
            name: "series", 
            description: "The series ID (year for events, number for stories)");
        var questArg = new Argument<int>(
            name: "quest", 
            description: "The quest ID (1-based index)");
        
        var solveCommand = new Command(
            name: "solve", 
            description: "Run the specified quest solution, if it exists");
        var templateCommand = new Command(
            name: "template",
            description: "Create templated solution and input files for the specified quest");
        
        solveCommand.AddArgument(seriesArg);
        solveCommand.AddArgument(questArg);
        solveCommand.SetHandler(
            handle: async (series, quest) => await SolveHandler(series, quest),
            symbol1: seriesArg,
            symbol2: questArg);
        
        templateCommand.AddArgument(seriesArg);
        templateCommand.AddArgument(questArg);
        templateCommand.SetHandler(
            handle: TemplateHandler,
            symbol1: seriesArg,
            symbol2: questArg);
        
        var rootCommand = new RootCommand(description: "CLI entry point for running 'Everybody Codes' solutions");
        rootCommand.AddCommand(solveCommand);
        rootCommand.AddCommand(templateCommand);
        
        return await rootCommand.InvokeAsync(args);
    }

    private static Task SolveHandler(int series, int quest)
    {
        var inputCachePath = Configuration[InputCachePathKey] ?? string.Empty;
        var solutionRunner = ServiceProvider.GetRequiredService<ISolutionRunner>();
        
        return solutionRunner.Run(series, quest, inputCachePath);
    }

    private static void TemplateHandler(int series, int quest)
    {
        ServiceProvider
            .GetRequiredService<ITemplateBuilder>()
            .Create(series, quest,
                inputCachePath:  GetConfigString(InputCachePathKey),
                templateFilePath: GetConfigString(TemplateFilePathKey),
                solutionsDirPath: GetConfigString(SolutionDirsPathKey));
    }

    private static string GetConfigString(string key)
    {
        return Configuration[key] ?? throw new KeyNotFoundException(key);
    }
}