using Utilities.Geometry.Euclidean;

namespace Solutions.Y2024.Q19;

public static class Transform
{
    //  NOTE: Transformation maps are structured as follows:
    //    key: vector from the 'pivot' point to the source point
    //    value: vector from the 'pivot' point to the destination point
    //
    private static readonly Dictionary<Vec2D, Vec2D> CwMap = new()
    {
        [new Vec2D(-1,  1)] = new Vec2D( 0,  1),
        [new Vec2D( 0,  1)] = new Vec2D( 1,  1),
        [new Vec2D( 1,  1)] = new Vec2D( 1,  0),
        [new Vec2D( 1,  0)] = new Vec2D( 1, -1),
        [new Vec2D( 1, -1)] = new Vec2D( 0, -1),
        [new Vec2D( 0, -1)] = new Vec2D(-1, -1),
        [new Vec2D(-1, -1)] = new Vec2D(-1,  0),
        [new Vec2D(-1,  0)] = new Vec2D(-1,  1)
    };
    private static readonly Dictionary<Vec2D, Vec2D> CcwMap = new()
    {
        [new Vec2D(-1,  1)] = new Vec2D(-1,  0),
        [new Vec2D( 0,  1)] = new Vec2D(-1,  1),
        [new Vec2D( 1,  1)] = new Vec2D( 0,  1),
        [new Vec2D( 1,  0)] = new Vec2D( 1,  1),
        [new Vec2D( 1, -1)] = new Vec2D( 1,  0),
        [new Vec2D( 0, -1)] = new Vec2D( 1, -1),
        [new Vec2D(-1, -1)] = new Vec2D( 0, -1),
        [new Vec2D(-1,  0)] = new Vec2D(-1, -1)
    };
    
    public static void CwRotate(Grid2D<Vec2D> map, Vec2D pivot) => Apply(map, pivot, CwMap);
    public static void CcwRotate(Grid2D<Vec2D> map, Vec2D pivot) => Apply(map, pivot, CcwMap);

    private static void Apply(Grid2D<Vec2D> map, Vec2D pivot, Dictionary<Vec2D, Vec2D> transform)
    {
        var writes = transform.ToDictionary(
            keySelector: kvp => pivot + kvp.Value,
            elementSelector: kvp => map[pivot + kvp.Key]);

        foreach (var (writePos, writeVal) in writes)
        {
            map[writePos] = writeVal;
        }
    }
}