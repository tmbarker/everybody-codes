namespace Solutions.Template;

[QuestInfo("", Topics.None, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        return part switch
        {
            1 => Part1(),
            2 => Part2(),
            3 => Part3(),
            _ => NotSolvedString
        };
    }
    
    private object Part1()
    {
        return NotSolvedString;
    }
    
    private object Part2()
    {
        return NotSolvedString;
    }

    private object Part3()
    {
        return NotSolvedString;
    }
}