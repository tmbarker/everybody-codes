using System.Numerics;
using Utilities.Extensions;

namespace Solutions.S01.Q01;

[QuestInfo("EniCode", Topics.Math, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    private delegate BigInteger EniFunc(BigInteger n, BigInteger exp, BigInteger mod);
    private static readonly BigInteger One = BigInteger.One;
    
    public override object Solve(int part)
    {
        var strLines = GetInputLines(part);
        var numLines = strLines
            .Select(line => line.ParseNumbers<BigInteger>())
            .ToArray();

        return part switch
        {
            1 => TestEni(numLines, EniNaive),
            2 => TestEni(numLines, EniModPow),
            3 => TestEni(numLines, EniSum),
            _ => NotSolvedString
        };
    }

    private static BigInteger TestEni(BigInteger[][] lines, EniFunc eni)
    {
        return lines
            .Select(nums =>
                { 
                    var a = nums[0]; var b = nums[1]; var c = nums[2]; 
                    var x = nums[3]; var y = nums[4]; var z = nums[5];
                    var mod = nums[6];
                    return eni(n: a, exp: x, mod) + eni(n: b, exp: y, mod) + eni(n: c, exp: z, mod);
                })
            .Max();
    }
    
    private static BigInteger EniNaive(BigInteger n, BigInteger exp, BigInteger mod)
    {
        return EniSeeded(seed: BigInteger.One, n, exp, mod);
    }
    
    private static BigInteger EniModPow(BigInteger n, BigInteger exp, BigInteger mod)
    {
        var rems = new BigInteger(5);
        var seed = BigInteger.ModPow(value: n, exponent: exp - rems, modulus: mod);
        return EniSeeded(seed, n, exp: rems, mod);
    }

    private static BigInteger EniSeeded(BigInteger seed, BigInteger n, BigInteger exp, BigInteger mod)
    {
        var rem = new Stack<BigInteger>();
        var cur = seed;
        
        for (var i = BigInteger.Zero; i < exp; i++)
        {
            cur = cur * n % mod;
            rem.Push(cur);
        }
        
        return BigInteger.Parse(string.Concat(rem));
    }
    
    private static BigInteger EniSum(BigInteger n, BigInteger exp, BigInteger mod)
    {
        var seen = new Dictionary<BigInteger, BigInteger>();
        var rems = new List<BigInteger>();
        var cur = BigInteger.One;
        
        for (var position = BigInteger.One; position <= exp; position++)
        {
            cur = cur * n % mod;
            if (seen.TryGetValue(cur, out var cycleStart))
            {
                var preCycleSum = BigInteger.Zero;
                var cycleSum = BigInteger.Zero;
                var trailSum = BigInteger.Zero;
                
                for (var i = 0; i < cycleStart - One; i++)
                {
                    preCycleSum += rems[i];
                }
                
                for (var i = cycleStart - One; i < position - One; i++)
                {
                    cycleSum += rems[(int)i];
                }
                
                var cycleLength = position - cycleStart;
                var remainingIterations = exp - cycleStart + One;
                var numCycles = remainingIterations / cycleLength;
                var trailLength = remainingIterations % cycleLength;
                
                for (var i = 0; i < trailLength; i++)
                {
                    trailSum += rems[(int)(cycleStart - One + i)];
                }
                
                return preCycleSum + cycleSum * numCycles + trailSum;
            }
            
            seen[cur] = position;
            rems.Add(cur);
        }
        
        return rems.Aggregate(BigInteger.Add);
    }
}
