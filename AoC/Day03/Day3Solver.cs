using System.Collections.Frozen;

namespace AoC.Day03;

public partial class Day3Solver : ISolver
{
    public string DayName => "Gear Ratios";

    public long? SolvePart1(string input)
    {
        var (numbers, symbols) = ParseInput(input);

        var symbolAdjacencyMap = symbols.SelectMany(symbol => symbol.AdjacentPositions).ToHashSet().ToFrozenSet();

        return numbers.Where(number => number.IsPartNumber(symbolAdjacencyMap)).Sum(number => number.Value);
    }

    public long? SolvePart2(string input)
    {
        var (numbers, symbols) = ParseInput(input);

        var numberPositionMap = numbers
            .SelectMany(number => number.Positions.Select(position => new { position, number }))
            .ToDictionary(x => x.position, x => x.number)
            .ToFrozenDictionary();

        return symbols
            .Where(symbol => symbol.Char == '*')
            .Select(candidateGear => candidateGear.AdjacentPositions
                .Select(pos => numberPositionMap.TryGetValue(pos, out var number) ? number : null)
                .OfType<Number>()
                .Distinct()
                .ToArray())
            .Where(adjacentNumbers => adjacentNumbers.Length == 2)
            .Select(adjacentNumbers => adjacentNumbers[0].Value * adjacentNumbers[1].Value)
            .Sum();
    }

    record Number(long Value, Vector2 TopLeft, int Length)
    {
        public Vector2[] Positions { get; } = Enumerable.Range(0, Length).Select(x => TopLeft + new Vector2(x, 0)).ToArray();

        public bool IsPartNumber(ISet<Vector2> symbolAdjacencyMap) => Positions.Any(symbolAdjacencyMap.Contains);
    }

    record Symbol(char Char, Vector2 Position)
    {
        public Vector2[] AdjacentPositions { get; } = GridUtils.DirectionsIncludingDiagonal.Select(dir => Position + dir).ToArray();
    }

    static (Number[] Numbers, Symbol[] Symbols) ParseInput(string input)
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

        return (numbers.ToArray(), symbols.ToArray());
    }

    [GeneratedRegex(@"(?<number>\d+)|[^.]", RegexOptions.Compiled)]
    private static partial Regex ParseLineRegex();
}
