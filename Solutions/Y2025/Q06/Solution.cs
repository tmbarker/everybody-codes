using Utilities.Collections;

namespace Solutions.Y2025.Q06;

using Memo = Dictionary<(int, int, int), long>;

[QuestInfo("Mentorship Matrix", Topics.None, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputText(part);
        return part switch
        {
            1 => CountPredecessors(input, germane: "Aa"),
            2 => CountPredecessors(input, germane: "AaBbCc"),
            3 => CountInRange(input, range: 1000, repeats: 1000),
            _ => NotSolvedString
        };
    }

    private static long CountPredecessors(string input, string germane)
    {
        var counts = new DefaultDict<char, long>(defaultValue: 0L);
        var filter = germane.ToHashSet();
        var total = 0L;
        
        foreach (var c in input.Where(filter.Contains))
        {
            if (char.IsUpper(c))
            {
                counts[c]++;
            }
            else
            {
                total += counts[char.ToUpper(c)];
            }
        }

        return total;
    }

    private static long CountInRange(string input, int range, int repeats)
    {
        var memo = new Memo();
        var length = input.Length;
        var total = 0L;
        
        for (var i = 0; i < input.Length; i++)
        {
            if (!char.IsLower(input[i]))
            {
                continue;
            }

            for (var r = 0; r < repeats; r++)
            {
                total += ComputeInRange(input, index: i + r * length, range, repeats, memo);   
            }
        }
        
        return total;
    }

    private static long ComputeInRange(string input, int index, int range, int repeats, Memo memo)
    {
        var nIdx = index % input.Length;
        var lIdx = Math.Max(0, index - range);
        var rIdx = Math.Min(repeats * input.Length - 1, index + range);
        
        //  NOTE: Memoization key is a tuple of the normalized novice index, and the effective length
        //  of the forward and backward mentor range, accounting for wrapping due to repetitions.
        //
        var key = (nIdx, lIdx - index, rIdx - index);
        if (memo.TryGetValue(key, out var cached))
        {
            return cached;
        }
        
        var count = 0L;
        var mentor = char.ToUpper(input[nIdx]);
        
        for (var i = lIdx; i <= rIdx; i++)
        {
            if (i != index && input[i % input.Length] == mentor)
            {
                count++;
            }
        }
        
        return memo[key] = count;
    }
}
