namespace Solutions.Y2024.Q07;

[QuestInfo("Not Fast but Furious", Topics.Simulation, Difficulty.Medium, favourite:true)]
public class Solution : SolutionBase
{
    public override object Solve(int part)
    {
        var tPlan = Track.GetPlan(part);
        var dPlans = ParseDevicePlans(part);

        return part != 3
            ? Rank(tPlan, dPlans, laps: 10)
            : CountStrategies(tPlan, dPlanRival: dPlans.Values.Single(), laps: 2024);
    }

    private static int Race(string tPlan, string dPlan, int laps)
    {
        var total = 0;
        var value = 10;

        for (var i = 0; i < tPlan.Length * laps; i++)
        {
            var tAct = tPlan[i % tPlan.Length];
            var dAct = dPlan[i % dPlan.Length];

            value = Execute(value, tAct, dAct);
            total += value;
        }

        return total;
    }

    private static int CountStrategies(string tPlan, string dPlanRival, int laps)
    {
        var dLen = dPlanRival.Length;
        var kLap = laps / dLen;
        var pts = kLap * Race(tPlan, dPlanRival, laps: dLen);
        return Strategies
            .Permute()
            .Count(dPlan => kLap * Race(tPlan, dPlan, laps: dLen) > pts);
    }

    private static int Execute(int value, char tAct, char dAct)
    {
        var act = tAct switch
        {
            '=' => dAct,
            'S' => dAct,
            '+' => '+',
            '-' => '-',
            _ => throw new NoSolutionException($"Invalid action ['{tAct}']")
        };

        return Math.Max(0, act switch
        {
            '+' => value + 1,
            '-' => value - 1,
            '=' => value,
            _ => throw new NoSolutionException($"Invalid action ['{act}']")
        });
    }

    private static string Rank(string tPlan, Dictionary<char, string> dPlans, int laps)
    {
        var results = new Dictionary<char, int>();
        foreach (var (id, dPlan) in dPlans) results[id] = Race(tPlan, dPlan, laps);
        return string.Concat(results.Keys.OrderByDescending(id => results[id]));
    }

    private Dictionary<char, string> ParseDevicePlans(int part)
    {
        var input = GetInputLines(part);
        var plans = new Dictionary<char, string>();

        foreach (var line in input)
        {
            var components = line.Split(":");
            var id = components[0].Single();
            var actions = components[^1]
                .Split(",")
                .Select(s => s.Single())
                .ToArray();

            plans[id] = new string(actions);
        }

        return plans;
    }
}