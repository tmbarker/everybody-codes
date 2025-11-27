global using Solutions.Attributes;
global using Solutions.Common;

using JetBrains.Annotations;
using System.Diagnostics;
using Utilities.Extensions;
using Utilities.Geometry.Euclidean;

namespace Solutions.Common;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.Default)]
public abstract class SolutionBase
{
    protected const string NotSolvedString = "Not solved";
    
    public string? InputDirectory { get; set; }
    public virtual int Parts => 3;
    
    protected virtual string P1InputName => "p1.txt";
    protected virtual string P2InputName => "p2.txt";
    protected virtual string P3InputName => "p3.txt";
    
    public abstract object Solve(int part);
    
    protected string GetInputText(int part, bool trimEnd = true)
    {
        Debug.Assert(InputDirectory != null);
        Debug.Assert(Path.Exists(InputDirectory));
        
        var fileName = GetInputFileName(part);
        var filePath = Path.Combine(InputDirectory, fileName);
        var text = File.ReadAllText(filePath);

        return trimEnd
            ? text.TrimEnd()
            : text;
    }

    protected T ParseInputText<T>(int part, Func<string, T> parser, bool trimEnd = true)
    {
        return parser(GetInputText(part, trimEnd));
    }
    
    protected string[] GetInputLines(int part)
    {
        Debug.Assert(InputDirectory != null);
        Debug.Assert(Path.Exists(InputDirectory));
        
        var fileName = GetInputFileName(part);
        var filePath = Path.Combine(InputDirectory, fileName);
        return File.ReadAllLines(filePath);
    }

    protected T[] ParseInputLines<T>(int part, Func<string, T> parser)
    {
        return GetInputLines(part).Select(parser).ToArray();
    }
    
    protected List<string[]> ChunkInputByNonEmpty(int part)
    {
        return GetInputLines(part).ChunkByNonEmpty();
    }
    
    protected Grid2D<char> GetInputGrid(int part, Origin origin = Origin.Xy)
    {
        var lines = GetInputLines(part);
        return lines.ToGrid(origin);
    }
    
    private string GetInputFileName(int part)
    {
        return part switch
        {
            1 => P1InputName,
            2 => P2InputName,
            3 => P3InputName,
            _ => string.Empty
        };
    }
}