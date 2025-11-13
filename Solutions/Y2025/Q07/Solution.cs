using Utilities.Extensions;

namespace Solutions.Y2025.Q07;

[QuestInfo("Namegraph", Topics.Recursion, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var names = input[0].Split(',');
        var rules = input[2..]
            .ToDictionary(
                keySelector:     rule => rule[0],
                elementSelector: rule => rule[4..].Where(char.IsLetter).ToHashSet());

        return part switch
        {
            1 => names
                .Single(name => IsValid(name, rules)),
            2 => names
                .Select((name, i) => IsValid(name, rules) ? i + 1 : 0)
                .Sum(),
            3 => names
                .Where(name => IsValid(name, rules))
                .Where(name => !names.Any(other => other != name && name.StartsWith(other)))
                .Sum(name => CountValid(name[^1], name.Length, minLength: 7, maxLength: 11, rules)),
            _ => NotSolvedString
        };
    }
    
    private static int CountValid(char curChar, int curLength, int minLength, int maxLength, Dictionary<char, HashSet<char>> rules)
    {
        var count = curLength >= minLength ? 1 : 0;
        if (curLength < maxLength && rules.TryGetValue(curChar, out var nextChars))
        {
            foreach (var nextChar in nextChars)
            {
                count += CountValid(nextChar, curLength + 1, minLength, maxLength, rules);
            }
        }
        return count;
    }
    
    private static bool IsValid(string name, Dictionary<char, HashSet<char>> rules)
    {
        return name
            .Windows(size: 2)
            .All(pair => rules.TryGetValue(pair[0], out var allowed) && allowed.Contains(pair[1]));
    }
}
