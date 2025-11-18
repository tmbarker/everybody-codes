using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q12;

[QuestInfo("One Spark to Burn Them All", Topics.Simulation, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var grid = GetInputGrid(part);
        return part switch
        {
            1 => Part1(grid),
            2 => Part2(grid),
            3 => Part3(grid),
            _ => NotSolvedString
        };
    }
    
    private static int Part1(Grid2D<char> grid)
    {
        var start = new Vec2D(X: 0, Y: grid.Height - 1);
        return Ignite(grid: grid, starts: [start], extinguished: []).Count;
    }
    
    private static int Part2(Grid2D<char> grid)
    {
        var start1 = new Vec2D(X: 0, Y: grid.Height - 1);
        var start2 = new Vec2D(X: grid.Width - 1, Y: 0);
        return Ignite(grid: grid, starts: [start1, start2], extinguished: []).Count;
    }
    
    private static int Part3(Grid2D<char> grid)
    {
        HashSet<Vec2D> starts = [];
        HashSet<Vec2D> extinguished = [];
        
        var localMaximums = GetLocalMaximums(grid);
        for (var i = 0; i < 3; i++)
        {
            var bestSet = new HashSet<Vec2D>();
            var bestPos = Vec2D.Zero;
            
            foreach (var pos in localMaximums.Where(candidate => !extinguished.Contains(candidate) && !starts.Contains(candidate)))
            {
                var ignited = Ignite(grid, starts: [pos], extinguished);
                if (ignited.Count > bestSet.Count)
                {
                    bestSet = ignited;
                    bestPos = pos;
                }
            }
            
            starts.Add(bestPos);
            extinguished.UnionWith(bestSet);
        }
        
        return extinguished.Count;
    }
    
    private static List<Vec2D> GetLocalMaximums(Grid2D<char> grid) {
        return grid
            .Where(pos => pos.GetAdjacentSet(Metric.Taxicab)
                .Where(grid.Contains)
                .All(adj => grid[adj] <= grid[pos]))
            .ToList();
    }

    private static HashSet<Vec2D> Ignite(Grid2D<char> grid, HashSet<Vec2D> starts, HashSet<Vec2D> extinguished)
    {
        var queue = new Queue<Vec2D>(collection: starts);
        var ignited = new HashSet<Vec2D>(collection: starts);

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var adjCandidates = cur
                .GetAdjacentSet(Metric.Taxicab)
                .Where(grid.Contains)
                .Where(adj => !extinguished.Contains(adj) && grid[cur] >= grid[adj]);

            foreach (var adj in adjCandidates)
            {
                if (ignited.Add(adj))
                {
                    queue.Enqueue(adj);
                }
            }
        }
        
        return ignited;
    }
}
