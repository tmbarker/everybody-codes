using Utilities.Extensions;
using Utilities.Numerics;

namespace Solutions.Y2025.Q04;

using Ratio = (long Numerator, long Denominator);

[QuestInfo("Teeth of the Wind", Topics.Math|Topics.Simulation, Difficulty.Medium, favourite: true)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var gears = ParseInputLines(part, line => line.ParseInts());
        var ratio = GetRatio(gears);
        
        return part switch
        {
            1 => ComputeLast(ratio,  turns: 2025L),
            2 => ComputeFirst(ratio, turns: 10000000000000L),
            3 => ComputeLast(ratio,  turns: 100L),
            _ => NotSolvedString
        };
    }

    private static long ComputeLast(Ratio ratio, long turns)  => turns * ratio.Numerator / ratio.Denominator;
    private static long ComputeFirst(Ratio ratio, long turns) => (turns * ratio.Denominator).CeilDiv(ratio.Numerator);
    
    private static Ratio GetRatio(int[][] shafts)
    {
        var num = 1L;
        var den = 1L;

        for (var i = 1; i < shafts.Length; i++)
        {
            num *= shafts[i - 1][^1];
            den *= shafts[i][0];
            
            var gcd = Numerics.Gcd(num, den);
            num /= gcd;
            den /= gcd;
        }
        
        return (num, den);
    }
}
