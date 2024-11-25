namespace Solutions.Y2024.Q01;

[QuestInfo("The Battle for the Farmlands", Topics.None, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    private static readonly Dictionary<char, int> PotionMap = new()
    {
        { 'A', 0 },
        { 'B', 1 },
        { 'C', 3 },
        { 'D', 5 }
    };
    
    public override object Solve(int part)
    {
        return CountPotions(input: GetInputText(part), groupSize: part);
    }

    private static int CountPotions(string input, int groupSize)
    {
        return input
            .Chunk(size: groupSize)
            .Sum(group =>
            {
                var present = group.Count(c => PotionMap.ContainsKey(c));
                var potions = 0;

                for (var i = 0; i < groupSize; i++)
                {
                    potions += PotionMap.TryGetValue(group[i], out var required)
                        ? required + present - 1
                        : 0;
                }

                return potions;
            });
    }
}