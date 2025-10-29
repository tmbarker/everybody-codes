using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q12;

[QuestInfo("Desert Shower", Topics.Vectors|Topics.Math, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private static readonly char[] Catapults = ['A', 'B', 'C'];
    
    public override object Solve(int part)
    {
        return part != 3
            ? ShootTargets(part)
            : ShootMeteors();
    }

    private int ShootTargets(int part)
    {
        var total = 0;
        var terrain = ParseTerrain(part);
        var catapults = terrain.Keys
            .Where(pos => Catapults.Contains(terrain[pos]))
            .ToHashSet();
        
        foreach (var target in terrain.Keys.Where(pos => terrain[pos] is 'T' or 'H'))
        foreach (var catapult in catapults)
        {
            var result = Projectile.Query(src: catapult, tgt: target);
            if (result.Success)
            {
                total += result.Power * (terrain[catapult] - 'A' + 1) * (terrain[target] == 'H' ? 2 : 1);
                break;
            }
        }
        
        return total;
    }

    private int ShootMeteors()
    {
        var total = 0;
        var meteors = ParseInputLines(part: 3, parser: Vec2D.Parse);
        var catapults = new Vec2D[]
        {
            new(0, 0),
            new(0, 1),
            new(0, 2)
        };

        foreach (var meteor in meteors)
        foreach (var catapult in catapults)
        {
            var result = Projectile.Query(src: catapult, tgt: new Vec2D(
                X: meteor.X / 2,
                Y: meteor.Y - meteor.X / 2 - meteor.X % 2));
            
            if (result.Success)
            {
                total += result.Power * (1 + catapult.Y);
                break;
            }
        }
        
        return total;
    }
    
    private Dictionary<Vec2D, char> ParseTerrain(int part)
    {
        var map = new Dictionary<Vec2D, char>();
        var lines = GetInputLines(part);
        var rows = lines.Length;
        var cols = lines.Max(s => s.Length);

        for (var row = 0; row < rows; row++)
        for (var col = 0; col < cols; col++)
        {
            map[new Vec2D(X: col, Y: row)] = lines[rows - row - 1].ElementAtOrDefault(col);
        }

        return map;
    }
}
