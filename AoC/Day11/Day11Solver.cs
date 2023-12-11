namespace AoC.Day11;

public class Day11Solver : ISolver
{
    public string DayName => "Cosmic Expansion";

    const char Space = '.';
    const char GalaxyChar = '#';
    const int DefaultExpansionAmount = 2;

    public long? SolvePart1(string input) => ParseExpandAndSumDistances(input);

    public long? SolvePart2(string input) => ParseExpandAndSumDistances(input, 1_000_000);

    public record Universe(Galaxy[] Galaxies)
    {
        public GalaxyPair[] GetGalaxyPairs() =>
            Galaxies.SelectMany(a => Galaxies.Where(b => a != b).Select(b => new GalaxyPair(a, b)))
                .DistinctBy(item => string.Join("-", new[] { item.GalaxyA.Id, item.GalaxyB.Id }.Order()))
                .ToArray();
    }

    public record Galaxy(int Id, Vector2 Position);

    public record GalaxyPair(Galaxy GalaxyA, Galaxy GalaxyB)
    {
        public long Distance { get; } = MathUtils.ManhattanDistance(GalaxyA.Position, GalaxyB.Position);
    }

    public static long ParseExpandAndSumDistances(string input, int expansionAmount = DefaultExpansionAmount) =>
        ParseAndExpandUniverse(input, expansionAmount).GetGalaxyPairs().Sum(pair => pair.Distance);

    public static Universe ParseAndExpandUniverse(string input, int expansionAmount = DefaultExpansionAmount)
    {
        var unexpanded = input.ReadLines().Select(line => line).ToArray();
        return ExpandUniverse(unexpanded, expansionAmount);
    }

    static Universe ParseUniverse(string[] input)
    {
        var nextId = 0;
        var galaxies = input.SelectMany(
            (line, y) => line.Select((chr, x) => (chr, x))
                .Where(item => item.chr == GalaxyChar)
                .Select(item => new Galaxy(++nextId, new Vector2(item.x, y))).ToArray()).ToArray();

        return new Universe(galaxies);
    }

    static Universe ExpandUniverse(string[] unexpanded, int expansionAmount)
    {
        var expandedUniverse = ParseUniverse(unexpanded);
        expansionAmount -= 1;

        void ExpandAxis(int[] indexesToExpand, int component)
        {
            for (var idx = 0; idx < indexesToExpand.Length; idx++)
            {
                var value = indexesToExpand[idx];

                expandedUniverse = new Universe(
                    expandedUniverse.Galaxies.Select(galaxy =>
                    {
                        var newPosition = galaxy.Position;
                        newPosition[component] += galaxy.Position[component] < value ? 0 : expansionAmount;
                        return galaxy with { Position = newPosition };
                    }).ToArray());

                indexesToExpand = indexesToExpand.Select(n => n + expansionAmount).ToArray();
            }
        }

        // Expand the rows:
        var rowsToExpand = unexpanded
                .Select((row, rowIndex) => (row, rowIndex))
                .Where(item => item.row.All(c => c == Space))
                .Select(item => item.rowIndex)
                .ToArray();

        ExpandAxis(rowsToExpand, 1);

        // Expand the columns:
        IEnumerable<char> GetColumn(int x) => unexpanded.Select(line => line[x]);
        var columnsToExpand = unexpanded
            .Select((_, columnIndex) => columnIndex)
            .Where(columnIndex => GetColumn(columnIndex).All(c => c == Space))
            .ToArray();

        ExpandAxis(columnsToExpand, 0);

        return expandedUniverse;
    }
}
