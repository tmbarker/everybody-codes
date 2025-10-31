using Utilities.Extensions;

namespace Solutions.S02.Q03;

public class Dice
{
    private readonly long[] _faces;
    private readonly long _seed;
    private long _pulse;
    private long _rollNumber;
    private long _prevResult;
    
    public long Id { get; }

    private Dice(long id, long[] faces, long seed)
    {
        Id = id;
        _faces = faces;
        _seed = seed;
        _pulse = seed;
        _rollNumber = 1;
        _prevResult = 0;
    }

    public long Roll()
    {
        var spin = _rollNumber * _pulse;
        _prevResult = (_prevResult + spin) % _faces.Length;
        _pulse += spin;
        _pulse %= _seed;
        _pulse += 1 + _rollNumber + _seed;
        _rollNumber++;
        return _faces[_prevResult];
    }
    
    public static Dice Parse(string line)
    {
        var numbers = line.ParseLongs();
        return new Dice(
            id:    numbers[0],
            faces: numbers[1..^1],
            seed:  numbers[^1]);
    }
}