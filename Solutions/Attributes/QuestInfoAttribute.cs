namespace Solutions.Attributes;

[AttributeUsage(validOn: AttributeTargets.Class)]
public class QuestInfoAttribute(string title, Topics topics, Difficulty difficulty) : Attribute
{
    public string Title { get; } = title;
    public Topics Topics { get; } = topics;
    public Difficulty Difficulty { get; } = difficulty;
}