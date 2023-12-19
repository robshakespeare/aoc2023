using System.Drawing;

namespace AoC.Day18;

/// <summary>
/// Solution is essentially the area of polygon given the coordinates of the vertices, in order.
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

        ///////////// Hmmmm:
        //var position = Vector2.Zero;
        //List<Vector2> vertices = [position];
        //HashSet<Vector2> perimeter = [position];

        //foreach (var line in input.ReadLines())
        //{
        //    var split = line.Split(' ');
        //    var dir = split[0] switch
        //    {
        //        "U" => GridUtils.North,
        //        "D" => GridUtils.South,
        //        "L" => GridUtils.West,
        //        "R" => GridUtils.East,
        //        _ => throw new Exception("Invalid dir: " + split[0])
        //    };
        //    var amount = int.Parse(split[1]);

        //    //position += dir * amount;
        //    //vertices.Add(position);

        //    //position += dir * (amount + 1);
        //    //Console.WriteLine(position);

        //    for (var i = 0; i < amount; i++)
        //    {
        //        position += dir;
        //        perimeter.Add(position);
        //    }

        //    vertices.Add(position);
        //}

        //var b = vertices
        //    .Select((v, i) => (Current: v, Next: vertices[(i + 1) % vertices.Count]))
        //    .Sum(pair => (pair.Current.X * pair.Next.Y) - (pair.Current.Y * pair.Next.X));

        //var area = (long)Math.Abs(b / 2);

        //return area + (perimeter.Count / 2) + 1;



        // Plot, only works for example! Actual is too big!
        //Console.WriteLine(string.Join(Environment.NewLine, vertices.ToStringGrid(
        //    p => p,
        //    p => (char)('A' + vertices.IndexOf(p)),
        //    '.')));

        ///////////// Hmmmm:

        //var instructions = input.ReadLines()
        //    .Select(line => line.Split(' '))
        //    .Select(split => new Instruction(split[0].Single(), long.Parse(split[1])))
        //    .ToArray();

        //var instructionPairs = instructions
        //    .Select((instruction, idx) => (instruction, idx))
        //    .GroupBy(x => x.idx / 2)
        //    .Select(grp =>
        //    {
        //        if (grp.Count() != 2)
        //        {
        //            throw new Exception("Invalid pairing");
        //        }

        //        return (Horizontal: grp.First().instruction, Vertical: grp.Last().instruction);
        //    })
        //    .ToArray();

        //long u = 0;

        //var (rSign, lSign) = instructionPairs[0].Horizontal.Direction switch
        //{
        //    'R' => (+1, -1),
        //    'L' => (-1, +1),
        //    var invalid => throw new FormatException("Invalid format, horizontal moves should be first in pair, but got: " + invalid)
        //};

        //var (uSign, dSign) = instructionPairs[0].Vertical.Direction switch
        //{
        //    'U' => (+1, -1),
        //    'D' => (-1, +1),
        //    var invalid => throw new FormatException("Invalid format, vertical moves should be second in pair, but got: " + invalid)
        //};

        //List<long> areas = [];

        //foreach (var (horizontal, vertical) in instructionPairs)
        //{
        //    var verticalAmount = (vertical.Direction == 'U' ? uSign : dSign) * vertical.Amount;

        //    u += verticalAmount;

        //    var horizontalAmount = (horizontal.Direction == 'R' ? rSign : lSign) * horizontal.Amount;

        //    var area = u * horizontalAmount;
        //    areas.Add(area);
        //}

        //return areas.Sum();
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

    static double ShoelaceArea(List<Vector2> v)
    {
        int n = v.Count;
        double a = 0.0;
        for (int i = 0; i < n - 1; i++)
        {
            a += ((double)v[i].X) * ((double)v[i + 1].Y) - ((double)v[i + 1].X) * ((double)v[i].Y);
        }
        return Math.Abs(a + v[n - 1].X * v[0].Y - v[0].X * v[n - 1].Y) / 2.0;
    }


    /// <summary>
    /// Shoelace formula, also known as Gauss's area formula, for polygonal area;
    /// and Pick's theorem, to cater for the polygon being in integer coordinates.
    /// </summary>
    static long CalculateTrenchDigOutArea(Instruction[] instructions)
    {
        var position = Vector2.Zero;
        List<Vector2> vertices = [position];
        //HashSet<Vector2> perimeter = [position];

        long perimeter = 0;

        long exclusivePerimeter = 0;

        long instructionCounter = 0;

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

            exclusivePerimeter += instruction.Direction is 'D' or 'L' ? amount : 0;


            //position += dir * (amount + 1);
            Console.WriteLine($"{++instructionCounter}: {position}");

            //for (var i = 0; i < amount; i++)
            //{
            //    position += dir;
            //    perimeter.Add(position);
            //}

            //vertices.Add(position);
        }

        //perimeter += 4;

        //var b = vertices
        //    .Select((v, i) => (Current: v, Next: vertices[(i + 1) % vertices.Count]))
        //    .Sum(pair => (pair.Current.X * pair.Next.Y) - (pair.Current.Y * pair.Next.X));

        //var area = (long) Math.Abs(b) / 2;

        var b = vertices
            .Select((v, i) => (Current: v, Next: vertices[(i + 1) % vertices.Count]))
            .Sum(pair => ((double)pair.Current.X * pair.Next.Y) - ((double)pair.Current.Y * pair.Next.X));

        var shoelaceArea = (long)(Math.Abs(b) / 2);

        //var shoelaceArea = (long) ShoelaceArea(vertices);

        Console.WriteLine($"Shoelace Area: {shoelaceArea}");
        Console.WriteLine($"Perimeter / 2: {perimeter / 2}");
        Console.WriteLine($"Exclusive Perimeter: {exclusivePerimeter}");

        //return area + perimeter;

        return shoelaceArea + exclusivePerimeter + 1;

        //return shoelaceArea + perimeter / 2 + 1;
    }

    [GeneratedRegex(@"\w{6}")]
    private static partial Regex ParseHexCodesRegex();
}
