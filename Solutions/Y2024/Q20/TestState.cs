using Utilities.Geometry;

namespace Solutions.Y2024.Q20;

using Map = Grid2D<char>;
using TestMemo = Dictionary<ValueTuple<Pose2D, int>, int>;

public readonly record struct TestState(Pose2D Pose, int Altitude)
{
    public static TestState CreateInitial(Map map, int altitude)
    {
        var startPos = map.Find(Poi.Start);
        var startPose = new Pose2D(startPos, Face: Vec2D.Down);
        return new TestState(startPose, Altitude: altitude);
    }
    
    public int GetAltitudeUpperBound(int t)
    {
        return Altitude + t * Terrain.GetMaxGain();
    }
    
    public IEnumerable<TestState> GetAdjacent(int t, Map map, TestMemo memo)
    {
        Vec2D[] candidates = [Pose.Left, Pose.Ahead, Pose.Right];
        foreach (var candidate in candidates)
        {
            if (!map.Contains(candidate) || !Terrain.IsFlyable(map[candidate], out var dA))
            {
                continue;
            }
            
            var newAltitude = Altitude + dA;
            if (newAltitude <= 0)
            {
                continue;
            }
            
            var newFace = candidate - Pose.Pos;
            var newPose = new Pose2D(Pos: candidate, Face: newFace);
            var newState = new TestState(newPose, newAltitude);
            
            var key = (newPose, nextTime: t);
            if (memo.TryGetValue(key, out var memoizedAltitude) && memoizedAltitude >= newAltitude)
            {
                continue;
            }
            
            memo[key] = newAltitude;
            yield return newState;
        }
    }
}