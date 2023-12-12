namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hot Springs";

    public long? SolvePart1(string input)
    {
        var rows = input.ReadLines()
            .Select(Row.Parse)
            //.Select(line => line.Split(' '))
            //.Select(split => new Row(split[0], split[1].Split(',').Select(int.Parse).ToArray()))
            .ToArray();

        return Solve(rows);
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    public record Row(string SpringConditions, int[] SizeOfEachContiguousGroupOfDamagedSprings, string Path = "")
    {
        public static Row Parse(string line)
        {
            var split = line.Split(' ');
            return new(split[0], split[1].Split(',').Select(int.Parse).ToArray());
        }
    }

    static long Solve(Row[] rows)
    {
        return rows.Sum(GetPossibleArrangements);
    }

    public static long GetPossibleArrangements(Row row)
    {
        // rs-todo: caching!
        return CountPossibleArrangements(row);
    }

    static long CountPossibleArrangements(Row row)
    {
        var (springs, sizes, pathSoFar) = row;

        if (springs[0] == '?')
        {
            return
                GetPossibleArrangements(row with { SpringConditions = '.' + springs[1..] }) +
                GetPossibleArrangements(row with { SpringConditions = '#' + springs[1..] });
        }
        else
        {
            var linearSection = string.Concat(springs.TakeWhile(spring => spring != '?'));
            var remainingSprings = springs[linearSection.Length..];

            var ourSizes = linearSection.ContiguousGroupBy(spring => spring)
                .Where(group => group.Key == '#')
                .Select(group => group.Count());

            // Our counts need to match
            // If they don't match, we can exclude this branch
            // When remaining becomes empty, that means we have a possible arrangement
            // Otherwise, we can recurse

            var matchedSizes = 0;

            foreach (var (ourSize, idx) in ourSizes.Select((ourSize, idx) => (ourSize, idx)))
            {
                if (idx >= sizes.Length)
                {
                    return 0;
                }

                var expectedSize = sizes[idx];
                if (expectedSize != ourSize)
                {
                    return 0;
                }

                matchedSizes++;
            }

            var remainingSizes = sizes[matchedSizes..];
            var path = pathSoFar + linearSection;

            if (remainingSprings == "" || remainingSprings.All(spring => spring == '.'))
            {
                if (remainingSizes.Length == 0)
                {
                    return 1;
                }

                return 0;
            }

            return GetPossibleArrangements(new Row(remainingSprings, remainingSizes, path));
        }
    }
}
