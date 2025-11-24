using Utilities.Geometry.Euclidean;

namespace Solutions.S02.Q01;

public class Machine
{
    private const char Nail = '*';
    private const char Void = '.';
    private const char Left = 'L';
    
    private readonly Grid2D<char> _grid;
    private readonly Dictionary<int, Vec2D> _topSlotMap;
    private readonly Dictionary<Vec2D, int> _botSlotMap;

    public IReadOnlyCollection<int> SlotIndices => _topSlotMap.Keys;
    
    private Machine(Grid2D<char> grid)
    {
        var slotIndices = Enumerable
            .Range(start: 0, count: grid.Width)
            .Where(i => i % 2 == 0)
            .ToArray();
        
        _grid = grid;
        _topSlotMap = slotIndices.ToDictionary(
            keySelector:     i => i / 2, 
            elementSelector: i => new Vec2D(X: i, Y: grid.Height));
        _botSlotMap = slotIndices.ToDictionary(
            keySelector:     i => new Vec2D(X: i, Y: 0),
            elementSelector: i => i / 2);
    }
    
    public static Machine Parse(string[] lines)
    {
        return new Machine(grid: lines.ToGrid());
    }
    
    public int Drop(int tossSlotIndex, string tokenBehaviour)
    {
        int result;
        var pos = _topSlotMap[tossSlotIndex];
        var bounce = 0;

        while (!_botSlotMap.TryGetValue(pos, out result))
        {
            var below = pos + Vec2D.Down;
            switch (_grid[below])
            {
                case Void:
                    pos = below;
                    continue;
                case Nail:
                    var behaviour = tokenBehaviour[bounce++ % tokenBehaviour.Length];
                    var bounceDir = behaviour == Left ? Vec2D.Left : Vec2D.Right;
                    var targetPos = below + bounceDir;
                    if (!_grid.Contains(targetPos) || _grid[targetPos] is Nail)
                    {
                        targetPos = below - bounceDir;
                    }
                    pos = targetPos;
                    continue;
                default:
                    throw new NoSolutionException($"Invalid machine cell '{_grid[below]}' at {below}.");
            }
        }

        return Math.Max(0, 2 * (result + 1) - (tossSlotIndex + 1));
    }
}