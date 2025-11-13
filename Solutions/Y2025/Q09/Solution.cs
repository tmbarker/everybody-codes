using Utilities.Collections;

namespace Solutions.Y2025.Q09;

[QuestInfo("Encoded in the Scales", Topics.Graphs, Difficulty.Medium)]
public sealed class Solution : SolutionBase
{
    private readonly record struct Offspring(Dna Child, Dna Parent1, Dna Parent2);
    private readonly record struct Dna(int Id, string Scale)
    {
        public static Dna Parse(string line)
        {
            var parts = line.Split(':', count: 2);
            return new Dna(int.Parse(parts[0]), parts[1]);
        }
    }
    
    public override object Solve(int part)
    {
        var dna = ParseInputLines(part, Dna.Parse);
        return part switch
        {
            1 or 2 => ScoreOffspring(dna),
            3 => ScoreFamilySize(dna),
            _ => NotSolvedString
        };
    }
    
    private static int ScoreOffspring(Dna[] dnas)
    {
        var offspring = new List<Offspring>();
        FindAllOffspring(dnas, onOffspringFound: found => offspring.Add(found));
        
        return offspring
            .Select(os => Score(os.Child, [os.Parent1, os.Parent2]))
            .Sum();
    }
    
    private static int ScoreFamilySize(Dna[] dnas)
    {
        var disjointSet = new DisjointSet<int>(dnas.Select(d => d.Id));
        FindAllOffspring(dnas, offspring =>
        {
            disjointSet.Union(offspring.Child.Id, offspring.Parent1.Id);
            disjointSet.Union(offspring.Child.Id, offspring.Parent2.Id);
        });
        
        return dnas
            .GroupBy(dna => disjointSet.FindSet(dna.Id))
            .MaxBy(g => g.Count())!
            .Sum(dna => dna.Id);
    }
    
    private static void FindAllOffspring(Dna[] dnas, Action<Offspring> onOffspringFound)
    {
        var n = dnas.Length;
        
        for (var p1 = 0; p1 < n; p1++)
        for (var p2 = p1 + 1; p2 < n; p2++)
        for (var c = 0; c < n; c++)
        {
            if (c == p1 || c == p2)
            {
                continue;
            }
                
            if (AreParents(dnas[c].Scale, dnas[p1].Scale, dnas[p2].Scale))
            {
                onOffspringFound(new Offspring(dnas[c], dnas[p1], dnas[p2]));
            }
        }
    }
    
    private static bool AreParents(string child, string parent1, string parent2)
    {
        for (var i = 0; i < child.Length; i++)
        {
            if (child[i] != parent1[i] && child[i] != parent2[i])
            {
                return false;
            }
        }
        return true;
    }
    
    private static int Score(Dna child, Dna[] parents)
    {
        return parents
            .Select(parent => child.Scale
                .Where((t, i) => t == parent.Scale[i])
                .Count())
            .Aggregate(seed: 1, (current, contr) => current * contr);
    }
}
