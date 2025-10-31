using Utilities.Collections;
using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.S02.Q03;

[QuestInfo("The Dice that Never Lie (Unless I Tell Them To)", Topics.Simulation|Topics.Vectors, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        return part switch
        {
            1 => GetRequiredRolls(input),
            2 => GetFinishOrder(input),
            3 => GetMaxCoins(input),
            _ => NotSolvedString
        };
    }

    private static long GetRequiredRolls(string[] input)
    {
        var total = 0L;
        var rolls = 0L;
        var dice = input.Select(Dice.Parse).ToArray();
        
        while (total < 10000L)
        {
            total += dice.Sum(die => die.Roll());
            rolls++;
        }
        
        return rolls;
    }

    private static string GetFinishOrder(string[] input)
    {
        var dice = input[..^2].Select(Dice.Parse).ToArray();
        var track = input[^1].Select(c => (long)c.AsDigit()).ToArray();
        var positions = new long[dice.Length];
        var finished = new List<long>();
        
        while (finished.Count < dice.Length)
        {
            for (var i = 0; i < dice.Length; i++)
            {
                if (finished.Contains(dice[i].Id))
                {
                    continue;
                }
                
                if (track[positions[i]] == dice[i].Roll() && ++positions[i] >= track.Length)
                {
                    finished.Add(dice[i].Id);
                }
            }
        }
        
        return string.Join(",", finished);
    }

    private static int GetMaxCoins(string[] input)
    {
        var chunks = input.ChunkByNonEmpty();
        var dice = chunks[0].Select(Dice.Parse).ToArray();
        var grid = Grid2D<long>.MapChars(strings: chunks[1], elementFunc: c => c.AsDigit());
        
        var coinPositions = new HashSet<Vec2D>();
        var adjacencyMemo = new DefaultDict<Vec2D, Vec2D[]>(defaultSelector:
            pos => [..pos.GetAdjacentSet(Metric.Taxicab).Where(grid.Contains)]);

        foreach (var die in dice)
        {
            var starts = new HashSet<Vec2D>(collection: grid);
            while (starts.Count > 0)
            {
                var roll = die.Roll();
                var next = new HashSet<Vec2D>();
                
                foreach (var start in starts.Where(pos => grid[pos] == roll))
                {
                    coinPositions.Add(start);
                    next.Add(start);
                    next.AddRange(adjacencyMemo[start]);
                }
                
                starts = next;
            }
        }
        
        return coinPositions.Count;
    }
}
