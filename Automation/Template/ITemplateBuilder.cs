namespace Automation.Template;

public interface ITemplateBuilder
{
    void Create(int series, int quest, string inputCachePath, string templateFilePath, string solutionsDirPath);
}