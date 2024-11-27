namespace Solutions.Attributes;

[Flags]
public enum Topics
{
    None = 0,
    StringParsing = 1 << 0,
    Vectors       = 1 << 1
}