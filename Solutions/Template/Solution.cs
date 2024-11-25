namespace Solutions.Template;

[QuestInfo("", Topics.None, Difficulty.Easy)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        return part switch
        {
            _ => NotSolvedString
        };
    }
}