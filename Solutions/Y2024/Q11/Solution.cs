using Utilities.Collections;

namespace Solutions.Y2024.Q11;

[QuestInfo("Biological Warfare", Topics.Simulation, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var spawns = BuildSpawnMap(input);
        
        return part switch
        {
            1 => Simulate(spawns, days: 4,  seed: "A"),
            2 => Simulate(spawns, days: 10, seed: "Z"),
            3 => GetMaxDelta(spawns, days: 20),
            _ => NotSolvedString
        };
    }
    
    private static long Simulate(Dictionary<string, string[]> spawns, int days, string seed)
    {
        var counts = new DefaultDict<string, long>(defaultValue: 0L) { [seed] = 1 };
        
        for (var i = 0; i < days; i++)
        {
            var next = new DefaultDict<string, long>(defaultValue: 0L);
            
            foreach (var (type, count) in counts)
            foreach (var spawn in spawns[type])
            {
                next[spawn] += count;
            }

            counts = next;
        }

        return counts.Values.Sum();
    }

    private static long GetMaxDelta(Dictionary<string, string[]> spawns, int days)
    {
        var map = spawns.ToDictionary(
            kvp => kvp.Key,
            kvp => Simulate(spawns, days, seed: kvp.Key));

        return map.Values.Max() - map.Values.Min();
    }
    
    private static Dictionary<string, string[]> BuildSpawnMap(string[] input)
    {
        var spawns = new Dictionary<string, string[]>();
        foreach (var line in input)
        {
            var components = line.Split(':');
            spawns[components[0]] = components[^1].Split(',').ToArray();
        }
        return spawns;
    }
}
