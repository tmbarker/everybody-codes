using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q13;

public readonly record struct State(Vec2D Pos, int Height)
{
    public IEnumerable<State> GetAdjacent(Dictionary<Vec2D, int> maze)
    {
        yield return this with { Height = (Height + 1).Modulo(10) };
        yield return this with { Height = (Height - 1).Modulo(10) };

        foreach (var adjPos in Pos.GetAdjacentSet(Metric.Taxicab))
        {
            if (maze.TryGetValue(adjPos, out var adjHeight) && Height == adjHeight)
            {
                yield return this with { Pos = adjPos };
            }
        }
    }
}