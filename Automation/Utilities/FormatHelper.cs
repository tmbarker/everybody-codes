namespace Automation.Utilities;

public static class FormatHelper
{
    private const int FirstEventYear = 2024;

    private static string GetPrefix(int series)
    {
        return series < FirstEventYear ? "S" : "Y";
    }

    private static string GetFormattedSeries(int series)
    {
        return series < FirstEventYear ? $"{series:D2}" : $"{series:D4}";
    }
    
    public static string GetDirectoryName(int series)
    {
        return $"{GetPrefix(series)}{GetFormattedSeries(series)}";
    }
    
    public static string GetNamespaceName(int series, int quest)
    {
        return $"Solutions.{GetDirectoryName(series)}.Q{quest:D2}";
    }
    
    public static string GetEventLabel(int series)
    {
        return series < FirstEventYear ? "Story" : "Year";
    }
}
