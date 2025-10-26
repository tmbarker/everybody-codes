namespace Solutions.Y2024.Q20;

public static class Poi
{
    [Flags]
    public enum Checkpoints
    {
        None   = 0,
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
        S = 1 << 3,
        All = A | B | C | S
    }
    
    public const char Start = 'S';
    public static readonly Dictionary<char, Checkpoints> CheckpointsMap = new()
    {
        ['A'] = Checkpoints.A,
        ['B'] = Checkpoints.B,
        ['C'] = Checkpoints.C,
        ['S'] = Checkpoints.S
    };
}