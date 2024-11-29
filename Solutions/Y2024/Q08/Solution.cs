using Utilities.Collections;

namespace Solutions.Y2024.Q08;

[QuestInfo("A Shrine for Nullpointer", Topics.Math, Difficulty.Medium)]
public class Solution : SolutionBase
{
    private long Input(int part) => ParseInputText(part, long.Parse);
    
    public override object Solve(int part)
    {
        return part switch
        {
            1 => ComputeFilled(blocks: Input(part), priest: 1L,          acolyte: long.MaxValue),
            2 => ComputeFilled(blocks: 20240000L,   priest: Input(part), acolyte: 1111L),
            3 => ComputeHollow(blocks: 202400000L,  priest: Input(part), acolyte: 10L),
            _ => NotSolvedString
        };
    }
    
    private static long ComputeFilled(long blocks, long priest, long acolyte)
    {
        var tMemo = new Dictionary<long, long> { { 1, 1 } };
        var total = 0L;
        var width = 0L;
        
        for (var l = 1L; total < blocks; l++)
        {
            width = 2 * l - 1L;
            total += width * GetFilledThickness(l, p: priest, a: acolyte, tMemo);
        }

        return (total - blocks) * width;
    }

    private static long ComputeHollow(long blocks, long priest, long acolyte)
    {
        var tMemo = new Dictionary<long, long> { { 1, 1 } };
        var hMemo = new DefaultDict<long, long>(defaultValue: 0L);
        var total = 0L;
        var empty = 0L;
        
        for (var l = 1L; blocks > total - empty; l++)
        {
            var bWidth = 2L * l - 1L;
            var hWidth = bWidth / 2L;
            
            empty = 0L;
            total += bWidth * GetHollowThickness(l, p: priest, a: acolyte, tMemo);
            
            for (var d = 0L; d <= hWidth; d++)
            {
                hMemo[d] += IsLayerPresentInColumn(layer: l, columnDistanceToCenter: d)
                    ? tMemo[l]
                    : 0;
            }

            for (var d = 0L; d < hWidth; d++)
            {
                var cHeight = hMemo[d];
                var cRemove = priest * bWidth * cHeight % acolyte;
                empty += d != 0L
                    ? 2L * cRemove
                    : cRemove;
            }
        }

        return total - empty - blocks;
    }

    private static bool IsLayerPresentInColumn(long layer, long columnDistanceToCenter)
    {
        var halfWidth = layer - 1;
        return
            columnDistanceToCenter >= -halfWidth &&
            columnDistanceToCenter <= halfWidth;
    }
    
    private static long GetFilledThickness(long l, long p, long a, Dictionary<long, long> memo)
    {
        if (memo.TryGetValue(l, out var thickness))
        {
            return thickness;
        }

        thickness = p * GetFilledThickness(l: l - 1, p, a, memo) % a;
        memo[l] = thickness;
        return thickness;
    }
    
    private static long GetHollowThickness(long l, long p, long a, Dictionary<long, long> memo)
    {
        if (memo.TryGetValue(l, out var thickness))
        {
            return thickness;
        }

        thickness = a + p * GetHollowThickness(l: l - 1, p, a, memo) % a;
        memo[l] = thickness;
        return thickness;
    }
}
