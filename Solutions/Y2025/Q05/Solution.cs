namespace Solutions.Y2025.Q05;

[QuestInfo("Fishbone Order", Topics.Simulation, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var fishbones = ParseInputLines(part, parser: Fishbone.Parse);   
        return part switch
        {
            1 => fishbones[0].Quality,
            2 => GetQualityRange(fishbones),
            3 => GetChecksum(fishbones),
            _ => NotSolvedString
        };
    }

    private static long GetQualityRange(Fishbone[] fishbones)
    {
        var qualities = fishbones
            .Select(fishbone => fishbone.Quality)
            .ToList();
        return qualities.Max() - qualities.Min();
    }
    
    private static long GetChecksum(Fishbone[] fishbones)
    {
        return fishbones
            .OrderDescending()
            .Select((fishbone, i) => fishbone.Id * (i + 1L))
            .Aggregate(seed: 0L, func: (acc, checksum) => acc + checksum);
    }
}
