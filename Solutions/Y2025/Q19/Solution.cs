using Utilities.Collections;
using Utilities.Extensions;
using Utilities.Numerics;

namespace Solutions.Y2025.Q19;

[QuestInfo("Flappy Quack", Topics.Simulation|Topics.Math, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    private readonly record struct State(int X, int Y, int Flaps)
    {
        public static State Initial => new(0, 0, 0);
    }
    
    public override object Solve(int part)
    {
        var states = new List<State> { State.Initial };
        var doorMap = new DefaultDict<int, List<Range<int>>>(defaultSelector: _ => []);
        
        foreach (var line in GetInputLines(part))
        {
            var nums = line.ParseInts();
            var x = nums[0];
            var y = nums[1];
            var h = nums[2];
            
            doorMap[x].Add(new Range<int>(min: y, max: y + h - 1));
        }
        
        foreach (var (doorX, doors) in doorMap.OrderBy(kvp => kvp.Key))
        {
            states = doors
                .SelectMany(doorYs => doorYs)
                .Select(targetY => (Y: targetY, Cost: GetMinFlaps(states, doorX, targetY)))
                .Where(result => result.Cost.HasValue)
                .Select(result => new State(doorX, result.Y, result.Cost!.Value))
                .ToList();
        }

        return states.Min(state => state.Flaps);
    }

    private static int? GetMinFlaps(List<State> states, int targetX, int targetY)
    {
        var costs = states
            .Where(state  => CanReach(state, targetX, targetY))
            .Select(state => CalculateFlaps(state, targetX, targetY))
            .ToList();

        return costs.Count > 0 ? costs.Min() : null;
    }

    private static bool CanReach(State state, int targetX, int targetY)
    {
        var dx = targetX - state.X;
        var dy = Math.Abs(targetY - state.Y);
        var congruent = (targetY - state.Y - dx) % 2 == 0;
        
        return dx >= dy && congruent;
    }

    private static int CalculateFlaps(State state, int targetX, int targetY)
    {
        var dx = targetX - state.X;
        var dy = Math.Abs(targetY - state.Y);
        var dxFlaps = (dx - dy) / 2;
        var dyFlaps = Math.Max(0, targetY - state.Y);
        
        return state.Flaps + dyFlaps + dxFlaps;
    }
}
