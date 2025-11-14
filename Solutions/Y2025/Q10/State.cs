using System.Numerics;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q10;

public readonly record struct State(Vec2D Dragon, ulong SheepMask, bool IsSheepTurn)
{
    public static State Initial()
    {
        var sheep = Board.GetSheep();
        var mask = sheep.Aggregate(
            seed: 0UL,
            func: (current, pos) => current | 1UL << PosToBitIndex(pos));

        return new State(
            Dragon: Board.GetDragon(),
            SheepMask: mask,
            IsSheepTurn: true);
    }
    
    public int SheepCount => BitOperations.PopCount(SheepMask);
    
    public IEnumerable<State> GetDragonMoves()
    {
        foreach (var move in Board.GetDragonMoves(Dragon))
        {
            var newMask = SheepMask;
            if (!Board.IsHideout(move))
            {
                newMask &= ~(1UL << PosToBitIndex(move));
            }

            yield return new State(move, newMask, IsSheepTurn: true);
        }
    }

    public IEnumerable<State> GetSheepMoves()
    {
        var sheepMoved = 0;
        var sheepRemaining = SheepMask;
        
        while (sheepRemaining != 0)
        {
            var bitIndex = BitOperations.TrailingZeroCount(sheepRemaining);
            var from = BitIndexToPos(bitIndex);
            var to = from + Vec2D.Down;
            sheepRemaining &= ~(1UL << bitIndex);
            
            if (!Board.Grid.Contains(to) || to != Dragon || Board.IsHideout(to))
            {
                sheepMoved++;
                if (!Board.Grid.Contains(to) || Board.IsEscape(to))
                {
                    continue;
                }
                
                var newMask = SheepMask;
                newMask &= ~(1UL << bitIndex);
                newMask |= 1UL << PosToBitIndex(to);
                
                yield return new State(Dragon, newMask, IsSheepTurn: false);
            }
        }
        
        if (sheepMoved == 0)
        {
            yield return this with { IsSheepTurn = false };
        }
    }
    
    private static int PosToBitIndex(Vec2D pos) => pos.Y * Board.Grid.Width + pos.X;
    private static Vec2D BitIndexToPos(int index) => new(index % Board.Grid.Width, index / Board.Grid.Width);
}