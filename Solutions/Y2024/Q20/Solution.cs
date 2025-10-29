using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q20;

using Map  = Grid2D<char>;
using Memo = Dictionary<ValueTuple<Pose2D, int>, int>;
using Key  = ValueTuple<Pose2D, Poi.Checkpoints>;

[InputSpecificSolution]
[QuestInfo("Gliding Finale", Topics.Graphs, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var map = Map.MapChars(input);
        
        return part switch
        {
            1 => FindMaxAltitude(t: 100, a: 1000, map),
            2 => FindMinPath(a: 10000, map),
            3 => FindMaxGlide(a: 384400, map),
            _ => NotSolvedString
        };
    }
    
    private static int FindMaxAltitude(int t, int a, Map map)
    {
        return FindMaxAltitude(
            m: 0,
            t: t,
            s: TestState.CreateInitial(map, a),
            map,
            memo: new Memo());
    }
    
    private static int FindMaxAltitude(int m, int t, TestState s, Map map, Memo memo)
    {
        if (t == 0)
        {
            return s.Altitude;
        }
        
        if (s.GetAltitudeUpperBound(t) <= m)
        {
            return m;
        }

        foreach (var adjacent in s.GetAdjacent(t: t - 1, map, memo))
        {
            m = Math.Max(m, FindMaxAltitude(m, t: t - 1, adjacent, map, memo));
        }
        
        return m;
    }
    
    private static int FindMinPath(int a, Map map)
    {
        var initial = RaceState.CreateInitial(map, a);
        var visited = new Dictionary<Key, int>();
        var curLayer = new List<RaceState> { initial };
        
        while (curLayer.Count > 0)
        {
            var nextLayer = new Dictionary<Key, RaceState>();
            foreach (var state in curLayer)
            {
                if (state.Pose.Pos == initial.Pose.Pos &&
                    state.Altitude >= initial.Altitude &&
                    state.Visited == Poi.Checkpoints.All)
                {
                    return state.Time;
                }

                const int altThreshold = 100;
                foreach (var adj in state.GetAdjacent(map))
                {
                    if (adj.Altitude < a - altThreshold || adj.Altitude > a + altThreshold)
                    {
                        continue;
                    }
                    
                    var adjKey = adj.Key;
                    if (visited.TryGetValue(adjKey, out var visitedAlt) &&
                        adj.Altitude <= visitedAlt)
                    {
                        continue;
                    }
                    
                    visited[adjKey] = adj.Altitude;
                    if (!nextLayer.TryGetValue(adjKey, out var layerState) || 
                        adj.Altitude > layerState.Altitude)
                    {
                        nextLayer[adjKey] = adj;
                    }
                }
            }
            
            curLayer = nextLayer.Values.ToList();
        }
        
        throw new NoSolutionException("No valid path found.");
    }

    private static int FindMaxGlide(int a, Map map)
    {
        var startPos = map.Find(Poi.Start);
        var warmCounts = Enumerable.Range(0, map.Width)
            .Select(x => Enumerable.Range(0, map.Height).Count(y => Terrain.IsWarm(map[x, y])))
            .ToArray();
        
        var maxWarm = warmCounts.Max();
        var bestCol = warmCounts
            .Select((count, x) => (count, x))
            .Where(p => p.count == maxWarm)
            .MinBy(p => Math.Abs(startPos.X - p.x)).x;
        
        var warmYOffsets = Enumerable.Range(0, map.Height)
            .Where(y => Terrain.IsWarm(map[bestCol, y]))
            .Select(y => startPos.Y - y)
            .ToHashSet();
        
        var distance = 0;
        var altitude = a - Math.Abs(startPos.X - bestCol);
        while (altitude > 0)
        {
            distance++;
            altitude += warmYOffsets.Contains(distance % map.Height) ? 1 : -1;
        }

        return distance;
    }
}
