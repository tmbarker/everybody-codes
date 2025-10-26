namespace Solutions.Y2024.Q17;

[QuestInfo("Galactic Geometry", Topics.Graphs, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var maxDistance = part == 3 ? 5 : int.MaxValue;
        return Sky
            .Parse(input, maxDistance)
            .GetSize();
    }
}
