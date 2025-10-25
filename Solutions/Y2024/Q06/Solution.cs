using Utilities.Collections;

namespace Solutions.Y2024.Q06;

using Graph = DefaultDict<string, HashSet<string>>;
using State = (string Node, string Path);

[QuestInfo("The Tree of Titans", Topics.Graphs, Difficulty.Easy, favourite: true)]
public sealed class Solution : SolutionBase
{
    private const string Root = "RR";
    private const string Fruit = "@";

    private static readonly HashSet<string> Pests = ["ANT", "BUG"];
    
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var graph = ParseGraph(input);

        return part == 1
            ? FindDistinct(graph, pathFormatter: s => s)
            : FindDistinct(graph, pathFormatter: s => new string(s[0], count: 1));
    }

    private static string FindDistinct(Graph graph, Func<string, string> pathFormatter)
    {
        var rootPath = pathFormatter(Root);
        var queue = new Queue<State>(collection: [(Node: Root, Path: rootPath)]);
        var paths = new HashSet<string>();

        while (queue.Count != 0)
        {
            var (node, path) = queue.Dequeue();
            if (node == Fruit)
            {
                paths.Add(path);
            }

            foreach (var adj in graph[node])
            {
                queue.Enqueue((Node: adj, Path: string.Concat(path, pathFormatter(adj))));
            }
        }

        return paths
            .GroupBy(path => path.Length)
            .MinBy(grouping => grouping.Count())!
            .Single();
    }

    private static Graph ParseGraph(string[] input)
    {
        var adjacency = new Graph(_ => []);
        foreach (var line in input)
        {
            var components = line.Split(':');
            var branch = components[0];
            var nodes = components[^1].Split(',');

            if (Pests.Contains(branch))
            {
                continue;
            }
            
            foreach (var node in nodes.Where(n => !Pests.Contains(n)))
            {
                adjacency[components[0]].Add(node);
            }
        }

        return adjacency;
    }
}
