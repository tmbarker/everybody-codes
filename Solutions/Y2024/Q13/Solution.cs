using Utilities.Collections;
using Utilities.Extensions;
using Utilities.Geometry;

namespace Solutions.Y2024.Q13;

[QuestInfo("Never Gonna Let You Down", Topics.Vectors|Topics.Graphs, Difficulty.Medium)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var lines = GetInputLines(part);
        var maze = ParseMaze(lines, out var starts, out var end);
        
        return Navigate(maze, starts, end);
    }

    private static int Navigate(Dictionary<Vec2D, int> maze, List<Vec2D> starts, Vec2D end)
    {
        var heap = new PriorityQueue<State, int>();
        var cost = new DefaultDict<State, int>(defaultValue: int.MaxValue);

        foreach (var seed in starts.Select(start => new State(Pos: start, Height: 0)))
        {
            cost[seed] = 0;
            heap.Enqueue(seed, 0);
        }

        while (heap.Count != 0)
        {
            var state = heap.Dequeue();
            if (state.Pos == end)
            {
                return cost[state];
            }

            foreach (var adj in state.GetAdjacent(maze))
            {
                if (cost[state] + 1 < cost[adj])
                {
                    cost[adj] = cost[state] + 1;
                    heap.Enqueue(adj, cost[adj]);
                }
            }
        }

        throw new NoSolutionException("No path found");
    }

    private static Dictionary<Vec2D, int> ParseMaze(string[] lines, out List<Vec2D> starts, out Vec2D end)
    {
        starts = [];
        end = Vec2D.Zero;
        
        var rows = lines.Length;
        var cols = lines.Max(line => line.Length);
        var maze = new Dictionary<Vec2D, int>();

        for (var y = 0; y < rows; y++)
        for (var x = 0; x < cols; x++)
        {
            switch (lines[rows - y - 1][x])
            {
                case '#':
                case ' ':
                    continue;
                case 'S':
                    starts.Add(new Vec2D(x, y));
                    maze[starts[^1]] = 0;
                    break;
                case 'E':
                    end = new Vec2D(x, y);
                    maze[end] = 0;
                    break;
                default:
                    maze[new Vec2D(x, y)] = lines[rows - y - 1][x].AsDigit();
                    break;
            }
        }

        return maze;
    }
}