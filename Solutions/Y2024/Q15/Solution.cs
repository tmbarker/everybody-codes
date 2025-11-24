using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q15;

[QuestInfo("From the Herbalist's Diary", Topics.Graphs, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private readonly record struct State(Vec2D Position, int HerbsMask, int Steps);
    
    public override object Solve(int part)
    {
        var grid = GetInputGrid(part, origin: Origin.Uv);
        var start = GetStart(grid);
        var allHerbsMask = grid
            .Where(p => char.IsLetter(grid[p]))
            .Select(pos => grid[pos])
            .Select(herb => 1 << (herb - 'A'))
            .Aggregate(0, (mask, herbBit) => mask | herbBit);
        
        var maxCollected = 0;
        var visited = new HashSet<(Vec2D Position, int HerbsMask)>();
        var queue = new Queue<State>([new State(start, HerbsMask: 0, Steps: 0)]);
        
        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (state.Position == start && state.HerbsMask == allHerbsMask)
            {
                return state.Steps;
            }
            
            var haveMask = state.HerbsMask;
            if (char.IsLetter(grid[state.Position]))
            {
                var herb = grid[state.Position];
                var herbBit = 1 << (herb - 'A');
                haveMask |= herbBit;
            }
            
            //  NOTE: The inputs for this problem are designed to have very deep search spaces. We need to
            //  aggressively prune states that are unlikely to lead to a solution. Here we skip any state
            //  that is more than two herbs behind the best we've seen so far.
            //
            var collected = int.PopCount(haveMask);
            if (collected + 2 < maxCollected)
            {
                continue;
            }
            
            maxCollected = Math.Max(maxCollected, collected);
            foreach (var adjPos in state.Position.GetAdjacentSet(Metric.Taxicab))
            {
                if (grid.Contains(adjPos) && grid[adjPos] is not ('#' or '~'))
                {
                    var adjKey = (Position: adjPos, HerbsMask: haveMask);
                    if (visited.Add(adjKey))
                    {
                        queue.Enqueue(new State(adjPos, haveMask, state.Steps + 1));
                    }
                }
            }
        }

        throw new NoSolutionException("No path found");
    }
    
    private static Vec2D GetStart(Grid2D<char> grid)
    {
        for (var x = 0; x < grid.Width; x++)
        {
            if (grid[x, 0] is '.')
            {
                return new Vec2D(x, 0);
            }
        }
        
        throw new NoSolutionException("No start position found");
    }
}