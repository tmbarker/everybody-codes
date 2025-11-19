using Utilities.Extensions;
using Utilities.Numerics;

namespace Solutions.Y2025.Q13;

using Segment = (Range<long> Vals, bool Cw);
using Range   = Range<long>;

[QuestInfo("Unlocking the Mountain", Topics.Simulation, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var ranges = part switch
        {
            1      => ParseInputLines(part, line => Range.Single(line.ParseLong())),
            2 or 3 => ParseInputLines(part, Range.Parse),
            _      => throw new NoSolutionException("Invalid part number")
        };

        return part switch
        {
            1 => Simulate(ranges, turns: 2025L),
            2 => Simulate(ranges, turns: 20252025L),
            3 => Simulate(ranges, turns: 202520252025L),
            _ => throw new NoSolutionException("Invalid part number")
        };
    }
    
    private static long Simulate(Range[] ranges, long turns)
    {
        var dial = new Segment[ranges.Length + 1];
        var remainingIndices = Enumerable.Range(1, ranges.Length).ToHashSet();
        
        dial[0] = (Vals: Range.Single(1L), Cw: true);

        for (var i = 0; i < ranges.Length; i++)
        {
            var clockwise = i % 2 == 0;
            var nextIndex = clockwise
                ? remainingIndices.Min()
                : remainingIndices.Max();

            dial[nextIndex] = new Segment(ranges[i], clockwise);
            remainingIndices.Remove(nextIndex);
        }
        
        var currentSlot = 0L;
        var totalSlots = dial.Sum(segment => segment.Vals.Length);
        var targetSlot = turns % totalSlots;
        
        foreach (var segment in dial)
        {
            var distToTarget = targetSlot - currentSlot;
            if (distToTarget < segment.Vals.Length)
            {
                return segment.Cw
                    ? segment.Vals.Min + distToTarget
                    : segment.Vals.Max - distToTarget;
            }
            
            currentSlot += segment.Vals.Length;
        }

        throw new NoSolutionException("Failed to find final slot");
    }
}
