using Utilities.Collections;
using Utilities.Extensions;

namespace Solutions.Y2024.Q05;

using State = List<List<int>>;

[QuestInfo("Pseudo-Random Clap Dance", Topics.Math|Topics.Simulation, Difficulty.Medium)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var numbers = ParseInputLines(part, parser: line => line.ParseInts());
        var state = ParseColumns(numbers);
        
        return part switch
        {
            1 => SimulateFixed(state, rounds: 10),
            2 => SimulateUntil(state, repeats: 2024),
            3 => FindMax(state),
            _ => NotSolvedString
        };
    }
    
    private static long SimulateFixed(State columns, int rounds)
    {
        for (var r = 1; r <= rounds; r++)
        {
            Tick(columns, round: r);
        }

        return Shout(columns);
    }
    
    private static long SimulateUntil(State columns, int repeats)
    {
        var counts = new DefaultDict<long, int>(defaultValue: 0);
        for (var r = 1; r < int.MaxValue; r++)
        {
            Tick(columns, round: r);

            var shout = Shout(columns);
            counts[shout] += 1;
            
            if (counts[shout] == repeats)
            {
                return r * shout;
            }
        }
        
        throw new NoSolutionException("No solution found");
    }
    
    private static long FindMax(State columns)
    {
        var cache = new HashSet<long>();
        var ticks = 0;
        
        for (var r = 1; r < int.MaxValue; r++)
        {
            var shout = Shout(columns: Tick(columns, round: r));
            if (cache.Add(shout))
            {
                ticks = 0;
            }

            if (++ticks > 1000)
            {
                return cache.Max();
            }
        }

        throw new NoSolutionException("No solution found");
    }

    private static long Shout(State columns)
    {
        var result = 0L;
        foreach (var column in columns)
        {
            var number = column[0];
            var digits = number != 0
                ? (int)Math.Log10(number) + 1
                : 1;
            
            result = result * (long)Math.Pow(10, digits) + number;
        }

        return result;
    }
    
    private static State Tick(State columns, int round)
    {
        var source = columns[(round - 1) % 4];
        var clapper = source[0];
        var target = columns[round % 4];

        source.RemoveAt(index: 0);
        
        var moduloMoves = (clapper - 1) % (2 * target.Count) + 1;
        if (moduloMoves <= target.Count) {
            target.Insert(index: moduloMoves - 1, clapper);
        }
        else {
            target.Insert(index: 2 * target.Count + 1 - moduloMoves, clapper);
        }

        return columns;
    }
    
    private static State ParseColumns(int[][] numbers)
    {
        var columns = new State(capacity: 4)
        {
            new List<int>(),
            new List<int>(),
            new List<int>(),
            new List<int>()
        };
            
        for (var i = 0; i < numbers.Length;    i++)
        for (var j = 0; j < numbers[i].Length; j++)
        {
            columns[j].Add(numbers[i][j]);
        }

        return columns;
    }
}
