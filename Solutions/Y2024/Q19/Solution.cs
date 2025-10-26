namespace Solutions.Y2024.Q19;

[QuestInfo("Encrypted Duck", Topics.Simulation, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var message = Message.Parse(input);

        return part switch
        {
            1 => message.Decode(rounds: 1L),
            2 => message.Decode(rounds: 100L),
            3 => message.Decode(rounds: 1048576000L),
            _ => NotSolvedString
        };
    }
}
