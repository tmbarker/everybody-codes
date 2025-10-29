using Utilities.Collections;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q18;

[QuestInfo("The Ring", Topics.Graphs, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var farm = Farm.Parse(input);
        
        return part switch
        {
            1 or 2 => GetAllWateredTime(farm),
            3 => GetBestWellTime(farm),
            _ => NotSolvedString
        };
    }

    private static int GetAllWateredTime(Farm farm)
    {
        var queue = new Queue<Vec2D>(collection: farm.Starts);
        var visited = new HashSet<Vec2D>(collection: farm.Starts);
        var unwatered = new HashSet<Vec2D>(collection: farm.Palms);
        var time = 0;
        
        while (queue.Count > 0)
        {
            var nodesAtTick = queue.Count;
            while (nodesAtTick-- > 0)
            {
                var pos = queue.Dequeue();
                if (farm.Palms.Contains(pos) && unwatered.Remove(pos) && unwatered.Count == 0)
                {
                    return time;
                }

                var adjacent = pos
                    .GetAdjacentSet(Metric.Taxicab)
                    .Where(farm.CanWater)
                    .Where(visited.Add);
                
                foreach (var adj in adjacent)
                {
                    queue.Enqueue(adj);
                }
            }

            time++;
        }

        throw new NoSolutionException("Some palms could not be watered.");
    }

    private static int GetBestWellTime(Farm farm)
    {
        var totalDistances = new DefaultDict<Vec2D, int>(defaultValue: 0);
        
        foreach (var palm in farm.Palms)
        foreach (var (pos, distance) in ComputeDistancesFromPoint(farm, palm))
        {
            if (farm.CanBuildWell(pos))
            {
                totalDistances[pos] += distance;
            }
        }
        
        return totalDistances.Values.Min();
    }
    
    private static DefaultDict<Vec2D, int> ComputeDistancesFromPoint(Farm farm, Vec2D start)
    {
        var dists = new DefaultDict<Vec2D, int>(defaultValue: 0);
        var queue = new Queue<Vec2D>([start]);
        
        while (queue.Count > 0)
        {
            var curPos = queue.Dequeue();
            var curDistance = dists[curPos];
            
            foreach (var adj in curPos.GetAdjacentSet(Metric.Taxicab))
            {
                if (farm.CanWater(adj) && !dists.ContainsKey(adj))
                {
                    dists[adj] = curDistance + 1;
                    queue.Enqueue(adj);
                }
            }
        }
        
        return dists;
    }
}
