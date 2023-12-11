namespace AoC.Day11;

public class Day11Solver : ISolver
{
    public string DayName => "Cosmic Expansion";

    const char Space = '.';
    const char GalaxyChar = '#';

    public long? SolvePart1(string input) => ParseUniverse(ParseAndExpandInput(input)).GalaxyPairs.Sum(pair => pair.Distance);

    public long? SolvePart2(string input)
    {
        return null;
    }

    public record Universe(string[] Map, Galaxy[] Galaxies)
    {
        public GalaxyPair[] GalaxyPairs { get; } = GetGalaxyPairs(Galaxies);

        static GalaxyPair[] GetGalaxyPairs(Galaxy[] galaxies) =>
            galaxies.SelectMany(a => galaxies.Where(b => a != b).Select(b => new GalaxyPair(a, b)))
                .DistinctBy(item => string.Join("-", new[] { item.GalaxyA.Id, item.GalaxyB.Id }.Order()))
                .ToArray();
    }

    public record Galaxy(int Id, Vector2 Position);

    public record GalaxyPair(Galaxy GalaxyA, Galaxy GalaxyB)
    {
        public long Distance { get; } = MathUtils.ManhattanDistance(GalaxyA.Position, GalaxyB.Position);
    }

    public static string[] ParseAndExpandInput(string input)
    {
        var unexpanded = input.ReadLines().Select(line => new StringBuilder(line)).ToList();
        return ExpandUniverse(unexpanded);
    }

    public static Universe ParseUniverse(string[] expandedUniverse)
    {
        var nextId = 0;
        var galaxies = expandedUniverse.SelectMany(
            (line, y) => line.Select((chr, x) => (chr, x))
                .Where(item => item.chr == GalaxyChar)
                .Select(item => new Galaxy(++nextId, new Vector2(item.x, y))).ToArray()).ToArray();

        galaxies.Length.Dump("Num Galaxies");

        return new Universe(expandedUniverse, galaxies);
    }

    static string[] ExpandUniverse(List<StringBuilder> unexpanded)
    {
        // Expand the rows:
        for (int y = 0; y < unexpanded.Count; y++)
        {
            var line = unexpanded[y].ToString();
            if (line.ToString().All(c => c == Space))
            {
                y++;
                unexpanded.Insert(y, new StringBuilder(line));
            }
        }

        // Expand the columns:
        IEnumerable<char> GetColumn(int x) => unexpanded.Select(line => line[x]);
        void ExpandColumn(int x) => unexpanded.ForEach(line => line.Insert(x, Space));
        for (int x = 0; x < unexpanded[0].Length; x++)
        {
            if (GetColumn(x).All(c => c == Space))
            {
                x++;
                ExpandColumn(x);
            }
        }

        return unexpanded.Select(line => line.ToString()).ToArray();
    }
}
