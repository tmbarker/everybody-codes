using Utilities.Extensions;

namespace Solutions.Y2025.Q18;

[QuestInfo("When Roots Remember", Topics.Math|Topics.Graphs, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private readonly record struct Branch(int? ConnectsToId, int Thickness);
    private readonly record struct Plant(int Id, int Thickness, List<Branch> Branches);
    
    public override object Solve(int part)
    {
        var input = ChunkInputByNonEmpty(part);
        return part switch
        {
            1 => Part1(input),
            2 => Part2(input),
            3 => Part3(input),
            _ => NotSolvedString
        };
    }
    
    private static long Part1(List<string[]> input)
    {
        var map = ParsePlants(input);
        return GetEnergy(id: map[^1].Id, map, memo: []);
    }
    
    private static long Part2(List<string[]> input)
    {
        var map = ParsePlants(input[..^1]);
        return input[^1]
            .Select(testCase => testCase.ParseInts())
            .Select(assignment => GetEnergy(id: map[^1].Id, map, assignment))
            .Sum();
    }

    private static long Part3(List<string[]> input)
    {
        var map = ParsePlants(input[..^1]);
        var free = map.Count(plant => plant.Branches.Any(branch => branch.ConnectsToId == null));
        var max = GetMaxEnergy(id: map[^1].Id, map, free);
        
        return input[^1]
            .Select(testCase => testCase.ParseInts())
            .Select(assignment => GetEnergy(id: map[^1].Id, map, assignment))
            .Where(energy => energy > 0)
            .Sum(energy => max - energy);
    }
    
    private static List<Plant> ParsePlants(List<string[]> chunks)
    {
        var map = new List<Plant>();

        foreach (var chunk in chunks)
        {
            var branches = new List<Branch>();
            var pNumbers = chunk[0].ParseInts();
            
            foreach (var line in chunk[1..])
            {
                var bNumbers = line.ParseInts();
                branches.Add(bNumbers.Length == 1
                    ? new Branch(ConnectsToId: null,        Thickness: bNumbers[0])
                    : new Branch(ConnectsToId: bNumbers[0], Thickness: bNumbers[1]));
            }

            map.Add(new Plant(Id: pNumbers[0], Thickness: pNumbers[1], branches));
        }
        
        return map;
    }
    
    private static long GetEnergy(int id, List<Plant> map, int[] assignment)
    {
        var memo = assignment
            .Select((energy, index) => (Id: index + 1, Energy: (long)energy))
            .ToDictionary(
                keySelector:     tuple => tuple.Id,
                elementSelector: tuple => tuple.Energy);
        
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
    
    private static long GetMaxEnergy(int id, List<Plant> map, int freeCount)
    {
        var random = new Random(Seed: 42);
        var maxEnergy = long.MinValue;
        
        for (var trial = 0; trial < 50; trial++)
        {
            var randomStart = Enumerable.Range(0, freeCount).Select(_ => random.Next(2)).ToArray();
            var candidate = SimulatedAnnealing(id, map, randomStart, random);
            var energy = GetEnergy(id, map, candidate);
            
            maxEnergy = Math.Max(energy, maxEnergy);
        }
        
        return maxEnergy;
    }
    
    private static int[] SimulatedAnnealing(int targetId, List<Plant> map, int[] initialAssignment, Random random)
    {
        const double coolingRate = 0.995;
        const double minTemperature = 0.1;
        
        var temperature = 1000.0;
        var maxIterations = Math.Min(10000, initialAssignment.Length * 100);
        
        var assignment = (int[])initialAssignment.Clone();
        var currentEnergy = GetEnergy(targetId, map, assignment);
        var bestAssignment = (int[])assignment.Clone();
        var bestEnergy = currentEnergy;
        
        for (var iteration = 0; iteration < maxIterations && temperature > minTemperature; iteration++)
        {
            var numFlips = random.Next(1, Math.Max(2, initialAssignment.Length / 10));
            var indices = Enumerable.Range(0, initialAssignment.Length)
                .OrderBy(_ => random.Next())
                .Take(numFlips)
                .ToArray();
            
            foreach (var idx in indices)
            {
                assignment[idx] = 1 - assignment[idx];
            }
            
            var newEnergy = GetEnergy(targetId, map, assignment);
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
}