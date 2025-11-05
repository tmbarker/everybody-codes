using Utilities.Extensions;

namespace Solutions.Y2025.Q03;

[QuestInfo("The Deepest Fit", Topics.None, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputText(part);
        var crates = input.ParseInts();
        
        return part switch
        {
            1 => crates.Distinct().Sum(),
            2 => crates.Distinct().Order().Take(20).Sum(),
            3 => crates.GroupBy(c => c).Max(g => g.Count()),
            _ => NotSolvedString
        };
    }
}
