using System.Diagnostics.CodeAnalysis;

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
    
    /// <summary>
    ///     Chunk the source between null or whitespace strings.
    /// </summary>
    /// <param name="source">The source collection</param>
    /// <returns>An iterator which yields chunk arrays</returns>
    public static IList<string[]> ChunkByNonEmpty(this IEnumerable<string> source)
    {
        return source.ChunkBy(takePredicate: s => !string.IsNullOrWhiteSpace(s));
    }
    
    /// <summary>
    ///     Chunk the source based on a <paramref name="takePredicate" /> and an
    ///     optional <paramref name="skipPredicate" />.
    /// </summary>
    /// <param name="source">The source collection</param>
    /// <param name="takePredicate">Select which elements should be included in a chunk</param>
    /// <param name="skipPredicate">
    ///     Select which elements to skip between chunks, if not provided
    ///     then <paramref name="takePredicate" /> is used and negated
    /// </param>
    /// <typeparam name="T">The type associated with each element in the source</typeparam>
    /// <returns>An iterator which yields chunk arrays</returns>
    public static IList<T[]> ChunkBy<T>(this IEnumerable<T> source, Predicate<T> takePredicate,
        Predicate<T>? skipPredicate = null)
    {
        var chunks = new List<T[]>();
        var enumerable = source as IList<T> ?? source.ToArray();

        for (var i = 0; i < enumerable.Count;)
        {
            var chunk = enumerable
                .Skip(i)
                .TakeWhile(takePredicate.Invoke)
                .ToArray();

            if (chunk.Length > 0)
            {
                chunks.Add(chunk);
            }

            i += chunk.Length;
            i += enumerable
                .Skip(i)
                .TakeWhile(element => skipPredicate?.Invoke(element) ?? !takePredicate.Invoke(element))
                .Count();
        }

        return chunks;
    }

    /// <summary>
    ///     Determines whether the sequence contains exactly one element that matches the specified predicate.
    /// </summary>
    /// <param name="source">The source collection</param>
    /// <param name="predicate">The predicate to invoke on each source element</param>
    /// <param name="element">When successful, the single element which passes the predicate</param>
    /// <returns>A <see cref="bool"/> representing the success of the query</returns>
    public static bool HasExactlyOne<T>(this IEnumerable<T> source, Func<T, bool> predicate,
        [NotNullWhen(returnValue: true)] out T? element)
    {
        var candidates = source.Where(predicate).Take(2).ToArray();
        if (candidates.Length == 1)
        {
            element = candidates.Single()!;
            return true;
        }
        
        element = default;
        return false;
    }
    
    /// <summary>
    ///     Filters a sequence of items based on a corresponding sequence of boolean selectors.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <paramref name="items" /> sequence</typeparam>
    /// <param name="items">The sequence of items to filter</param>
    /// <param name="selectors">
    ///     A sequence of boolean values that determines whether each corresponding element in <paramref name="items" />
    ///     is included in the result. An element is included if the corresponding selector is <c>true</c>
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> containing elements from <paramref name="items" /> where the corresponding
    ///     value in <paramref name="selectors" /> is <c>true</c>
    /// </returns>
    public static IEnumerable<T> ZipWhere<T>(this IEnumerable<T> items, IEnumerable<bool> selectors)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(selectors);
        
        return ZipWhereIterator(items, selectors);
    }
    
    private static IEnumerable<T> ZipWhereIterator<T>(IEnumerable<T> items, IEnumerable<bool> selectors)
    {
        using var itemEnumerator = items.GetEnumerator();
        using var selectorEnumerator = selectors.GetEnumerator();

        while (itemEnumerator.MoveNext() && selectorEnumerator.MoveNext())
        {
            if (selectorEnumerator.Current)
            {
                yield return itemEnumerator.Current;
            }
        }
    }
}