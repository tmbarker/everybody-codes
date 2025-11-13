using Utilities.Extensions;
using Utilities.Numerics;

namespace Solutions.Y2025.Q08;

[QuestInfo("The Art of Connection", Topics.Math, Difficulty.Hard)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var sequence = ParseInputText(part, t => t.ParseInts());
        return part switch
        {
            1 => CountBisections(sequence, nails: 32),
            2 => CountKnots(sequence, nails: 256),
            3 => GetBestCut(sequence, nails: 256),
            _ => NotSolvedString
        };
    }

    private static int CountBisections(int[] sequence, int nails)
    {
        return sequence
            .Windows(size: 2)
            .Count(chord => Numerics.ModAdd(a: chord[0] - 1, b: nails / 2, m: nails) == chord[1] - 1);
    }

    private static int CountKnots(int[] sequence, int nails)
    {
        var links = new List<int>[nails + 1];
        var freq = new int[nails + 2];
        var result = 0;

        for (var i = 0; i < nails + 1; i++)
        {
            links[i] = [];
        }

        foreach (var chord in sequence.Windows(size: 2))
        {
            var start = Math.Min(chord[0], chord[1]);
            var end = Math.Max(chord[0], chord[1]);
            links[start].Add(end);
        }

        for (var start = 0; start < links.Length; start++)
        {
            foreach (var end in links[start])
            {
                for (var i = start + 1; i < end; i++)
                {
                    result += freq[i];
                }
            }

            foreach (var end in links[start])
            {
                freq[end] += 1;
            }
        }

        return result;
    }

    private static int GetBestCut(int[] sequence, int nails)
    {
        var links = new List<int>[nails + 1];
        var delta = new int[nails + 2];
        var result = 0;

        for (var i = 0; i < nails + 1; i++)
        {
            links[i] = [];
        }

        foreach (var chord in sequence.Windows(size: 2))
        {
            var start = Math.Min(chord[0], chord[1]);
            var end = Math.Max(chord[0], chord[1]);
            links[start].Add(end);
            delta[start + 1] += 1;
            delta[end] -= 1;
        }

        for (var start = 1; start < nails - 1; start++)
        {
            foreach (var end in links[start])
            {
                delta[end] += 2;
                delta[end + 1] -= 1;
            }

            foreach (var end in links[start - 1])
            {
                delta[end] -= 1;
                delta[end + 1] += 2;
            }

            var cuts = 0;
            for (var i = start + 2; i < nails + 1; i++)
            {
                cuts += delta[i];
                result = Math.Max(result, cuts);
            }
        }

        return result;
    }
}
