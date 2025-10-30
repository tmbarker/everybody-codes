using Utilities.Collections;

namespace Solutions.S02.Q02;

[QuestInfo("The Pocket-Money Popper", Topics.Simulation, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    private const string BoltSequence = "RGB";
    
    public override object Solve(int part)
    {
        var pattern = GetInputText(part);
        return part switch
        {
            1 => PopLine(pattern),
            2 => PopCircle(pattern, repeats: 100),
            3 => PopCircle(pattern, repeats: 100000),
            _ => NotSolvedString
        };
    }

    private static int PopLine(string pattern)
    {
        var boltsFired = 0;
        var balloons = new Queue<char>(collection: pattern);
        
        while (balloons.Count != 0)
        {
            var bolt = BoltSequence[boltsFired++ % BoltSequence.Length];
            do
            {
                var balloon = balloons.Dequeue();
                if (balloon != bolt)
                {
                    break;
                }
            } while (balloons.Count > 0);
        }
        
        return boltsFired;
    }

    private static int PopCircle(string pattern, int repeats)
    {
        var halfIdx= repeats * pattern.Length / 2;
        var halfOne = new Deque<char>();
        var halfTwo = new Deque<char>();
        var boltsFired = 0;
        
        for (var i = 0; i < repeats; i++)
        for (var b = 0; b < pattern.Length; b++)
        {
            var targetIndex = i * pattern.Length + b;
            var targetColor = pattern[targetIndex % pattern.Length];
            var targetDeque = targetIndex < halfIdx ? halfOne : halfTwo;
            
            targetDeque.EnqueueTail(targetColor);
        }
        
        while (halfOne.Count > 0)
        {
            var bolt = BoltSequence[boltsFired++ % BoltSequence.Length];
            var remaining = halfOne.Count + halfTwo.Count;
            var nearColor = halfOne.DequeueHead();
            
            if (bolt == nearColor && remaining % 2 == 0 && halfTwo.Count > 0)
            {
                halfTwo.DequeueHead();
                remaining--;
            }
            
            if (--remaining > 0)
            {
                var targetHalfOneSize = (remaining + 1) / 2;
                while (halfOne.Count < targetHalfOneSize && halfTwo.Count > 0)
                {
                    halfOne.EnqueueTail(halfTwo.DequeueHead());
                }
                while (halfOne.Count > targetHalfOneSize)
                {
                    halfTwo.EnqueueHead(halfOne.DequeueTail());
                }
            }
        }
        
        return boltsFired;
    }
}