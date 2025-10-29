using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities.Collections;
using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q10;

using Shrine = Grid2D<char>;
using CharMap = DefaultDict<char, HashSet<Vec2D>>;

[QuestInfo("Shrine Needs to Shine", Topics.StringParsing, Difficulty.Hard, favourite: true)]
public sealed class Solution : SolutionBase
{
    private const char Unknown = '.';
    private const char Mystery = '?';
    private const char Anchor  = '*';

    private static readonly bool[] InnerMask = [false, false, true, true, true, true, false, false];
    private static readonly bool[] OuterMask = [true, true, false, false, false, false, true, true];
    
    public override object Solve(int part)
    {
        var l = GetInputLines(part);
        var g = Shrine.MapChars(l);
        var w = ExtractWords(g);

        return part == 1
            ? w.Single()
            : w.Sum(GetPower);
    }
    
    private static List<string> ExtractWords(Shrine grid)
    {
        var tiles = FindTiles(grid);
        var words = new List<string>();
        var solved = new HashSet<Aabb2D>();
        var update = true;

        while (update)
        {
            update = false;
            foreach (var tile in tiles)
            {
                if (solved.Contains(tile)) continue;
                if (!Simplify(grid, tile)) continue;

                update = true;

                if (CheckSolved(grid, tile, out var word))
                {
                    solved.Add(tile);
                    words.Add(word);
                }
            }
        }
        
        return words;
    }

    private static bool Simplify(Shrine grid, Aabb2D tile)
    {
        var offset = tile.Min + 2 * Vec2D.One;
        var updated = false;

        for (var dy = 0; dy < 4; dy++)
        for (var dx = 0; dx < 4; dx++)
        {
            var x = offset.X + dx;
            var y = offset.Y + dy;

            if (grid[x, y] != Unknown) continue;

            var symbols = Collect(grid, tile, x, y, InnerMask);
            var options = Collect(grid, tile, x, y, OuterMask);

            if (options.Keys.HasExactlyOne(c => c != Mystery && options[c].Count == 2, out var chr))
            {
                grid[x, y] = chr;
                updated = true;
                continue;
            }

            if (symbols[Unknown].Count > 1 || options[Mystery].Count != 1) continue;
            if (!options.Keys.HasExactlyOne(c => c != Mystery && options[c].Count == 1 && symbols[c].Count == 0,
                    out var infer)) continue;

            grid[x, y] = infer;
            grid[options[Mystery].Single()] = infer;
            updated = true;
        }

        return updated;
    }

    private static CharMap Collect(Shrine grid, Aabb2D tile, int x, int y, bool[] mask)
    {
        var map = new CharMap(defaultSelector: _ => []);

        foreach (var pos in tile.EnumerateRow(y).ZipWhere(mask)) map[grid[pos]].Add(pos);
        foreach (var pos in tile.EnumerateCol(x).ZipWhere(mask)) map[grid[pos]].Add(pos);

        return map;
    }
    
    private static bool CheckSolved(Shrine grid, Aabb2D tile, [NotNullWhen(true)] out string? word)
    {
        var solved = true;
        var sb = new StringBuilder();
        
        foreach (var pos in EnumerateSymbols(tile))
        {
            solved &= grid[pos] != Unknown;
            sb.Append(grid[pos]);

            if (!solved)
            {
                word = null;
                return false;
            }
        }
        
        word = sb.ToString();
        return true;
    }
    
    private static IEnumerable<Vec2D> EnumerateSymbols(Aabb2D tile)
    {
        var offset = tile.Min + 2 * Vec2D.One;
        for (var dy = 3; dy >= 0; dy--)
        for (var dx = 0; dx <= 3; dx++)
        {
            yield return new Vec2D(offset.X + dx, offset.Y + dy);
        }
    }
    
    private static List<Aabb2D> FindTiles(Shrine grid)
    {
        var tiles = new List<Aabb2D>();
        var seeds = grid
            .Where(pos => grid[pos] == Unknown && grid[pos + new Vec2D(X: -1, Y: 1)] == Anchor);

        foreach (var seed in seeds)
        {
            tiles.Add(new Aabb2D(
                xMin: seed.X - 2, xMax: seed.X + 5,
                yMin: seed.Y - 5, yMax: seed.Y + 2));
        }

        return tiles;
    }

    private static long GetPower(string word)
    {
        return word
            .Select((c, i) => (c - 'A' + 1L) * (i + 1L))
            .Sum();
    }
}
