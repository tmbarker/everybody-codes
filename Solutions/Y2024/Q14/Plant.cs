using Utilities.Collections;
using Utilities.Geometry;

namespace Solutions.Y2024.Q14;

public class Plant
{
    private readonly HashSet<Vec3D> _segments;
    private readonly HashSet<Vec3D> _leaves;

    public int Height => _segments.Max(segment => segment.Y);
    public int Size   => _segments.Count;
    
    private Plant(HashSet<Vec3D> segments, HashSet<Vec3D> leaves)
    {
        _segments = segments;
        _leaves = leaves;
    }

    public static Plant Parse(string[] input)
    {
        var segments = new HashSet<Vec3D>();
        var leaves = input
            .Select(branch => GrowBranch(branch, segments))
            .ToHashSet();

        return new Plant(segments, leaves);
    }
    
    public int Minimize()
    {
        var costs = new Dictionary<Vec3D, DefaultDict<Vec3D, int>>();
        var trunk = _segments
            .Where(seg => seg is { X: 0, Z: 0 })
            .ToHashSet();

        foreach (var leaf in _leaves)
        {
            var seed = (leaf, 0);
            var goal = new HashSet<Vec3D>();
            var heap = new PriorityQueue<Vec3D, int>([seed]);
            var cost = new DefaultDict<Vec3D, int>(defaultValue: int.MaxValue, [seed]);

            while (heap.Count != 0)
            {
                var pos = heap.Dequeue();
                if (trunk.Contains(pos) && goal.Add(pos) && goal.Count == trunk.Count)
                {
                    break;
                }

                foreach (var adj in pos.GetAdjacentSet(Metric.Taxicab).Where(_segments.Contains))
                {
                    if (cost[pos] + 1 < cost[adj])
                    {
                        cost[adj] = cost[pos] + 1;
                        heap.Enqueue(adj, cost[adj]);
                    }
                }
            }

            costs[leaf] = cost;
        }

        return trunk.Min(candidate => _leaves.Sum(leaf => costs[leaf][candidate]));
    }
    
    private static Vec3D GrowBranch(string branch, HashSet<Vec3D> segments)
    {
        var leaf = Vec3D.Zero;
        foreach (var step in branch.Split(','))
        {
            var k = int.Parse(step[1..]);
            var d = step[0] switch
            {
                'U' => Vec3D.Up,
                'D' => Vec3D.Down,
                'L' => Vec3D.Left,
                'R' => Vec3D.Right,
                'F' => Vec3D.Forward,
                'B' => Vec3D.Back,
                _ => throw new NoSolutionException($"Invalid step in branch [{step}]")
            };

            for (var i = 0; i < k; i++)
            {
                leaf += d;
                segments.Add(leaf);
            }
        }

        return leaf;
    }
}