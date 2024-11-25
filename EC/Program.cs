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
        .AddSingleton<IRunner, SolutionRunner>()
        .AddSingleton<ITemplateBuilder, TemplateBuilder>()
        .BuildServiceProvider();
    
    public static async Task<int> Main(string[] args)
    {
        var yearArg = new Argument<int>(
            name: "year", 
            description: "The year the quest belongs to");
        var questArg = new Argument<int>(
            name: "day", 
            description: "The quest day");
        
        var solveCommand = new Command(
            name: "solve", 
            description: "Run the specified quest solution, if it exists");
        var templateCommand = new Command(
            name: "template",
            description: "Create templated solution and input files for the specified quest");
        
        solveCommand.AddArgument(yearArg);
        solveCommand.AddArgument(questArg);
        solveCommand.SetHandler(
            handle: async (year, quest) => await SolveHandler(year, quest),
            symbol1: yearArg,
            symbol2: questArg);
        
        templateCommand.AddArgument(yearArg);
        templateCommand.AddArgument(questArg);
        templateCommand.SetHandler(
            handle: TemplateHandler,
            symbol1: yearArg,
            symbol2: questArg);
        
        var rootCommand = new RootCommand(description: "CLI entry point for running 'Everybody Codes' solutions");
        rootCommand.AddCommand(solveCommand);
        rootCommand.AddCommand(templateCommand);
        
        return await rootCommand.InvokeAsync(args);
    }

    private static Task SolveHandler(int year, int quest)
    {
        var inputCachePath = Configuration[InputCachePathKey] ?? string.Empty;
        var runner = ServiceProvider.GetRequiredService<IRunner>();
        
        return runner.Run(year, quest, inputCachePath);
    }

    private static void TemplateHandler(int year, int quest)
    {
        ServiceProvider
            .GetRequiredService<ITemplateBuilder>()
            .Create(year, quest,
                inputCachePath:  GetConfigString(InputCachePathKey),
                templateFilePath: GetConfigString(TemplateFilePathKey),
                solutionsDirPath: GetConfigString(SolutionDirsPathKey));
    }

    private static string GetConfigString(string key)
    {
        return Configuration[key] ?? throw new KeyNotFoundException(key);
    }
}