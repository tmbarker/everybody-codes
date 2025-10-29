using Utilities.Extensions;
using Utilities.Geometry.Euclidean;
using Utilities.Numerics;

namespace Solutions.S01.Q03;

[QuestInfo("The Conical Snail Clock", Topics.Math, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private static readonly Vec2D Step = new(X: 1, Y: -1);
    private readonly record struct Congruence(long Remainder, long Modulus);
    
    public override object Solve(int part)
    {
        var snails = ParseInputLines(part, Vec2D.Parse);
        return part switch
        {
            1 => PredictPositions(starts: snails, time: 100),
            2 or 3 => GetFirstSyncTime(starts: snails),
            _ => NotSolvedString
        };
    }

    private static int PredictPositions(Vec2D[] starts, int time)
    {
        return starts
            .Select(start => PredictPos(start, time))
            .Sum(ScorePos);
    }
    
    private static Vec2D PredictPos(Vec2D start, int time)
    {
        var trackNum = GetTrackNumber(start);
        var trackStart = GetTrackStartPos(trackNum);
        var posIndexOnTrack = (start - trackStart).X;
        var newIndexOnTrack = (posIndexOnTrack + time) % trackNum;
        
        return trackStart + newIndexOnTrack * Step;
    }

    private static long GetFirstSyncTime(Vec2D[] starts)
    {
        var congruences = new List<Congruence>();
        foreach (var pos in starts)
        {
            var trackNum = GetTrackNumber(pos);
            var trackStart = GetTrackStartPos(trackNum);
            var posIndexOnTrack = (pos - trackStart).X;
            
            congruences.Add(new Congruence(
                Remainder: (trackNum - 1 - posIndexOnTrack).Modulo(trackNum), 
                Modulus: trackNum));
        }

        return SolveCongruences(congruences);
    }
    
    private static long SolveCongruences(List<Congruence> congruences)
    {
        var a1 = congruences[0].Remainder;
        var m1 = congruences[0].Modulus;
        
        for (var i = 1; i < congruences.Count; i++)
        {
            //  NOTE: We need to iteratively solve both congruences simultaneously:
            //    x ≡ a1 (mod m1)
            //    x ≡ a2 (mod m2)
            // 
            var (a2, m2) = (congruences[i].Remainder, congruences[i].Modulus);
            var gcd = Numerics.Gcd(m1, m2);
            var lcm = Numerics.Lcm(m1, m2);
            
            if ((a2 - a1).Modulo(gcd) != 0)
            {
                throw new NoSolutionException($"Congruences are unsatisfiable:\n\t{a1} (mod {m1})\n\t{a2} (mod {m2})");
            }
            
            //  Solve: m1 * k ≡ (a2 - a1) (mod m2) for k
            //
            var remainderDiff = (a2 - a1).Modulo(m2);
            var reducedM1 = m1 / gcd;
            var reducedM2 = m2 / gcd;
            var reducedDiff = remainderDiff / gcd;
            
            var inverse = Numerics.ModInverse(reducedM1, reducedM2);
            var k = Numerics.ModMultiply(reducedDiff, inverse, reducedM2);
            
            //  Update to combined congruence: x ≡ newRemainder (mod lcm)
            //
            var increment = Numerics.ModMultiply(m1, k, m: lcm);
            a1 = Numerics.ModAdd(a1, increment, m: lcm);
            m1 = lcm;
        }
        
        return a1;
    }
    
    private static int GetTrackNumber(Vec2D pos) => pos.X + pos.Y - 1;
    private static Vec2D GetTrackStartPos(int number) => new(X: 1, Y: number);
    private static int ScorePos(Vec2D pos) => pos.X + 100 * pos.Y;
}
