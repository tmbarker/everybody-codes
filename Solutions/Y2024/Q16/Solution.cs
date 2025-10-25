namespace Solutions.Y2024.Q16;

[QuestInfo("Cat Grin of Fortune", Topics.Math|Topics.Simulation, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var machine = Machine.Parse(input);
        
        return part switch
        {
            1 => machine.Read(spins: 100L),
            2 => machine.ScoreCumulative(spins: 202420242024L),
            3 => machine.FindExtrema(spins: 256L),
            _ => NotSolvedString
        };
    }
}
