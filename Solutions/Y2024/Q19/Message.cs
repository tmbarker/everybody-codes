using System.Diagnostics;
using System.Text;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q19;

public sealed class Message
{
    private const char RotateLeft  = 'L';
    private const char RotateRight = 'R';
    private const char Start = '>';
    private const char End   = '<';
    
    private readonly List<char> _key;
    private Grid2D<char> _map;
    
    private Message(List<char> key, Grid2D<char> map)
    {
        _key = key;
        _map = map;
    }

    public string Decode(long rounds)
    {
        var lookup = CreateLookupGrid();
        var exponent = 1L;
        
        while (exponent <= rounds)
        {
            if ((exponent & rounds) != 0)
            {
                _map = ApplyTransformation(_map, lookup);
            }
            
            if (exponent < rounds)
            {
                lookup = ApplyTransformation(lookup, lookup);
            }
            
            exponent <<= 1;
        }
        
        return Read();
    }

    private Grid2D<Vec2D> CreateLookupGrid()
    {
        var lookup = Grid2D<Vec2D>.WithDimensions(_map.Height, _map.Width);
        for (var y = 0; y < _map.Height; y++)
        for (var x = 0; x < _map.Width; x++)
        {
            lookup[new Vec2D(x, y)] = new Vec2D(x, y);
        }
        
        var k = 0;
        for (var y = _map.Height - 2; y > 0; y--)
        for (var x = 1; x < _map.Width - 1; x++)
        {
            var pivot = new Vec2D(x, y);
            var command = _key[k++ % _key.Count];

            switch (command)
            {
                case RotateLeft:
                    Transform.CcwRotate(lookup, pivot);
                    break;
                case RotateRight:
                    Transform.CwRotate(lookup, pivot);
                    break;
                default:
                    Debug.Fail($"Invalid command '{command}' in key.");
                    break;
            }
        }
        
        return lookup;
    }
    
    private static Grid2D<T> ApplyTransformation<T>(Grid2D<T> source, Grid2D<Vec2D> lookup)
    {
        var result = Grid2D<T>.WithDimensions(source.Height, source.Width);
        for (var y = 0; y < source.Height; y++)
        for (var x = 0; x < source.Width; x++)
        {
            var pos = new Vec2D(x, y);
            result[pos] = source[lookup[pos]];
        }
        
        return result;
    }
    
    private string Read()
    {
        var sb = new StringBuilder();
        var pos = _map.Find(Start) + Vec2D.Right;
        var end = _map.Find(End);
        Debug.Assert(pos.Y == end.Y);
        
        while (pos != end)
        {
            sb.Append(_map[pos]);
            pos += Vec2D.Right;
        }

        return sb.ToString();
    }
    
    public static Message Parse(string[] input)
    {
        var keyData = input[0];
        var mapData = input[2..];
        return new Message(
            key: keyData.ToList(),
            map: Grid2D<char>.MapChars(mapData));
    }
}