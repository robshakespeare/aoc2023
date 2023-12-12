namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hot Springs";

    public long? SolvePart1(string input) => SumCountOfPossibleArrangements(input);

    public long? SolvePart2(string input)
    {
        return null;
    }

    static long SumCountOfPossibleArrangements(string input)
    {
        Dictionary<string, long> arrangementsCountsCache = [];
        var rows = input.ReadLines()
            .Select(line => RowState.Parse(line, arrangementsCountsCache))
            .ToArray();

        return rows.Sum(row => row.CountPossibleArrangements());
    }

    public record RowState(string Springs, int[] ExpectedCounts, int ContiguousDamagedSpringsCount, Dictionary<string, long>? Cache)
    {
        public static RowState Parse(string line, Dictionary<string, long>? cache = null)
        {
            var split = line.Split(' ');
            return new(split[0] + '.', split[1].Split(',').Select(int.Parse).ToArray(), 0, cache);
        }

        public long CountPossibleArrangements()
        {
            if (Springs.Length == 0)
            {
                // We've reached the end of this arrangement, so count it if we've consumed all of the group counts
                return ExpectedCounts.Length == 0 ? 1 : 0;
            }

            var cacheKey = $"{Springs}_{string.Join(',', ExpectedCounts)}_{ContiguousDamagedSpringsCount}";

            if (Cache?.TryGetValue(cacheKey, out var cachedCount) == true)
            {
                return cachedCount;
            }

            var count = Springs[0] switch
            {
                '?' => // at this point, the arrangement splits in to 2 possibilities, either an undamaged spring '.', or a damaged spring '#'
                    (this with { Springs = '.' + Springs[1..] }).CountPossibleArrangements() +
                    (this with { Springs = '#' + Springs[1..] }).CountPossibleArrangements(),

                '#' => // we are part (start, middle or end) of a group of damaged springs, so increment group count
                    new RowState(Springs[1..], ExpectedCounts, ContiguousDamagedSpringsCount + 1, Cache).CountPossibleArrangements(),

                '.' when ContiguousDamagedSpringsCount > 0 => // we have just finished a group, so check its size is valid
                    ExpectedCounts.Length > 0 && ExpectedCounts[0] == ContiguousDamagedSpringsCount
                        ? new RowState(Springs[1..], ExpectedCounts[1..], 0, Cache).CountPossibleArrangements()
                        : 0, // this arrangement isn't valid, so don't continue, drop out, and don't count it

                '.' => new RowState(Springs[1..], ExpectedCounts, 0, Cache).CountPossibleArrangements(),
                var spring => throw new InvalidOperationException($"Unexpected spring: {spring}")
            };

            Cache?.Add(cacheKey, count);

            return count;
        }
    }
}
