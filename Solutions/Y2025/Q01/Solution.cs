using Utilities.Extensions;

namespace Solutions.Y2025.Q01;

[QuestInfo("Whispers in the Shell", Topics.Simulation, Difficulty.Easy)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var names = input[0].Split(',');
        var steps = input[^1]
            .Split(',')
            .Select(instr => (instr[0] == 'L' ? -1 : 1) * int.Parse(instr[1..]));

        var nameIdx = 0;
        foreach (var step in steps)
        {
            switch (part)
            {
                case 1:
                    nameIdx = Math.Clamp(nameIdx + step, 0, names.Length - 1);
                    break;
                case 2:
                    nameIdx = (nameIdx + step).Modulo(names.Length);
                    break;
                case 3:
                    var target = step.Modulo(names.Length);
                    (names[0], names[target]) = (names[target], names[0]);
                    break;
            }
        }

        return names[nameIdx];
    }
}
