using System.Runtime.CompilerServices;

namespace Solutions.Y2025.Q02;

[QuestInfo("From Complex to Clarity", Topics.Vectors, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputText(part);
        var pos = Complex.Parse(input);
        
        return part switch
        {
            1 => Validate(a: pos),
            2 => CountGrid(origin: pos, size: 101),
            3 => CountGrid(origin: pos, size: 1001),
            _ => NotSolvedString
        };
    }

    private static string Validate(Complex a)
    {
        var divisor = new Complex(10, 10);
        var result = Complex.Zero;

        for (var i = 0; i < 3; i++)
        {
            result = Cycle(seed: result, add: a, div: divisor);
        }

        return result.ToString();
    }

    private static int CountGrid(Complex origin, int size)
    {
        const long sze = 1000L;
        const long abs = 1000000L;
        
        var density = sze / (size - 1);
        var positions = new List<Complex>(size * size);
        var div = new Complex(X: 100000L, Y: 100000L);
        
        for (var ix = 0; ix < size; ix++)
        for (var iy = 0; iy < size; iy++)
        {
            positions.Add(new Complex(
                X: origin.X + density * ix, 
                Y: origin.Y + density * iy));
        }
        
        return positions
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Count(pos => CheckPos(pos, div, abs));
    }
    
    private static bool CheckPos(Complex pos, Complex div, long abs)
    {
        var result = Complex.Zero;
        for (var c = 0; c < 100; c++)
        {
            result = Cycle(seed: result, add: pos, div: div);
            if (Math.Abs(result.X) > abs || Math.Abs(result.Y) > abs)
            {
                return false;
            }
        }

        return true;  
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Complex Cycle(Complex seed, Complex add, Complex div)
    {
        seed *= seed;
        seed /= div;
        seed += add;
        return seed;
    }
}
