using Utilities.Extensions;

namespace Solutions.Y2025.Q02;

public readonly record struct Complex(long X, long Y)
{
    public static readonly Complex Zero = new(0L, 0L);
        
    public static Complex operator +(Complex a, Complex b)
    {
        return new Complex(
            X: a.X + b.X, 
            Y: a.Y + b.Y);
    }
        
    public static Complex operator *(Complex a, Complex b)
    {
        return new Complex(
            X: a.X * b.X - a.Y * b.Y,
            Y: a.X * b.Y + a.Y * b.X);
    }
        
    public static Complex operator /(Complex a, Complex b)
    {
        return new Complex(
            X: a.X / b.X,
            Y: a.Y / b.Y);
    }

    public static Complex Parse(string line)
    {
        var nums = line.ParseLongs();
        return new Complex(nums[0], nums[1]);
    }
    
    public override string ToString()
    {
        return $"[{X},{Y}]";
    }
}