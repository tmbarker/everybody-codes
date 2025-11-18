using Utilities.Extensions;

namespace Solutions.Y2024.Q02;

[QuestInfo("The Runes of Power", Topics.StringParsing, Difficulty.Medium)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var input = GetInputLines(part);
        var words = input[0].Split(':')[^1].Split(',');
        var lines = input[2..];
        
        return part switch
        {
            1 => CountHelmet(words, line: lines.Single()),
            2 => CountShield(words, rows: lines),
            3 => CountArmour(words, rows: lines),
            _ => NotSolvedString
        };
    }
    
    private static int CountHelmet(string[] words, string line)
    {
        return line
            .Select((_, s) => words.Count(word => Check(target: line, word, start: s, wrap: false)))
            .Sum();
    }
    
    private static int CountShield(string[] words, string[] rows)
    {
        var symbols = new HashSet<(int Row, int Col)>();
        var runes = words
            .Concat(words.Select(s => s.Reversed()))
            .ToArray();

        foreach (var word in runes)
        {
            for (var r = 0; r < rows.Length; r++) 
            for (var c = 0; c < rows[r].Length; c++)
            {
                if (Check(target: rows[r], word, start: c, wrap: false))
                {
                    var row = r;
                    symbols.UnionWith(other: Enumerable
                        .Range(start: c, count: word.Length)
                        .Select(col => (row, col)));
                }
            }     
        }
    
        return symbols.Count;
    }
    
    private static int CountArmour(string[] words, string[] rows)
    {
        var symbols = new HashSet<(int Row, int Col)>();
        var runes = words
            .Concat(words.Select(w => w.Reversed()))
            .ToArray();
        
        var cols = rows
            .First()
            .Select((_, col) => GetCol(rows, col))
            .ToArray(); 
        
        foreach (var word in runes)
        {
            for (var r = 0; r < rows.Length; r++) 
            for (var c = 0; c < rows[r].Length; c++)
            {
                var horizontal = rows[r];
                var vertical = cols[c];
                
                if (Check(target: horizontal, word, start: c, wrap: true))
                {
                    var row = r;
                    symbols.UnionWith(other: Enumerable
                        .Range(start: c, count: word.Length)
                        .Select(col => col % rows[row].Length)
                        .Select(col => (row, col)));
                }

                if (Check(target: vertical, word, start: r, wrap: false))
                {
                    var col = c;
                    symbols.UnionWith(other: Enumerable
                        .Range(start: r, count: word.Length)
                        .Select(row => (row, col)));
                }
            }     
        }
    
        return symbols.Count;
    }

    private static string GetCol(string[] lines, int col)
    {
        return new string(lines
            .Select(line => line[col])
            .ToArray());
    }
    
    private static bool Check(string target, string word, int start, bool wrap)
    {
        for (var i = 0; i < word.Length; i++)
        {
            var j = wrap
                ? (start + i) % target.Length
                : start + i;

            if (j >= target.Length)
            {
                return false;
            }
            
            if (target[j] != word[i])
            {
                return false;
            }
        }
        
        return true;
    }
}
