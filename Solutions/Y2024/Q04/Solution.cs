using Utilities.Extensions;

namespace Solutions.Y2024.Q04;

[QuestInfo("Royal Smith's Puzzle", Topics.Math, Difficulty.Easy)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var heights = ParseInputLines(part, parser: line => line.ParseInt());
        return part switch
        {
            1 => LevelNaive(heights),
            2 => LevelNaive(heights),
            3 => LevelMedian(heights),
            _ => NotSolvedString
        };
    }

    private static int LevelNaive(int[] heights)
    {
        var sum = 0;
        var min = int.MaxValue;

        foreach (var height in heights)
        {
            sum += height;
            min = Math.Min(height, min);
        }

        return sum - min * heights.Length;
    }

    private static int LevelMedian(int[] heights)
    {
        var sorted = heights
            .OrderBy(n => n)
            .ToArray();
        
        var n = sorted.Length;
        var median = n % 2 == 1
            ? sorted[n / 2]
            : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;

        var integral = (int)Math.Round(median);
        return heights.Sum(h => Math.Abs(h - integral));
    }
}
