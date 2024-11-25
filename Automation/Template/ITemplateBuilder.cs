namespace Automation.Template;

public interface ITemplateBuilder
{
    void Create(int year, int quest, string inputCachePath, string templateFilePath, string solutionsDirPath);
}