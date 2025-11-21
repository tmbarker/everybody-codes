using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q14;

[QuestInfo("The Game of Light", Topics.Simulation, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    private static readonly Vec2D[] Diags = [new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)];
    
    public override object Solve(int part)
    {
        var grid = GetInputGrid(part);
        return part switch
        {
            1 => Simulate(grid, rounds: 10L),
            2 => Simulate(grid, rounds: 2025L),
            3 => Calculate(pattern: grid, size: 34, rounds: 1000000000L),
            _ => NotSolvedString
        };
    }

    private static long Simulate(Grid2D<char> grid, long rounds)
    {
        var aabb = new Aabb2D(extents: grid.ToList());
        var totals = 0L;
        var active = grid.FindAll(element: '#').ToHashSet();

        for (var i = 0L; i < rounds; i++)
        {
            var nextActive = Tick(active, aabb);
            totals += nextActive.Count;
            active = nextActive;
        }

        return totals;
    }
    
    private static long Calculate(Grid2D<char> pattern, int size, long rounds)
    {
        var aabb = new Aabb2D(xMin: 0, yMin: 0, xMax: size - 1, yMax: size - 1);
        var offset = new Vec2D(X: (size - pattern.Width) / 2, Y: (size - pattern.Height) / 2);
        var patternOn  = pattern.FindAll(element: '#').Select(pos => pos + offset).ToHashSet();
        var patternOff = pattern.FindAll(element: '.').Select(pos => pos + offset).ToHashSet();
        
        var total   = 0L;
        var partial = 0L;
        
        HashSet<Vec2D> active = [];
        const long period = 4095;
        
        for (var round = 0L; round < period; round++)
        {
            active = Tick(active, aabb);
            if (patternOn.All(active.Contains) && patternOff.All(pos => !active.Contains(pos)))
            {
                total += active.Count;
            }
            
            if (round == rounds % period)
            {
                partial = total;
            }
        }

        return total * rounds / period + partial;
    }
    
    private static HashSet<Vec2D> Tick(HashSet<Vec2D> active, Aabb2D aabb)
    {
        return active
            .Where(pos => Diags.Count(diag => active.Contains(pos + diag)) % 2 == 1)
            .Concat(aabb
                .Where(pos => !active.Contains(pos))
                .Where(pos => Diags.Count(diag => aabb.Contains(pos + diag, inclusive: true) && active.Contains(pos + diag)) % 2 == 0))
            .ToHashSet();
    }
}
