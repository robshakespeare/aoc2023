namespace AoC.Day18;

/// <summary>
/// After realising that the solution must be something to do with working out square floor space of an irregular building floor by tracing edges,
/// I searched and came across https://forums.anandtech.com/threads/quirky-problem-calculating-the-area-of-an-irregular-shape-programmatically.317692/
/// Afetr attempting to implemenet that, and realising potential issues, with more searching I came across Phil Scovis' Quora answer
/// here: https://www.quora.com/How-do-you-calculate-the-area-of-an-irregular-shape-and-how-do-you-divide-it-into-equal-parts
/// Solution is essentially the area of polygon given the coordinates of the vertices, in order, but also needing to account for the inclusive bounds
/// of the integer coordinate system.  More reading afterwards, I disoverred the names for these algorithms/theorems are:
/// * Shoelace formula, also known as Gauss's area formula, for polygonal area;
/// * and Pick's theorem, to cater for the need for inclusive bounds (down and left edges) because of integer coordinates.
///
/// After implementing the above, I spent far too long trying to work out why part 1 still worked, but part 2 didn't;
/// until realising the implementation was fine except for it had just reached the end of single precision floating point accuracy!!
/// Changed to `double` and it was fine!
/// </summary>
public partial class Day18Solver : ISolver
{
    public string DayName => "Lavaduct Lagoon";

    public long? SolvePart1(string input)
    {
        var instructions = input.ReadLines()
            .Select(line => line.Split(' '))
            .Select(split => new Instruction(split[0].Single(), long.Parse(split[1])))
            .ToArray();

        return CalculateTrenchDigOutArea(instructions);
    }

    public long? SolvePart2(string input)
    {
        var instructions = ParseHexCodesRegex().Matches(input)
            .Select(match => new Instruction(
                match.Value[^1] switch
                {
                    '0' => 'R',
                    '1' => 'D',
                    '2' => 'L',
                    '3' => 'U',
                    var invalidDir => throw new Exception("Invalid hex dir: " + invalidDir)
                },
                Convert.ToInt64(match.Value[..^1], 16)))
            .ToArray();

        return CalculateTrenchDigOutArea(instructions);
    }

    record Instruction(char Direction, long Amount);

    static long ShoelaceArea(IList<Vector2> vertices)
    {
        var b = vertices
            .Select((v, i) => (Current: v, Next: vertices[(i + 1) % vertices.Count]))
            .Sum(pair => ((double)pair.Current.X * pair.Next.Y) - ((double)pair.Current.Y * pair.Next.X));

        return (long)(Math.Abs(b) / 2);
    }

    static long CalculateTrenchDigOutArea(Instruction[] instructions)
    {
        var position = Vector2.Zero;
        List<Vector2> vertices = [position];
        long perimeter = 0;

        foreach (var instruction in instructions)
        {
            var dir = instruction.Direction switch
            {
                'U' => GridUtils.North,
                'D' => GridUtils.South,
                'L' => GridUtils.West,
                'R' => GridUtils.East,
                var invalidDir => throw new Exception("Invalid dir: " + invalidDir)
            };
            var amount = instruction.Amount;

            position += dir * amount;
            vertices.Add(position);

            perimeter += amount;
        }

        var shoelaceArea = ShoelaceArea(vertices);

        Console.WriteLine($"Shoelace Area: {shoelaceArea}");
        Console.WriteLine($"Perimeter / 2: {perimeter / 2}");

        return shoelaceArea + perimeter / 2 + 1;
    }

    [GeneratedRegex(@"\w{6}")]
    private static partial Regex ParseHexCodesRegex();
}
