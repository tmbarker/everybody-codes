using Utilities.Geometry;

namespace Solutions.Y2024.Q12;

public static class Projectile
{
    public readonly record struct Result
    {
        private static readonly Result Failure = new(success: false, power: 0);
        
        public bool Success { get; }
        public int Power { get; }

        private Result(bool success, int power)
        {
            Success = success;
            Power = power;
        }

        public static Result Fail() => Failure;
        public static Result Succeed(int power) => new(success: true, power);
    }

    public static Result Query(Vec2D src, Vec2D tgt)
    {
        var dx = tgt.X - src.X;
        var dy = tgt.Y - src.Y;
        
        if (dx < dy)            return Result.Fail();                 // Cannot be hit
        if (dx <= 2 * dy)       return Result.Succeed(dy);            // Upwards OR Horizontal phase
        if ((dx + dy) % 3 == 0) return Result.Succeed((dx + dy) / 3); // Downwards phase

        return Result.Fail();
    }
}