using System.Text.RegularExpressions;

namespace Solutions.S01.Q02;

[QuestInfo("Tangled Trees", Topics.Graphs, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    private static readonly Regex Add  = new(@"^ADD id=(?<ID>\d+) left=\[(?<LR>\d+),(?<LS>.)\] right=\[(?<RR>\d+),(?<RS>.)\]$");
    private static readonly Regex Swap = new(@"^SWAP (?<ID>\d+)$");
    
    public override object Solve(int part)
    {
        var forest = BuildForest(
            instructions: GetInputLines(part),
            swapMode: part is 1 or 2 ? Forest.SwapMode.Value : Forest.SwapMode.Node);
        return forest.Read();
    }

    private static Forest BuildForest(string[] instructions, Forest.SwapMode swapMode)
    {
        var forest = new Forest();
        foreach (var line in instructions)
        {
            var add = Add.Match(line);
            if (add.Success)
            {
                forest.AddPair(
                    id: int.Parse(add.Groups["ID"].Value),
                    lr: int.Parse(add.Groups["LR"].Value),
                    rr: int.Parse(add.Groups["RR"].Value),
                    ls: add.Groups["LS"].Value,
                    rs: add.Groups["RS"].Value);
                continue;
            }
            
            var swap = Swap.Match(line);
            if (swap.Success)
            {
                forest.SwapPair(
                    id: int.Parse(swap.Groups["ID"].Value),
                    mode: swapMode);
            }
        }
        
        return forest;
    }
}
