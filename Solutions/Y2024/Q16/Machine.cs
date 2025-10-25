using Utilities.Collections;
using Utilities.Extensions;
using Utilities.Numerics;

namespace Solutions.Y2024.Q16;

public sealed class Machine
{
    private const int WheelWidth = 3;
    private const int WheelPadding = 1;
    private const int ScoreThreshold = 3;
    private const char SlotSeparator = ' ';
    
    private readonly record struct Wheel(long SlotsPerSpin, List<string> Symbols);
    private readonly List<Wheel> _wheels;

    private Machine(List<Wheel> wheels)
    {
        _wheels = wheels;
    }

    private IEnumerable<string> GetSymbols(long spins)
    {
        return _wheels
            .Select(wheel => wheel.Symbols[(int)(spins * wheel.SlotsPerSpin).Modulo(wheel.Symbols.Count)]);
    }

    private static long CalculateScore(IEnumerable<string> symbols)
    {
        var counts = new DefaultDict<char, long>(defaultValue: 0L);
        foreach (var symbol in symbols)
        {
            counts[symbol[0]]++;
            counts[symbol[^1]]++;
        }

        return counts
            .Where(kvp => kvp.Value >= ScoreThreshold)
            .Sum(kvp => kvp.Value - ScoreThreshold + 1);
    }

    private long Score(long spins)
    {
        return CalculateScore(GetSymbols(spins));
    }

    private long ScoreFromState(int[] state)
    {
        return CalculateScore(_wheels.Select((wheel, i) => wheel.Symbols[state[i]]));
    }

    public string Read(long spins)
    {
        return string.Join(SlotSeparator, GetSymbols(spins));
    }

    public long ScoreCumulative(long spins)
    {
        var cycleLength = Numerics.Lcm(_wheels
            .Select(wheel => wheel.Symbols.Count / Numerics.Gcd(wheel.Symbols.Count, wheel.SlotsPerSpin))
            .ToArray());

        if (spins <= cycleLength)
        {
            return Simulate(spins);
        }

        var cycScore = Simulate(cycleLength);
        var remScore = Simulate(spins % cycleLength);
        return cycScore * (spins / cycleLength) + remScore;
    }

    private long Simulate(long spins)
    {
        return Enumerable
            .Range(1, (int)spins)
            .Sum(s => Score(s));
    }

    public string FindExtrema(long spins)
    {
        var memo = new Dictionary<(string, long), (long min, long max)>();
        var (min, max) = FindExtremaRecursive(new int[_wheels.Count], spins, memo);
        return $"{max} {min}";
    }
    
    private (long min, long max) FindExtremaRecursive(int[] state, long remainingSpins, 
        Dictionary<(string, long), (long, long)> memo)
    {
        if (remainingSpins == 0)
        {
            return (0, 0);
        }
        
        var key = (string.Join(SlotSeparator, state), remainingSpins);
        if (memo.TryGetValue(key, out var cached))
        {
            return cached;
        }
        
        var (minTotal, maxTotal) = (long.MaxValue, long.MinValue);
        foreach (var action in new[] { 0, 1, -1 })
        {
            var nextState = ApplyAction(state, action);
            var currentScore = ScoreFromState(nextState);
            var (futureMin, futureMax) = FindExtremaRecursive(nextState, remainingSpins - 1, memo);
            
            minTotal = Math.Min(minTotal, currentScore + futureMin);
            maxTotal = Math.Max(maxTotal, currentScore + futureMax);
        }
        
        return memo[key] = (minTotal, maxTotal);
    }

    private int[] ApplyAction(int[] state, int action)
    {
        return state
            .Select((pos, i) => (int)(pos + _wheels[i].SlotsPerSpin + action).Modulo(_wheels[i].Symbols.Count))
            .ToArray();
    }
    
    public static Machine Parse(string[] lines)
    {
        var spinsData = lines[0].ParseLongs();
        var wheelData = lines[2..];
        var numWheels = (wheelData[0].Length + 1) / (WheelWidth + WheelPadding);
        var wheels = new List<Wheel>();
        
        for (var w = 0; w < numWheels; w++)
        {
            var symbols = new List<string>();
            var slotsPerSpin = spinsData[w];
            var slotStartIndex = w * (WheelWidth + WheelPadding);
            
            foreach (var line in wheelData)
            {
                if (slotStartIndex >= line.Length)
                {
                    continue;
                }
                
                var candidate = line.Substring(slotStartIndex, WheelWidth);
                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    symbols.Add(candidate);
                }
            }
            
            wheels.Add(new Wheel(slotsPerSpin, symbols));
        }
        
        return new Machine(wheels);
    }
}