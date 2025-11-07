using Utilities.Extensions;

namespace Solutions.Y2025.Q05;

public class SpineSegment(long center) : IComparable<SpineSegment>
{
    public long Center { get; } = center;
    public long? Left { get; set; }
    public long? Right { get; set; }

    public long Number => long.Parse($"{Left}{Center}{Right}");

    public int CompareTo(SpineSegment? other)
    {
        return Number.CompareTo(other!.Number);
    }
}

public class Fishbone(long id, List<SpineSegment> segments) : IComparable<Fishbone>
{
    public long Id { get; } = id;
    private List<SpineSegment> Segments { get; } = segments;

    public long Quality => long.Parse(string.Concat(Segments.Select(segment => segment.Center)));

    public int CompareTo(Fishbone? other)
    {
        var thisQuality = Quality;
        var otherQuality = other!.Quality;
        if (thisQuality != otherQuality)
        {
            return thisQuality.CompareTo(otherQuality);
        }
        
        var maxSegments = Math.Max(Segments.Count, other.Segments.Count);
        for (var i = 0; i < maxSegments; i++)
        {
            var thisLevel = GetLevelNumber(i);
            var otherLevel = other.GetLevelNumber(i);
            if (thisLevel != otherLevel)
            {
                return thisLevel.CompareTo(otherLevel);
            }
        }
        
        return Id.CompareTo(other.Id);
    }
    
    private long GetLevelNumber(int level)
    {
        return level < Segments.Count
            ? Segments[level].Number
            : 0;
    }

    public static Fishbone Parse(string input)
    {
        var nums = input.ParseInts();
        var segments = new List<SpineSegment>();

        foreach (var num in nums[1..])
        {
            var left = segments.FirstOrDefault(segment => segment.Left is null && num < segment.Center);
            if (left != null)
            {
                left.Left = num;
                continue;
            }
            
            var right = segments.FirstOrDefault(segment => segment.Right is null && num > segment.Center);
            if (right != null)
            {
                right.Right = num;
                continue;
            }

            segments.Add(new SpineSegment(center: num));
        }

        return new Fishbone(id: nums[0], segments);
    }
}
