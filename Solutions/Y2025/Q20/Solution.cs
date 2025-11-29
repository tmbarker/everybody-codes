namespace Solutions.Y2025.Q20;

[QuestInfo("Dream in Triangles", Topics.Simulation|Topics.Math, Difficulty.Medium, favourite: true)]
public sealed class Solution : SolutionBase
{
    private readonly record struct TriPos(int Row, int Col)
    {
        private bool PointDown => Col % 2 == 0;
        
        public IEnumerable<TriPos> GetAdjacent()
        {
            yield return this with { Col = Col - 1 };
            yield return this with { Col = Col + 1 };

            yield return PointDown
                ? new TriPos(Row - 1, Col + 1)
                : new TriPos(Row + 1, Col - 1);
        }
    }
    
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var triangle = ParseTriangle(input);
        
        return part switch
        {
            1 => Part1(triangle),
            2 => Part2(triangle),
            3 => Part3(),
            _ => NotSolvedString
        };
    }

    private static Dictionary<TriPos, char> ParseTriangle(string[] input)
    {
        var triangle = new Dictionary<TriPos, char>();
        var lines = input
            .Select(line => line.Trim('.'))
            .ToList();
        
        for (var row = 0; row < lines.Count; row++)
        for (var col = 0; col < lines[row].Length; col++)
        {
            triangle.Add(new TriPos(row, col), lines[row][col]);
        }

        return triangle;
    }

    private static int Part1(Dictionary<TriPos, char> triangle)
    {
        return triangle.Keys
            .Where(pos => triangle[pos] is 'T')
            .Sum(pos => pos
                .GetAdjacent()
                .Count(adjPos => triangle.TryGetValue(adjPos, out var adjVal) && adjVal is 'T')) / 2;
    }
    
    private static int Part2(Dictionary<TriPos, char> triangle)
    {
        var start = triangle.Keys.Single(pos => triangle[pos] is 'S');
        var end   = triangle.Keys.Single(pos => triangle[pos] is 'E');
        
        var costs = new Dictionary<TriPos, int> { [start] = 0 };
        var queue = new PriorityQueue<TriPos, int>([(start, 0)]);
        
        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            if (pos == end)
            {
                return costs[pos];
            }
            
            foreach (var adj in pos.GetAdjacent())
            {
                if (!triangle.TryGetValue(adj, out var adjVal))
                {
                    continue;
                }

                if (adjVal is not ('T' or 'E' or 'S'))
                {
                    continue;
                }
                
                if (!costs.TryGetValue(adj, out var cost) || costs[pos] + 1 < cost)
                {
                    costs[adj] = costs[pos] + 1;
                    queue.Enqueue(adj, costs[adj]);
                }
            }
        }

        throw new NoSolutionException("No path found.");
    }

    private static object Part3()
    {
        // TODO: Solve
        return NotSolvedString;
    }
}
