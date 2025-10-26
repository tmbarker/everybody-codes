namespace Solutions.Y2024.Q20;

public static class Terrain
{
    private const char Void = '.';
    private const char Cool = '-';
    private const char Warm = '+';
    
    private static readonly Dictionary<char, int> AltitudeMap = new()
    {
        [Warm] =  1,
        [Void] = -1,
        [Cool] = -2
    };

    public static int GetMaxGain() => AltitudeMap.Values.Max();
    public static bool IsWarm(char c) => c is Warm;
    public static bool IsFlyable(char terrain, out int da)
    {
        if (Poi.CheckpointsMap.ContainsKey(terrain))
        {
            da = AltitudeMap[Void];
            return true;
        }
        
        return AltitudeMap.TryGetValue(terrain, out da);
    }
}