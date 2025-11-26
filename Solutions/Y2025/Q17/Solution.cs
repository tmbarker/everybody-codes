using System.Numerics;
using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q17;

[QuestInfo("Deadline-Driven Development", Topics.Vectors|Topics.Simulation, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private readonly record struct Key(Vec2D Pos, uint OctantMask, int HorDir, int RadDir);
    private readonly record struct State(Vec2D Pos, int Time, int MinSqrDist, uint OctantMask, int HorDir, int RadDir)
    {
        public static State Initial(Vec2D start) => new(Pos: start, Time: 0, MinSqrDist: int.MaxValue, OctantMask: 0, HorDir: 0, RadDir: 0);
    }
    
    public override object Solve(int part)
    {
        return part switch
        {
            1 => Part1(),
            2 => Part2(),
            3 => Part3(),
            _ => NotSolvedString
        };
    }

    private int Part1()
    {
        var grid = GetInputGrid(part: 1);
        var volcano = grid.Find(element: '@');
        
        return grid
            .Where(pos => pos != volcano && IsDestroyed(volcano, pos, radius: 10))
            .Sum(pos => grid[pos].AsDigit());
    }
    
    private int Part2()
    {
        var grid = GetInputGrid(part: 2);
        var volcano = grid.Find(element: '@');
        
        var destroyed = new HashSet<Vec2D>();
        var scores = new List<(int Lava, int Radius)>();
        var changed = true;

        for (var radius = 1; changed; radius++)
        {
            var countAtTick = destroyed.Count;
            var lava = grid
                .Where(pos => pos != volcano && IsDestroyed(volcano, pos, radius) && destroyed.Add(pos))
                .Sum(pos => grid[pos].AsDigit());
            
            scores.Add((lava, radius));
            changed = destroyed.Count > countAtTick;
        }

        var max = scores.MaxBy(entry => entry.Lava);
        return max.Lava * max.Radius;
    }

    private static bool IsDestroyed(Vec2D volcano, Vec2D target, int radius)
    {
        return
            (volcano.X - target.X) * (volcano.X - target.X) +
            (volcano.Y - target.Y) * (volcano.Y - target.Y) <= radius * radius;
    }
    
    private object Part3()
    {
        var input = GetInputLines(part: 3);
        var grid = Grid2D<int>.MapChars(input, c => c switch
        {
            'S' =>  0,
            '@' => -1,
             _  =>  c.AsDigit()
        });
        
        var start  = grid.Find(element:  0);
        var volcano= grid.Find(element: -1);
        var seen = new Dictionary<Key, (int Time, int MinSqrDist)>();
        var heep = new PriorityQueue<State, int>([(State.Initial(start), 0)]);
        
        Span<Vec2D> neighbors = stackalloc Vec2D[4];
        while (heep.Count > 0)
        {
            var (pos, time, minSqrDist, octantMask, horDir, radDir) = heep.Dequeue();
            var key = new Key(pos, octantMask, horDir, radDir);
            
            if (seen.TryGetValue(key, out var best) && 
                best.Time <= time && 
                best.MinSqrDist >= minSqrDist)
            {
                continue;
            }
            
            seen[key] = (Time: time, MinSqrDist: minSqrDist);
            
            if (pos == start && octantMask == 0b11111111u)
            {
                return time * (time / 30);
            }
            
            neighbors[0] = pos with { Y = pos.Y - 1 };
            neighbors[1] = pos with { Y = pos.Y + 1 };
            neighbors[2] = pos with { X = pos.X - 1 };
            neighbors[3] = pos with { X = pos.X + 1 };
            
            for (var i = 0; i < 4; i++)
            {
                var neighbor = neighbors[i];
                if (!grid.Contains(neighbor) || neighbor == volcano)
                {
                    continue;
                }
                
                var nt = time + grid[neighbor];
                var dx = neighbor.X - volcano.X;
                var dy = neighbor.Y - volcano.Y;
                var sqrDistance = dx * dx + dy * dy;
                var minDistance = Math.Min(minSqrDist, sqrDistance);
                
                var lavaRadius = nt / 30;
                var lavaSqrRadius = lavaRadius * lavaRadius;
                if (minDistance <= lavaSqrRadius)
                {
                    continue;
                }
                
                var octant = GetOctant(dx, dy);
                var newOctantMask = octantMask | (1u << octant);
                if (neighbor != start && !IsValidRadialProgress(octant, octantMask, radDir, out radDir))
                {
                    continue;
                }

                if (neighbor != start && !IsValidHorizontalProgress(dx, horDir, octantMask, out horDir))
                {
                    continue;
                }
                
                heep.Enqueue(new State(
                    Pos: neighbor,
                    Time: nt,
                    MinSqrDist: minDistance,
                    OctantMask: newOctantMask,
                    HorDir: horDir,
                    RadDir: radDir), nt);
            }
        }

        throw new NoSolutionException("No path found");
    }
    
    private static int GetOctant(int dx, int dy)
    {
        var absDx = Math.Abs(dx);
        var absDy = Math.Abs(dy);

        //  NOTE: Octants are numberered from 0 to 7 as the angle
        //  between the volcano and the point increases from 0 to 2π:
        //
        //        \ 2 | 1 /
        //       3 \  |  / 0
        //      ————  @  ————
        //       4 /  |  \ 7
        //        / 5 | 6 \
        //
        return dx switch
        {
            >  0 when dy >= 0 => absDx > absDy ? 0 : 1,
            <= 0 when dy >  0 => absDy > absDx ? 2 : 3,
            <  0              => absDx > absDy ? 4 : 5,
            _                 => absDy > absDx ? 6 : 7
        };
    }
    
    private static bool IsValidRadialProgress(int octant, uint visitedMask, int currentDir, out int newDir)
    {
        newDir = currentDir;

        //  NOTE: Octants 1 and 2 can be freely moved between as the start (and end) point
        //  is on the boundary of these octants. Otherwise, we need to ensure we are moving 
        //  through the octants monotonically.
        //
        if (octant is 1 or 2)
        {
            return true;
        }

        if (visitedMask == 0 || ((1u << octant) & visitedMask) != 0)
        {
            return true;
        }
        
        var highest = 31 - BitOperations.LeadingZeroCount(visitedMask);
        var lowest  = BitOperations.TrailingZeroCount(visitedMask);
        
        return currentDir switch
        {
            0 => TryDetermineDirection(octant, highest, lowest, out newDir),
            1 => octant > highest || (octant == 0 && highest == 7),
            _ => octant < lowest  || (octant == 7 && lowest  == 0)
        };
    }
    
    private static bool TryDetermineDirection(int octant, int highest, int lowest, out int direction)
    {
        direction = (octant, highest, lowest) switch
        {
            (octant: 0, highest: 7, lowest: _) =>  1,
            (octant: 7, highest: _, lowest: 0) => -1,
            _ when octant > highest            =>  1,
            _ when octant < lowest             => -1,
            _                                  =>  0
        };
        return direction != 0;
    }
    
    private static bool IsValidHorizontalProgress(int dx, int currentDir, uint octantMask, out int newDir)
    {
        newDir = currentDir;
        
        //  NOTE: Once we've moved in a horizontal direction, the only time we should
        //  move to the other side of the center line, and therefore the
        //  other side of the volcano, is when we are encircling it in octants 5 and 6.
        //
        if (dx == 0)
        {
            return true;
        }
        
        if (currentDir == 0)
        {
            newDir = Math.Sign(dx);
            return true;
        }
        
        return (octantMask & 0b01100000u) != 0 || Math.Sign(dx) == currentDir;
    }
}
