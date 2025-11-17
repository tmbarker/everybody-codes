using Utilities.Extensions;

namespace Solutions.Y2025.Q11;

[QuestInfo("The Scout Duck Protocol", Topics.Simulation, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var nums = ParseInputLines(part, long.Parse);
        return part switch
        {
            1 => Part1(nums, rounds: 10),
            2 => Part2(nums),
            3 => Part3(nums),
            _ => NotSolvedString
        };
    }

    private static long Part1(long[] nums, int rounds)
    {
        var change = true;
        for (var round = 0; round < rounds; round++)
        {
            if (change)
            {
                change = Phase1(nums);
                if (change)
                {
                    continue;
                }
            }
            
            if (!Phase2(nums))
            {
                break;
            }
        }
        
        return nums
            .Select((n, i) => n * (i + 1L))
            .Sum();
    }
    
    private static long Part2(long[] nums)
    {
        var rounds = -1;
        var change = true;

        while (change)
        {
            change = Phase1(nums);
            rounds++;
        }
        
        return rounds + Phase2Length(nums);
    }

    private static long Phase2Length(long[] nums)
    {
        var mean = nums.Sum() / nums.Length;
        return nums.Sum(num => num < mean
            ? mean - num
            : 0L);
    }
    
    private static long Part3(long[] nums)
    {
        return nums.Windows(size: 2).Any(pair => pair[0] >= pair[1])
            ? throw new NoSolutionException("This solution only works for increasing sequences.")
            : Phase2Length(nums);
    }
    
    private static bool Phase1(long[] nums)
    {
        var change = false;
        for (var i = 0; i < nums.Length - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                nums[i]--;
                nums[i + 1]++;
                change = true;
            }
        }
        return change;
    }

    private static bool Phase2(long[] nums)
    {
        var change = false;
        for (var i = 0; i < nums.Length - 1; i++)
        {
            if (nums[i] == nums[i + 1])
            {
                continue;
            }
            
            var delta = nums[i] < nums[i + 1] ? 1 : -1;
            nums[i] += delta;
            nums[i + 1] -= delta;
            change = true;
        }
        return change;
    }
}
