namespace Utilities.Extensions;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            set.Add(item);
        }
    }
}