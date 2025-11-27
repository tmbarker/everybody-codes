using Utilities.Extensions;

namespace Solutions.Y2025.Q18;

[QuestInfo("When Roots Remember", Topics.Math|Topics.Graphs, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private readonly record struct Branch(int? ConnectsToId, int Thickness);
    private readonly record struct Plant(int Id, int Thickness, List<Branch> Branches);
    
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
    
    private long Part1()
    {
        var input = GetInputLines(part: 1);
        var chunks = input.ChunkByNonEmpty();
        var map = ParsePlants(chunks);
        
        return GetEnergy(id: map[^1].Id, map, memo: []);
    }

    private static List<Plant> ParsePlants(IList<string[]> chunks)
    {
        var map = new List<Plant>();

        foreach (var chunk in chunks)
        {
            var branches = new List<Branch>();
            var plantData = chunk[0].ParseInts();
            var id = plantData[0];
            var thickness = plantData[1];
            
            foreach (var line in chunk.Skip(1))
            {
                var nums = line.ParseInts();
                branches.Add(nums.Length == 1
                    ? new Branch(ConnectsToId: null, Thickness: nums.Single())
                    : new Branch(ConnectsToId: nums[0], nums[1]));
            }

            map.Add(new Plant(id, thickness, branches));
        }
        
        return map;
    }
    
    private long Part2()
    {
        var input = GetInputLines(part: 2);
        var chunks = input.ChunkByNonEmpty().ToArray();
        var map = ParsePlants(chunks[..^1]);
        var last = map[^1].Id;
        
        var freePlants = map
            .Where(plant => plant.Branches.Any(branch => branch.ConnectsToId == null))
            .ToList();

        return chunks[^1]
            .Select(testCase => testCase.ParseInts())
            .Select(assignment => GetEnergy(last, map, freePlants, assignment))
            .Sum();
    }

    private long Part3()
    {
        var input = GetInputLines(part: 3);
        var chunks = input.ChunkByNonEmpty().ToArray();
        var map = ParsePlants(chunks[..^1]);
        var last = map[^1].Id;
        
        var freePlants = map
            .Where(plant => plant.Branches.Any(branch => branch.ConnectsToId == null))
            .ToList();
        
        var max = GetMaxEnergy(last, map, freePlants);
        
        return chunks[^1]
            .Select(testCase => testCase.ParseInts())
            .Select(assignment => GetEnergy(last, map, freePlants, assignment))
            .Where(energy => energy > 0)
            .Sum(energy => max - energy);
    }
    
    private static long GetMaxEnergy(int targetId, List<Plant> map, List<Plant> freePlants)
    {
        var random = new Random(42);
        var bestEnergy = long.MinValue;
        
        for (var trial = 0; trial < 50; trial++)
        {
            var randomStart = Enumerable.Range(0, freePlants.Count).Select(_ => random.Next(2)).ToArray();
            var candidate = SimulatedAnnealing(targetId, map, freePlants, randomStart, random);
            var energy = GetEnergy(targetId, map, freePlants, candidate);
            
            bestEnergy = Math.Max(energy, bestEnergy);
        }
        
        return bestEnergy;
    }
    
    private static int[] SimulatedAnnealing(int targetId, List<Plant> map, List<Plant> freePlants, 
        int[] initialAssignment, Random random)
    {
        const double coolingRate = 0.995;
        const double minTemperature = 0.1;
        
        var temperature = 1000.0;
        var maxIterations = Math.Min(10000, freePlants.Count * 100);
        
        var assignment = (int[])initialAssignment.Clone();
        var currentEnergy = GetEnergy(targetId, map, freePlants, assignment);
        var bestAssignment = (int[])assignment.Clone();
        var bestEnergy = currentEnergy;
        
        for (var iteration = 0; iteration < maxIterations && temperature > minTemperature; iteration++)
        {
            var numFlips = random.Next(1, Math.Max(2, freePlants.Count / 10));
            var indices = Enumerable.Range(0, freePlants.Count)
                .OrderBy(_ => random.Next())
                .Take(numFlips)
                .ToArray();
            
            foreach (var idx in indices)
            {
                assignment[idx] = 1 - assignment[idx];
            }
            
            var newEnergy = GetEnergy(targetId, map, freePlants, assignment);
            var delta = newEnergy - currentEnergy;
            
            if (delta > 0 || random.NextDouble() < Math.Exp(delta / temperature))
            {
                currentEnergy = newEnergy;
                if (currentEnergy > bestEnergy)
                {
                    bestEnergy = currentEnergy;
                    bestAssignment = (int[])assignment.Clone();
                }
            }
            else
            {
                foreach (var idx in indices)
                {
                    assignment[idx] = 1 - assignment[idx];
                }
            }
            
            temperature *= coolingRate;
        }
        
        return bestAssignment;
    }
    
    private static long GetEnergy(int id, List<Plant> map, List<Plant> freePlants, int[] assignment)
    {
        var memo = new Dictionary<int, long>();
        for (var i = 0; i < freePlants.Count; i++)
        {
            memo[freePlants[i].Id] = assignment[i];
        }
        return GetEnergy(id, map, memo);
    }
    
    private static long GetEnergy(int id, List<Plant> map, Dictionary<int, long> memo)
    {
        if (memo.TryGetValue(id, out var cachedEnergy))
        {
            return cachedEnergy;
        }

        var plant = map[id - 1];
        var incoming = 0L;
        
        foreach (var branch in plant.Branches)
        {
            if (branch.ConnectsToId == null)
            {
                incoming += branch.Thickness;
                continue;
            }
            
            incoming +=  branch.Thickness * GetEnergy(branch.ConnectsToId.Value, map, memo);
        }

        var energy = incoming < plant.Thickness ? 0L : incoming;
        memo[id] = energy;
        return energy;
    }
}