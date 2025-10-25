using Utilities.Geometry;

namespace Solutions.Y2024.Q03;

[QuestInfo("Mining Maestro", Topics.Vectors, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        return SumArea(part, metric: part != 3
            ? Metric.Taxicab
            : Metric.Chebyshev);
    }

    private int SumArea(int part, Metric metric)
    {
        var input = GetInputLines(part);
        var depth = 1;
        var grid = Grid2D<int>.MapChars(input, elementFunc: c => c == '#' ? 1 : 0);
        var targets = GetTargets(grid, metric, depth);
        
        while (targets.Count != 0)
        {
            depth++;
            foreach (var target in targets)
            {
                grid[target] = depth;
            }

            targets = GetTargets(grid, metric, depth);
        }

        return grid.Sum(pos => grid[pos]);
    }

    private static HashSet<Vec2D> GetTargets(Grid2D<int> grid, Metric metric, int depth)
    {
        return grid
            .Where(pos => grid[pos] == depth && pos
                .GetAdjacentSet(metric)
                .All(adj => grid.Contains(adj) && grid[adj] == depth))
            .ToHashSet();
    }
}
