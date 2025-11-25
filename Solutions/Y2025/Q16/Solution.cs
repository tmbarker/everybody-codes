using Utilities.Extensions;

namespace Solutions.Y2025.Q16;

[QuestInfo("Harmonics of Stone", Topics.Math, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputText(part);
        var numbers = input.ParseInts();

        return part switch
        {
            1 => CountBricks(spell: numbers, cols: 90L),
            2 => Part2(wall: numbers),
            3 => Part3(wall: numbers, totalBricks: 202520252025000L),
            _ => NotSolvedString
        };
    }
    
    private static long Part2(int[] wall)
    {
        return GetSpell(wall).Aggregate(seed: 1L, func: (acc, step) => acc * step);
    }

    private static long CountBricks(int[] spell, long cols)
    {
        return spell.Select(step => cols / step).Sum();
    }
    
    private static List<int> GetSpell(int[] wall)
    {
        var spell = new List<int>();
        for (var i = 0; i < wall.Length; i++)
        {
            if (wall[i] <= 0)
            {
                continue;
            }
            
            spell.Add(i + 1);
            for (var j = i; j < wall.Length; j += i + 1)
            {
                wall[j]--;
            }
        }
        return spell;
    }

    private static long Part3(int[] wall, long totalBricks)
    {
        var spell = GetSpell(wall);
        var left = 1L;
        var right = totalBricks;
        var result = 0L;
        
        while (left <= right)
        {
            var middle = left + (right - left) / 2L;
            var requiredBricks = CountBricks(spell.ToArray(), middle);
            
            if (requiredBricks <= totalBricks)
            {
                result = middle;
                left = middle + 1L;
            }
            else
            {
                right = middle - 1L;
            }
        }
        
        return result;
    }
}
