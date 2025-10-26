using Utilities.Collections;
using Utilities.Geometry;

namespace Solutions.Y2024.Q17;

public sealed class Sky
{
    private const char Star = '*';
    private readonly List<Constellation> _constellations;
    
    private Sky(List<Constellation> constellations)
    {
        _constellations = constellations;
    }

    public long GetSize()
    {
        return _constellations
            .Select(constellation => constellation.Size)
            .OrderDescending()
            .Take(3)
            .Aggregate(1L, (acc, size) => acc * size);
    }
    
    public static Sky Parse(string[] input, int maxStarDistance)
    {
        var stars = new List<Vec2D>();
        var allEdges = new List<Constellation.Edge>();
        var mstEdges = new List<Constellation.Edge>();
        
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            if (input[y][x] == Star)
            {
                stars.Add(new Vec2D(x, y));
            }
        }
        
        for (var i = 0; i < stars.Count; i++)
        for (var j = i + 1; j < stars.Count; j++)
        {
            var distance = Vec2D.Distance(stars[i], stars[j], Metric.Taxicab);
            if (Vec2D.Distance(stars[i], stars[j], Metric.Taxicab) <= maxStarDistance)
            {
                allEdges.Add(new Constellation.Edge(stars[i], stars[j], distance));
            }
        }
        
        var disjointSet = new DisjointSet<Vec2D>(stars);
        foreach (var edge in allEdges.OrderBy(edge => edge.Distance))
        {
            if (disjointSet.FindSet(edge.A) != disjointSet.FindSet(edge.B))
            {
                disjointSet.Union(edge.A, edge.B);
                mstEdges.Add(edge);
            }
        }
        
        var constellations = new List<Constellation>();
        var componentGroups = stars
            .GroupBy(star => disjointSet.FindSet(star))
            .ToList();
        
        foreach (var group in componentGroups)
        {
            var componentStars = group.ToHashSet();
            var componentEdges = mstEdges
                .Where(edge => componentStars.Contains(edge.A) && componentStars.Contains(edge.B))
                .ToList();
            
            constellations.Add(new Constellation(componentStars, componentEdges));
        }
        
        return new Sky(constellations);
    }
}

