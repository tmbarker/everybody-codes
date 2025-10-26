using Utilities.Geometry;

namespace Solutions.Y2024.Q17;

public sealed class Constellation
{
    public readonly record struct Edge(Vec2D A, Vec2D B, int Distance);
    
    private readonly HashSet<Vec2D> _stars;
    private readonly List<Edge> _edges;
    
    public int Size => _stars.Count + _edges.Sum(edge => edge.Distance);

    internal Constellation(HashSet<Vec2D> stars, List<Edge> edges)
    {
        _stars = stars;
        _edges = edges;
    }
}