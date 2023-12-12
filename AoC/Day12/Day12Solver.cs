namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hot Springs";

    public long? SolvePart1(string input)
    {
        var rows = input.ReadLines()
            .Select(RowState.Parse)
            .ToArray();

        return SumCountOfPossibleArrangements(rows);
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    static long SumCountOfPossibleArrangements(RowState[] rows)
    {
        ArrangementsCountsCache.Clear();
        return rows.Sum(row => row.CountPossibleArrangements());
    }

    static readonly Dictionary<string, long> ArrangementsCountsCache = [];

    public record RowState(string Springs, int[] ExpectedCounts, string Path, int ContiguousDamagedSpringsCount)
    {
        public static RowState Parse(string line)
        {
            var split = line.Split(' ');
            return new(split[0] + '.', split[1].Split(',').Select(int.Parse).ToArray(), "", 0);
        }

        public long CountPossibleArrangements()
        {
            if (Springs.Length == 0)
            {
                // We've reached the end of this arrangement, so count it if we've consumed all of the group counts
                return ExpectedCounts.Length == 0 ? 1 : 0;
            }

            var cacheKey = $"{Springs}_{string.Join(',', ExpectedCounts)}_{ContiguousDamagedSpringsCount}";

            if (ArrangementsCountsCache.TryGetValue(cacheKey, out var cachedCount))
            {
                return cachedCount;
            }

            var count = Springs[0] switch
            {
                '?' =>
                    // at this point, the arrangement splits in to 2 possibilities, either an undamaged spring '.', or a damaged spring '#'
                    new RowState('.' + Springs[1..], ExpectedCounts, Path, ContiguousDamagedSpringsCount).CountPossibleArrangements() +
                    new RowState('#' + Springs[1..], ExpectedCounts, Path, ContiguousDamagedSpringsCount).CountPossibleArrangements(),
                '#' =>
                    // we are part (start, middle or end) of a group of damaged springs, so increment group count
                    new RowState(Springs[1..], ExpectedCounts, Path + '#', ContiguousDamagedSpringsCount + 1).CountPossibleArrangements(),
                '.' when ContiguousDamagedSpringsCount > 0 =>
                    // we have just finished a group, check its size is valid
                    ExpectedCounts.Length > 0 && ExpectedCounts[0] == ContiguousDamagedSpringsCount
                        ? new RowState(Springs[1..], ExpectedCounts[1..], Path + '.', 0).CountPossibleArrangements()
                        : 0, // this arrangement isn't valid, so don't continue, drop out, and don't count it
                '.' =>
                    new RowState(Springs[1..], ExpectedCounts, Path + '.', 0).CountPossibleArrangements(),
                var spring =>
                    throw new InvalidOperationException($"Unexpected spring: {spring}")
            };

            ArrangementsCountsCache[cacheKey] = count;

            return count;
        }
    }
}
