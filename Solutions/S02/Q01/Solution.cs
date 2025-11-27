namespace Solutions.S02.Q01;

[QuestInfo("Nail Down Your Luck", Topics.Simulation|Topics.Graphs, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var chunks = ChunkInputByNonEmpty(part);
        var machine = Machine.Parse(chunks[0]);
        var tokens = chunks[1];
        
        return part switch
        {
            1 => DropTokens(machine, tokens),
            2 => MaximizeWinnings(machine, tokens),
            3 => FindExtrema(machine, tokens),
            _ => NotSolvedString
        };
    }
    
    private static int DropTokens(Machine machine, string[] tokens)
    {
        return tokens
            .Select((behaviour, index) => machine.Drop(index, behaviour))
            .Sum();
    }

    private static int MaximizeWinnings(Machine machine, string[] tokens)
    {
        return tokens
            .Select(behaviour => machine.SlotIndices.Max(index => machine.Drop(index, behaviour)))
            .Sum();
    }

    private static string FindExtrema(Machine machine, string[] tokens)
    {
        var slotIndices = machine.SlotIndices.ToArray();
        var memo = new Dictionary<(int, int), (int, int)>();
        var (min, max) = SolveDp(tokenIndex: 0, usedSlotsMask: 0);
        
        return $"{min} {max}";

        (int min, int max) SolveDp(int tokenIndex, int usedSlotsMask)
        {
            if (tokenIndex >= tokens.Length)
            {
                return (min: 0, max: 0);
            }
            
            var key = (tokenIndex, usedSlotsMask);
            if (memo.TryGetValue(key, out var cachedExtrema))
            {
                return cachedExtrema;
            }
            
            var minCoins = int.MaxValue;
            var maxCoins = int.MinValue;
            
            for (var i = 0; i < slotIndices.Length; i++)
            {
                if ((usedSlotsMask & (1 << i)) != 0)
                {
                    continue;
                }
                
                var coins = machine.Drop(slotIndices[i], tokens[tokenIndex]);
                var (nextMin, nextMax) = SolveDp(
                    tokenIndex: tokenIndex + 1,
                    usedSlotsMask: usedSlotsMask | (1 << i));
                
                minCoins = Math.Min(minCoins, coins + nextMin);
                maxCoins = Math.Max(maxCoins, coins + nextMax);
            }
            
            memo[key] = (minCoins, maxCoins);
            return (minCoins, maxCoins);
        }
    }
}
