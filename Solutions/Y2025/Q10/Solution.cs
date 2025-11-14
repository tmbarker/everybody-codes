using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.Y2025.Q10;

[QuestInfo("Feast on the Board", Topics.Simulation|Topics.Recursion, Difficulty.Hard, favourite: true)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        Board.Grid = GetInputGrid(part);
        return part switch
        {
            1 => Part1(moves: 4),
            2 => Part2(rounds: 20),
            3 => Part3(),
            _ => NotSolvedString
        };
    }

    private static int Part1(int moves)
    {
        HashSet<Vec2D> heads = [Board.GetDragon()];
        for (var i = 0; i < moves; i++)
        {
            var next = heads.SelectMany(Board.GetDragonMoves).ToHashSet();
            heads.AddRange(next);
        }
        return heads.Count(Board.IsSheep);
    }
    
    private static int Part2(int rounds)
    {
        var heads = new HashSet<Vec2D>([Board.GetDragon()]);
        var sheep = Board.GetSheep();
        var count = 0;
        
        for (var i = 0; i < rounds; i++)
        {
            heads = heads.SelectMany(Board.GetDragonMoves).ToHashSet();
            EatSheep();
            
            sheep = sheep.Select(pos => pos + Vec2D.Down).Where(Board.Grid.Contains).ToHashSet();
            EatSheep();
        }
        
        return count;
        void EatSheep() => count += sheep.RemoveWhere(pos => heads.Contains(pos) && !Board.IsHideout(pos));
    }
    
    private static long Part3()
    {
        return CountSequences(state: State.Initial(), memo: new Dictionary<State, long>());
    }

    private static long CountSequences(State state, Dictionary<State, long> memo)
    {
        if (memo.TryGetValue(state, out var cached))
        {
            return cached;
        }
        
        if (state.SheepCount == 0)
        {
            memo[state] = 1L;
            return 1L;
        }
        
        var counts = 0L;
        var adjacentStates = state.IsSheepTurn 
            ? state.GetSheepMoves() 
            : state.GetDragonMoves();
        
        foreach (var adjacentState in adjacentStates)
        {
            counts += CountSequences(adjacentState, memo);
        }
        
        memo[state] = counts;
        return counts;
    }
}
