using System.Collections;
using Spectre.Console;

namespace AoC;

public static class GeneralExtensions
{
    /// <summary>
    /// Returns the indexes in specified Range, from the inclusive start index of the Range to the exclusive end index of the Range.
    /// </summary>
    public static int[] ToArray(this Range range) => ToEnumerable(range).ToArray();

    /// <summary>
    /// Enumerates the indexes in specified Range, from the inclusive start index of the Range to the exclusive end index of the Range.
    /// </summary>
    public static IEnumerable<int> ToEnumerable(this Range range)
    {
        for (var i = range.Start.Value; i < range.End.Value; i++)
        {
            yield return i;
        }
    }

    /// <summary>
    /// Converts the specified enumerable/collection to a read only array.
    /// </summary>
    public static IReadOnlyList<TSource> ToReadOnlyArray<TSource>(this IEnumerable<TSource> source) => Array.AsReadOnly(source.ToArray());

    /// <summary>
    /// Parses and returns each line in the input string.
    /// </summary>
    public static IEnumerable<string> ReadLines(this string input)
    {
        using var sr = new StringReader(input);
        while (sr.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    /// <summary>
    /// Parses and returns each line in the input string as an Int64.
    /// </summary>
    public static IEnumerable<long> ReadLinesAsLongs(this string input) => input.ReadLines().Select(long.Parse);

    /// <summary>
    /// Increments the value in the dictionary matching the key by the specified amount, or adds the amount in to the dictionary if no matching key is found.
    /// </summary>
    public static void AddOrIncrement<TKey>(this Dictionary<TKey, long> dictionary, TKey key, long amount = 1) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = 0;
        }

        dictionary[key] += amount;
    }

    /// <summary>
    /// Adds a key/value pair to the dictionary by using the specified function if the key does not already exist, or returns the existing value if the key exists.
    /// </summary>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> buildValue)
        where TKey : notnull => dictionary.TryGetValue(key, out var value) ? value : dictionary[key] = buildValue();

    /// <summary>
    /// Groups the contiguous elements of a sequence according to a specified key selector function.
    /// rs-todo: tests!
    /// </summary>
    public static IEnumerable<IGrouping<TKey, TSource>> ContiguousGroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (!source.Any())
        {
            yield break;
        }

        TKey currentKey = keySelector(source.First());
        List<TSource> items = [source.First()];

        foreach (var item in source.Skip(1))
        {
            TKey nextKey = keySelector(item);

            if (Equals(nextKey, currentKey))
            {
                yield return new ContiguousGroup<TKey, TSource>(currentKey, items);
                items = [];
            }

            currentKey = nextKey;
            items.Add(item);
        }

        yield return new ContiguousGroup<TKey, TSource>(currentKey, items);
    }

    private class ContiguousGroup<TKey, TSource>(TKey key, List<TSource> items) : IGrouping<TKey, TSource>
    {
        public TKey Key => key;

        public IEnumerator<TSource> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
