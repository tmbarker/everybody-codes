using Utilities.Geometry;

namespace Solutions.Y2024.Q18;

public sealed class Farm
{
    private const char Empty = '.';
    private const char Palm = 'P';
    
    public Grid2D<char> Map { get; }
    public HashSet<Vec2D> Palms { get; }
    public HashSet<Vec2D> Starts { get; }

    private Farm(Grid2D<char> map)
    {
        Map = map;
        Palms = map.Where(pos => map[pos] is Palm).ToHashSet();
        Starts = map.Where(pos => map[pos] is Empty && map.EdgeContains(pos)).ToHashSet();
    }
    
    public bool CanWater(Vec2D pos)
    {
        return Map.Contains(pos) && Map[pos] is Empty or Palm;
    }
    
    public bool CanBuildWell(Vec2D pos)
    {
        return Map.Contains(pos) && Map[pos] is Empty;
    }
    
    public static Farm Parse(string[] input)
    {
        return new Farm(map: Grid2D<char>.MapChars(input));
    }
}