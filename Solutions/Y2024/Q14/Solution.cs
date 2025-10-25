namespace Solutions.Y2024.Q14;

[QuestInfo("The House of Palms", Topics.Vectors, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var plant = Plant.Parse(input);
        
        return part switch
        {
            1 => plant.Height,
            2 => plant.Size,
            3 => plant.Minimize(),
            _ => NotSolvedString
        };
    }
}