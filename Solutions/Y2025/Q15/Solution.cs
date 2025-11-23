using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q15;

[QuestInfo("Definitely Not a Maze", Topics.Graphs, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputText(part);
        var steps = input.Split(',');
        var dir = Vec2D.Up;
        var end = Vec2D.Zero;
        
        var walls = new List<(Vec2D from, Vec2D to)>();
        var xs = new SortedSet<int> { -1, 0, 1 };
        var ys = new SortedSet<int> { -1, 0, 1 };
        
        foreach (var step in steps)
        {
            var start = end;
            var turn = step[0];
            var amount = int.Parse(step[1..]);
            
            dir = turn switch
            {
                'R' => Rot3D.N90Z.Transform(dir),
                'L' => Rot3D.P90Z.Transform(dir),
                _ => throw new NoSolutionException("Invalid direction")
            };
            
            end += amount * dir;
            
            walls.Add((from: start, to: end));
            xs.Add(end.X - 1);
            xs.Add(end.X);
            xs.Add(end.X + 1);
            ys.Add(end.Y - 1);
            ys.Add(end.Y);
            ys.Add(end.Y + 1);
        }
        
        var xsList = xs.ToList();
        var ysList = ys.ToList();
        var shrinkX = xsList.Select((x, i) => new { x, i }).ToDictionary(item => item.x, item => item.i);
        var shrinkY = ysList.Select((y, i) => new { y, i }).ToDictionary(item => item.y, item => item.i);
        var grid = Grid2D<char>.WithDimensions(rows: ysList.Count, cols: xsList.Count);
        
        foreach (var (from, to) in walls)
        {
            var x1 = shrinkX[from.X];
            var x2 = shrinkX[to.X];
            var y1 = shrinkY[from.Y];
            var y2 = shrinkY[to.Y];
            
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1, y2);
            
            for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
            {
                grid[new Vec2D(x, y)] = '#';
            }
        }
        
        var startPos = new Vec2D(shrinkX[0], shrinkY[0]);
        var endPos = new Vec2D(shrinkX[end.X], shrinkY[end.Y]);
        var queue = new Queue<(Vec2D pos, int cost)>([(startPos, 0)]);
        
        while (queue.Count > 0)
        {
            var (pos, cost) = queue.Dequeue();
            foreach (var adj in pos.GetAdjacentSet(Metric.Taxicab))
            {
                if (!grid.Contains(adj))
                {
                    continue;
                }
                
                var adjCost = cost + Math.Abs(xsList[pos.X] - xsList[adj.X]) + Math.Abs(ysList[pos.Y] - ysList[adj.Y]);
                if (grid[adj] != '#')
                {
                    grid[adj] = '#';
                    queue.Enqueue((adj, adjCost));
                }
                else if (adj == endPos)
                {
                    return adjCost;
                }
            }
        }

        throw new NoSolutionException("No path found");
    }
}
