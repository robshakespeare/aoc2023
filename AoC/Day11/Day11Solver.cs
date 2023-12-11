using Spectre.Console;

namespace AoC.Day11;

public class Day11Solver : ISolver
{
    public string DayName => "Cosmic Expansion";

    const char Space = '.';
    const char GalaxyChar = '#';

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

    public static long ParseExpandAndSumDistances(string input, int expansionAmount = 1) =>
        ParseAndExpandUniverse(input, expansionAmount).GetGalaxyPairs().Sum(pair => pair.Distance);

    public static Universe ParseAndExpandUniverse(string input, int expansionAmount = 1)
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

        //var universeHeight = unexpanded.Length;
        //var universeWidth = unexpanded[0].Length;

        // Expand the rows:
        {
            var rowsToExpand = unexpanded
                .Select((row, rowIndex) => (row, rowIndex))
                .Where(item => item.row.All(c => c == Space))
                .Select(item => item.rowIndex)
                .ToArray();

            for (var row = 0; row < rowsToExpand.Length; row++)
            {
                var y = rowsToExpand[row];

                //for (int y = 0; y < universeHeight; y += expansionAmount)
                //{

                //var line = unexpanded[y].ToString();
                //if (line.ToString().All(c => c == Space))
                //{

                expandedUniverse = new Universe(
                    expandedUniverse.Galaxies.Select(galaxy =>
                    {
                        //var newPosition = g.Position.Y < y ? g.Position : new Vector2(g.Position.X, g.Position.Y + expansionAmount);
                        var newPosition = galaxy.Position with { Y = galaxy.Position.Y + (galaxy.Position.Y < y ? 0 : expansionAmount) };
                        return galaxy with { Position = newPosition };
                    }).ToArray());

                //y += expansionAmount;
                //universeHeight += expansionAmount;
                rowsToExpand = rowsToExpand.Select(n => n + expansionAmount).ToArray();

                //}
            }
        }


        // Expand the columns:
        {
            IEnumerable<char> GetColumn(int x) => unexpanded.Select(line => line[x]);
            var columnsToExpand = unexpanded
                .Select((_, columnIndex) => columnIndex)
                .Where(columnIndex => GetColumn(columnIndex).All(c => c == Space))
                .ToArray();

            for (var column = 0; column < columnsToExpand.Length; column++)
            {
                var x = columnsToExpand[column];

                expandedUniverse = new Universe(
                    expandedUniverse.Galaxies.Select(galaxy =>
                    {
                        //var newPosition = g.Position.Y < y ? g.Position : new Vector2(g.Position.X, g.Position.Y + expansionAmount);
                        var newPosition = galaxy.Position with { X = galaxy.Position.X + (galaxy.Position.X < x ? 0 : expansionAmount) };
                        return galaxy with { Position = newPosition };
                    }).ToArray());

                //y += expansionAmount;
                //universeHeight += expansionAmount;
                columnsToExpand = columnsToExpand.Select(n => n + expansionAmount).ToArray();

                //if (GetColumn(x).All(c => c == Space))
                //{
                //    x += expansionAmount;
                //    ExpandColumn(x);
                //}
            }
        }
        

        return expandedUniverse;
    }
}
