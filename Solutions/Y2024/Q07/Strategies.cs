namespace Solutions.Y2024.Q07;

public static class Strategies
{
    public static HashSet<string> Permute()
    {
        return Compute(actionCounts: new Dictionary<string, int>
        {
            { "+", 5 },
            { "-", 3 },
            { "=", 3 }
        });
    }

    private static HashSet<string> Compute(Dictionary<string, int> actionCounts)
    {
        var result = new HashSet<string>();
        var length = actionCounts.Values.Sum();

        GeneratePermutations(actionCounts, permutation: string.Empty, length, result);
        return result;
    }

    private static void GeneratePermutations(
        Dictionary<string, int> actionCounts,
        string permutation,
        int remaining,
        HashSet<string> result)
    {
        if (remaining == 0)
        {
            result.Add(permutation);
            return;
        }

        foreach (var (action, count) in actionCounts)
        {
            if (count <= 0) continue;

            actionCounts[action]--;
            GeneratePermutations(actionCounts, permutation: permutation + action, remaining: remaining - 1, result);
            actionCounts[action]++;
        }
    }
}