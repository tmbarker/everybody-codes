using System.Diagnostics.CodeAnalysis;
using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q10;

public static class Board
{
    private const char Dragon = 'D';
    private const char Sheep = 'S';
    private const char Hideout = '#';

    private static readonly HashSet<Vec2D> SheepEscapePositions = [];
    private static readonly Dictionary<Vec2D, List<Vec2D>> DragonMoveCache = [];
    private static readonly Vec2D[] DragonMoveDeltas = [new( 2,  1), new( 1,  2), new(-1,  2), new(-2,  1), 
                                                        new(-2, -1), new(-1, -2), new( 1, -2), new( 2, -1)];

    [field: AllowNull, MaybeNull]
    public static Grid2D<char> Grid
    {
        get;
        set
        {
            field = value;
            DragonMoveCache.Clear();
            SheepEscapePositions.Clear();
            SheepEscapePositions.UnionWith(EvaluateSheepEscapePositions());
        }
    }

    public static Vec2D GetDragon() => Grid.Find(Dragon);
    public static HashSet<Vec2D> GetSheep() => [..Grid.FindAll(Sheep)];
    
    public static bool IsHideout(Vec2D pos) => Grid.Contains(pos) && Grid[pos] is Hideout;
    public static bool IsSheep(Vec2D pos) => Grid.Contains(pos) && Grid[pos] is Sheep;
    public static bool IsEscape(Vec2D pos) => SheepEscapePositions.Contains(pos);
    
    
    public static List<Vec2D> GetDragonMoves(Vec2D pos)
    {
        if (!DragonMoveCache.TryGetValue(pos, out var value))
        {
            value = DragonMoveDeltas
                .Select(delta => pos + delta)
                .Where(result => Grid.Contains(result))
                .ToList();
            DragonMoveCache[pos] = value;
        }
        
        return value;
    }
    
    private static HashSet<Vec2D> EvaluateSheepEscapePositions()
    {
        var escapePositions = new HashSet<Vec2D>();
        
        for (var x = 0; x < Grid.Width; x++)
        for (var y = 0; y < Grid.Height; y++)
        {
            var pos = new Vec2D(x, y);
            var below = pos + Vec2D.Down;
            
            if (IsHideout(pos) && (!Grid.Contains(below) || escapePositions.Contains(below)))
            {
                escapePositions.Add(pos);
            }
        }

        return escapePositions;
    }
}