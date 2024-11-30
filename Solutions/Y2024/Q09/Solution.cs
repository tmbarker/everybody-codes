namespace Solutions.Y2024.Q09;

[QuestInfo("Sparkling Bugs", Topics.Math | Topics.Recursion, Difficulty.Easy, favourite: true)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        return part != 3
            ? Minimize(part)
            : Maximize();
    }

    private long Minimize(int part)
    {
        var stamps = Stamps.Get(part);
        var memo = stamps.ToDictionary(
            keySelector: stamp => stamp,
            elementSelector: _ => 1L);

        return GetInputLines(part)
            .Select(long.Parse)
            .Sum(value => GetMin(value, stamps, memo));
    }

    private long Maximize()
    {
        var stamps = Stamps.Get(part: 3);
        var memo = stamps.ToDictionary(
            keySelector: stamp => stamp,
            elementSelector: _ => 1L);

        var total = 0L;
        foreach (var number in ParseInputLines(part: 3, parser: long.Parse))
        {
            var min = long.MaxValue;
            foreach (var (a, b) in Split(number, delta: 100L))
            {
                var aMin = GetMin(value: a, stamps, memo);
                var bMin = GetMin(value: b, stamps, memo);

                min = Math.Min(min, aMin + bMin);
            }

            total += min;
        }

        return total;
    }

    private static IEnumerable<(long, long)> Split(long number, long delta)
    {
        var aMin = Math.Max(0L, (number - delta) / 2L);
        var aMax = Math.Min(number, (number + delta) / 2L);
        
        for (var a = aMin; a <= aMax; a++)
        {
            var b = number - a;
            if (Math.Abs(a - b) <= delta)
            {
                yield return (a, b);
            }
        }
    }
    
    private static long GetMin(long value, long[] stamps, Dictionary<long, long> memo)
    {
        if (memo.TryGetValue(value, out var cached)) return cached;

        var min = long.MaxValue;
        foreach (var stamp in stamps)
            if (value >= stamp)
                min = Math.Min(min, 1L + GetMin(value - stamp, stamps, memo));

        memo[value] = min;
        return min;
    }
}
