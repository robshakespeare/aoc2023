using System.Collections.Frozen;

namespace AoC.Day03;

public partial class Day3Solver : ISolver
{
    public string DayName => "";

    public long? SolvePart1(string input)
    {
        var (numbers, symbols, adjacencyMap) = ParseInput(input);

        return numbers.Where(num => num.IsPartNumber(adjacencyMap)).Sum(num => num.Value);
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    record Number(long Value, Vector2 TopLeft, int Length)
    {
        public Vector2[] Positions { get; } = Enumerable.Range(0, Length).Select(x => TopLeft + new Vector2(x, 0)).ToArray();

        public bool IsPartNumber(ISet<Vector2> AdjacencyMap) => Positions.Any(AdjacencyMap.Contains);
    }

    record Symbol(char Char, Vector2 Position)
    {
        public Vector2[] AdjacentPositions { get; } = GridUtils.DirectionsIncludingDiagonal.Select(dir => Position + dir).ToArray();
    }

    static (Number[] Numbers, Symbol[] Symbols, FrozenSet<Vector2> AdjacencyMap) ParseInput(string input)
    {
        var numbers = new List<Number>();
        var symbols = new List<Symbol>();

        foreach (var (line, y) in input.ReadLines().Select((line, y) => (line, y)))
        {
            foreach (Match match in ParseLineRegex().Matches(line))
            {
                var position = new Vector2(match.Index, y);

                if (match.Groups["number"].Success)
                {
                    numbers.Add(new Number(long.Parse(match.Value), position, match.Length));
                }
                else
                {
                    symbols.Add(new Symbol(match.Value.Single(), position));
                }
            }
        }

        var symbolAdjacentPositions = symbols.SelectMany(x => x.AdjacentPositions).ToHashSet().ToFrozenSet();

        return (numbers.ToArray(), symbols.ToArray(), symbolAdjacentPositions);
    }

    [GeneratedRegex(@"(?<number>\d+)|[^.]", RegexOptions.Compiled)]
    private static partial Regex ParseLineRegex();
}
