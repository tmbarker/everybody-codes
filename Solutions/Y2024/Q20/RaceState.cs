using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q20;

using Map = Grid2D<char>;
using Key  = ValueTuple<Pose2D, Poi.Checkpoints>;

public readonly record struct RaceState(Pose2D Pose, int Altitude, int Time, Poi.Checkpoints Visited)
{
    public static RaceState CreateInitial(Map map, int altitude)
    {
        var startPos = map.Find(Poi.Start);
        var startPose = new Pose2D(startPos, Face: Vec2D.Down);
        return new RaceState(startPose, altitude, Time: 0, Visited: Poi.Checkpoints.None);
    }

    public Key Key => (Pose, Visited);
    
    public IEnumerable<RaceState> GetAdjacent(Map map)
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

            var newVisited = Visited;
            if (Poi.CheckpointsMap.TryGetValue(map[candidate], out var checkpoint))
            {
                newVisited |= checkpoint;
            }
            
            var hasA = newVisited.HasFlag(Poi.Checkpoints.A);
            var hasB = newVisited.HasFlag(Poi.Checkpoints.B);
            var hasC = newVisited.HasFlag(Poi.Checkpoints.C);
            var hasS = newVisited.HasFlag(Poi.Checkpoints.S);
            if ((hasB && !hasA) || (hasC && !hasB) || (hasS && !hasC))
            {
                continue;
            }
            
            var newFace = candidate - Pose.Pos;
            var newPose = new Pose2D(Pos: candidate, Face: newFace);
            yield return new RaceState(newPose, newAltitude, Time: Time + 1, newVisited);
        }
    }
}