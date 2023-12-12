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
        return rows.Sum(row => row.CountPossibleArrangements());
    }

    public class RowState
    {
        private readonly string springs;
        private readonly int[] expectedCounts;
        private readonly string path;
        private readonly int contiguousDamagedSpringsCount;

        private RowState(string springs, int[] expectedCounts, string path, int contiguousDamagedSpringsCount)
        {
            this.springs = springs;
            this.expectedCounts = expectedCounts;
            this.path = path;
            this.contiguousDamagedSpringsCount = contiguousDamagedSpringsCount;
        }

        public static RowState Parse(string line)
        {
            var split = line.Split(' ');
            return new(split[0] + '.', split[1].Split(',').Select(int.Parse).ToArray(), "", 0);
        }

        public long CountPossibleArrangements()
        {
            if (springs.Length == 0)
            {
                // We've reached the end of this arrangement, so count it if we've consumed all of the group counts
                return expectedCounts.Length == 0 ? 1 : 0;
            }

            // rs-todo: caching!

            var count = springs[0] switch
            {
                '?' =>
                    // at this point, the arrangement splits in to 2 possibilities, either an undamaged spring '.', or a damaged spring '#'
                    new RowState('.' + springs[1..], expectedCounts, path, contiguousDamagedSpringsCount).CountPossibleArrangements() +
                    new RowState('#' + springs[1..], expectedCounts, path, contiguousDamagedSpringsCount).CountPossibleArrangements(),
                '#' =>
                    // we are part (start, middle or end) of a group of damaged springs, so increment group count
                    new RowState(springs[1..], expectedCounts, path + '#', contiguousDamagedSpringsCount + 1).CountPossibleArrangements(),
                '.' when contiguousDamagedSpringsCount > 0 =>
                    // we have just finished a group, check its size is valid
                    expectedCounts.Length > 0 && expectedCounts[0] == contiguousDamagedSpringsCount
                        ? new RowState(springs[1..], expectedCounts[1..], path + '.', 0).CountPossibleArrangements()
                        : 0, // this arrangement isn't valid, so don't continue, drop out, and don't count it
                '.' =>
                    new RowState(springs[1..], expectedCounts, path + '.', 0).CountPossibleArrangements(),
                var spring =>
                    throw new InvalidOperationException($"Unexpected spring: {spring}")
            };

            return count;

            //var spring = springs[0];
            //if (spring == '?')
            //{
            //    return
            //        new RowState('.' + springs[1..], expectedCounts, path, contiguousDamagedSpringsCount).CountPossibleArrangements() +
            //        new RowState('#' + springs[1..], expectedCounts, path, contiguousDamagedSpringsCount).CountPossibleArrangements();
            //}
            //else if (spring == '#')
            //{
            //    // we are part (start, middle or end) of a group of damaged springs, so increment group count
            //    return new RowState(springs[1..], expectedCounts, path + spring, contiguousDamagedSpringsCount + 1).CountPossibleArrangements();
            //}
            //else if (spring == '.')
            //{
            //    // if we have just finished a group, check its size is valid
            //    if (contiguousDamagedSpringsCount > 0)
            //    {
            //        var isValid = expectedCounts.Length > 0 && expectedCounts[0] == contiguousDamagedSpringsCount;
            //        if (isValid)
            //        {
            //            return new RowState(springs[1..], expectedCounts[1..], path + spring, 0).CountPossibleArrangements();
            //        }
            //        else
            //        {
            //            return 0; // this arrangement isn't valid, so don't continue, drop out, and don't count it
            //        }
            //    }

            //    return new RowState(springs[1..], expectedCounts, path + spring, 0).CountPossibleArrangements();
            //}
            //else
            //{
            //    throw new InvalidOperationException($"Unexpected spring: {spring}");
            //}
        }
    }
}
