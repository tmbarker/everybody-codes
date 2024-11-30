using Utilities.Extensions;

namespace Solutions.Y2024.Q09;

public static class Stamps
{
    private static readonly Dictionary<int, string> Map = new()
    {
        { 1, "1, 3, 5, 10" },
        { 2, "1, 3, 5, 10, 15, 16, 20, 24, 25, 30" },
        { 3, "1, 3, 5, 10, 15, 16, 20, 24, 25, 30, 37, 38, 49, 50, 74, 75, 100, 101" }
    };
    
    public static long[] Get(int part)
    {
        return Map[part]
            .ParseLongs()
            .OrderDescending()
            .ToArray();
    }
}