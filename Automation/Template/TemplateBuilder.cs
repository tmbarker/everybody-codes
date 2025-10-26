using Automation.Logger;

namespace Automation.Template;

public class TemplateBuilder(ILogger logger) : ITemplateBuilder
{
    public void Create(int year, int quest, string inputCachePath, string templateFilePath, string solutionsDirPath)
    {
        var fullInputCachePath   = GetFullPath(inputCachePath);
        var fullTemplateFilePath = GetFullPath(templateFilePath);
        var fullSolutionsDirPath = GetFullPath(solutionsDirPath);
        
        StubInputs(year, quest, fullInputCachePath);
        StubSolution(year, quest, fullTemplateFilePath, fullSolutionsDirPath);
    }

    private void StubInputs(int year, int quest, string inputCachePath)
    {
        if (string.IsNullOrWhiteSpace(inputCachePath) || !Directory.Exists(inputCachePath))
        {
            logger.Log($"Failed to create template, input cache directory could not be located [{inputCachePath}]",
                ConsoleColor.Red);
            return;
        }
        
        var inputDirPath = Path.Combine(inputCachePath, $"Y{year}", $"Q{quest:D2}");
        Directory.CreateDirectory(inputDirPath);
        
        for (var i = 0; i < 3; i++)
        {
            var inputFilePath = Path.Combine(inputDirPath, $"p{i + 1}.txt");
            if (File.Exists(inputFilePath))
            {
                logger.Log($"Will not create input file, target already exists [{inputFilePath}]",
                    ConsoleColor.Gray);
                continue;
            }
            
            File.CreateText(inputFilePath);
            logger.Log($"Input stub created [{inputFilePath}]", ConsoleColor.Green);
        }
    }
    
    private void StubSolution(int year, int quest, 
        string templateFilePath, 
        string solutionsDirPath)
    {
        if (string.IsNullOrWhiteSpace(templateFilePath) || !File.Exists(templateFilePath))
        {
            logger.Log($"Template file could not be located [{templateFilePath}]",
                ConsoleColor.Red);
            return;
        }
        
        if (string.IsNullOrWhiteSpace(solutionsDirPath) || !Directory.Exists(solutionsDirPath))
        {
            logger.Log($"Target solution directory could not be located [{solutionsDirPath}]",
                ConsoleColor.Red);
            return;
        }
        
        var destDirPath = Path.Combine(solutionsDirPath, $"Y{year}", $"Q{quest:D2}");
        var destFilePath = Path.Combine(destDirPath, Path.GetFileName(templateFilePath));
        
        if (File.Exists(destFilePath))
        {
            logger.Log($"Will not create solution file, target already exists [{destFilePath}]",
                ConsoleColor.Gray);
            return;
        }
        
        var templateLines = File.ReadAllLines(templateFilePath);
        for (var i = 0; i < templateLines.Length; i++)
        {
            if (templateLines[i].Contains("namespace"))
            {
                templateLines[i] = $"namespace Solutions.Y{year}.Q{quest:D2};";
                break;
            }
        }

        Directory.CreateDirectory(destDirPath);
        File.WriteAllLines(destFilePath, templateLines);
        logger.Log($"Solution stub created [{destFilePath}]", ConsoleColor.Green);
    }
    
    private static string GetFullPath(string path)
    {
        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(path, basePath: AppContext.BaseDirectory);
    }
}