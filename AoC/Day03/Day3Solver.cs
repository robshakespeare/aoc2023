using System.Collections.Frozen;

namespace AoC.Day03;

public partial class Day3Solver : ISolver
{
    public string DayName => "Gear Ratios";

    public long? SolvePart1(string input)
    {
        var (numbers, symbols, _) = ParseInput(input);

        var symbolAdjacencyMap = symbols.SelectMany(symbol => symbol.AdjacentPositions).ToHashSet().ToFrozenSet();

        return numbers.Where(number => number.IsPartNumber(symbolAdjacencyMap)).Sum(number => number.Value);
    }

    public long? SolvePart2(string input)
    {
        var map = ParseInput(input);

        //var numberPositionMap = map.Numbers
        //    .SelectMany(number => number.Positions.Select(position => new { position, number }))
        //    .ToDictionary(x => x.position, x => x.number)
        //    .ToFrozenDictionary();

        return map.Symbols
            .Where(symbol => symbol.Char == '*')
            .Select(candidateGear => new
            {
                candidateGear,
                adjacentNumbers = map.GetNumbers(candidateGear.AdjacentPositions).ToArray(),
                //adjacentNumbers = candidateGear.AdjacentPositions
                //    .Select(pos => numberPositionMap.TryGetValue(pos, out var number) ? number : null)
                //    .Where(number => number != null)
                //    .Select(number => number!)
                //    .Distinct()
                //    .ToArray()
            })
            .Where(x => x.adjacentNumbers.Length == 2)
            .Select(x => x.adjacentNumbers[0].Value * x.adjacentNumbers[1].Value)
            .Sum();
    }

    record Number(long Value, Vector2 TopLeft, int Length) /*: Element(Position)*/
    {
        public Vector2[] Positions { get; } = Enumerable.Range(0, Length).Select(x => TopLeft + new Vector2(x, 0)).ToArray();

        public bool IsPartNumber(ISet<Vector2> symbolAdjacencyMap) => Positions.Any(symbolAdjacencyMap.Contains);
    }

    record Symbol(char Char, Vector2 Position) /*: Element(Position)*/
    {
        public Vector2[] AdjacentPositions { get; } = GridUtils.DirectionsIncludingDiagonal.Select(dir => Position + dir).ToArray();

        //public bool IsGear => Char == '*';
    }

    record Map(Number[] Numbers, Symbol[] Symbols, Number?[][] Grid)
    {
        //public IEnumerable<TElement> GetElementsOfType<TElement>(IEnumerable<Vector2> positions) where TElement : Element =>
        //    positions.Select(Grid.SafeGet).OfType<TElement>().Distinct();

        public IEnumerable<Number> GetNumbers(IEnumerable<Vector2> positions) =>
            positions.Select(Grid.SafeGet).OfType<Number>().Distinct();
    }

    static Map ParseInput(string input)
    {
        var lines = input.ReadLines().ToArray();
        var width = lines.First().Length;
        var grid = Enumerable.Range(0, lines.Length).Select(y => new Number?[width]).ToArray();

        var numbers = new List<Number>();
        var symbols = new List<Symbol>();

        foreach (var (line, y) in lines.Select((line, y) => (line, y)))
        {
            foreach (Match match in ParseLineRegex().Matches(line))
            {
                var position = new Vector2(match.Index, y);

                if (match.Groups["number"].Success)
                {
                    var number = new Number(long.Parse(match.Value), position, match.Length);
                    numbers.Add(number);
                    number.Positions.ToList().ForEach(pos => grid[(int)pos.Y][(int)pos.X] = number);
                }
                else
                {
                    symbols.Add(new Symbol(match.Value.Single(), position));
                }
            }
        }

        //numbers.ForEach(number => number.Positions.ToList().ForEach(pos => grid[(int)pos.Y][(int)pos.X] = number));

        return new Map([.. numbers], [.. symbols], grid);
    }

    [GeneratedRegex(@"(?<number>\d+)|[^.]", RegexOptions.Compiled)]
    private static partial Regex ParseLineRegex();
}
